# Engineering Decisions

This document records the major design decisions behind Knight of Cinders. It is intentionally reflective: the goal is to explain not only what I built, but why I built it this way and what tradeoffs came with those choices.

## 1. Why use finite state machines for actors?

FSMs solved the biggest runtime problem in this project: keeping actor behavior explicit while the number of actions kept growing. The player alone needs locomotion, attacks, defend, roll, jump, hurt, death, and invulnerability, while enemies and bosses need their own decision loops, attacks, hurt states, and phase-specific transitions. The FSM structure let each actor own a bounded set of valid behaviors instead of burying everything in one long update loop.

This was better than one controller because the project had already started to run into animation and state-mixing problems before the current FSM structure was established. In practice, explicit states made it much easier to control animation entry, timing-sensitive combat windows, and transitions such as `Idle -> Attack`, `Run -> Roll`, or `Charge -> Vulnerable` without accidental overlap. The player root in `script/Player/Knight.cs` and the enemy roots in `script/EnemyAI/...` all follow the same pattern: register states, store the current state, and swap through `TransitionState(...)`.

What became easier after that:
- isolating animation-driven behavior inside one state at a time
- adding new actions without rewriting one giant decision function
- reusing the same control pattern across player, skeletons, Dark Wolf, and Evil Wizard
- reasoning about boss phases as explicit runtime modes instead of boolean soup

The downsides are mostly scale and maintenance. As the action set grows, the number of state classes grows with it. That is manageable in this project, but if it expanded further I would likely introduce more hierarchy or shared abstractions for related movement/combat states instead of keeping every state entirely flat.

### Why not Animator transitions?

Animator transitions were useful for visual playback, but I did not want the Animator to become the main source of gameplay truth. Combat windows, stamina checks, hurt rules, roll invulnerability, and death transitions all needed explicit C# control. In this project, animation is driven by state changes, not the other way around. That kept runtime rules readable in code, reduced the risk of gameplay logic being buried inside Animator graphs, and avoided turning the Animator into a large web of transition conditions and booleans that would have been harder to debug than direct code.

### Why not behavior trees?

Behavior trees would have been useful for broader AI decision-making, especially in a larger game with more branching enemy logic. For this project, though, most actors follow relatively clear action states and phase transitions: idle, move, attack, hurt, die, and boss-specific modes like vulnerable or laser wall. FSMs were simpler to implement, easier to debug, and a better fit for the scope and pacing of these encounters.

## 2. Why use manager components instead of one large controller?

I separated responsibilities into focused manager components because combat, movement, stamina, health, feedback, UI, and progression all change for different reasons. On the player side, `MovementManager`, `CombatManager`, `StaminaManager`, and `HealthManager` each own a distinct slice of runtime behavior. On the enemy side, the same pattern appears again through base capability layers like `EnemyMovementManager`, `EnemyCombatManager`, and `EnemyHealthManager`, plus specialized managers for bosses and encounters.

This helped debugging and iteration because balance or bug-fixing work was usually localized. If I needed to tune stamina pressure, I could stay in `StaminaManager`. If roll rules or jump movement felt wrong, the relevant logic lived in `MovementManager`. If hit resolution or combo timing felt off, the work was in `CombatManager` and `HitBoxManager`. That separation mattered a lot in a project where most validation was manual playtesting.

The weakness is that the code is separated by responsibility, but not fully decoupled. Many managers still reach each other through serialized references and direct component access, especially through parameter structs like `KnightParameter` and enemy parameter containers. So the boundaries are cleaner than a monolith, but not yet truly interface-driven.

The strongest manager boundaries in practice were:
- `HealthManager`, because health, death, and UI broadcast behavior stayed relatively coherent
- `StaminaManager`, because action gating and regeneration rules are clearly centralized
- `GamePlayCoordinator`, because encounter lifecycle concerns are kept out of combat code

The weakest boundaries are around player combat and movement, where responsiveness required a lot of cross-checking between stamina, movement, combat flags, animator state, and hurt/death conditions.

### Why isn't health inside `Knight`?

Health was separated because it changes for different reasons than movement or combat. `Knight` should own actor identity and state transitions, while `HealthManager` owns damage intake, passive healing, death checks, and health event broadcasts. That separation made it easier to connect health changes to UI and defeat flow without turning the actor root into a catch-all script.

### Why isn't movement inside `Knight`?

Movement has its own rules for velocity, jumping, rolling, gravity, facing direction, swimming, and motion locks. Keeping that inside `Knight` would have made the actor root too large and too responsible for low-level physics behavior. `Knight` works better as the FSM owner and high-level state root, while `MovementManager` executes the locomotion rules that those states allow.

## 3. Why use an EventManager?

Several systems needed to react to the same gameplay event without being tightly coupled to each other. A hit should affect combat resolution, health, camera shake, audio feedback, and UI. A boss-fight transition should affect barriers, cameras, ambience, music, and fight cleanup. A win or defeat event should affect cutscenes and presentation flow. That is exactly what `script/EventManager.cs` is doing.

Events helped separate combat, UI, feedback, audio, and progression because the event source does not need to know every downstream reaction. `HitBoxManager` can raise `OnHitOccured`, while `CombatManager`, enemy combat managers, `CombatFeedbackManager`, and `AudioFeedbackManager` each react in their own way. `GamePlayCoordinator`, `UIManager`, `AmbienceManager`, `BackGroundMusicManager`, and cutscene helpers all subscribe to different slices of the same event layer.

The main risk of a global event bus is traceability. When many systems subscribe to a shared static hub, the runtime flow becomes harder to follow than direct method calls. That is a tradeoff I accepted because it simplified cross-system coordination, but it does mean event-driven bugs can take longer to trace.

Direct references still remain necessary in several places:
- serialized references in the Unity inspector
- parameter objects that bundle related runtime components
- actor-specific manager calls where one subsystem genuinely owns another runtime rule

So the project is not “everything through events.” The event layer helps for cross-system reactions, while direct references still handle local actor behavior.

### Why use an `EventManager` instead of direct references everywhere?

A single hit needs multiple systems to react at once. If I used only direct references, combat code would need to know about health, UI, audio, camera shake, hit stop, and sometimes progression or cutscene consequences. Events let the hit source broadcast once and let each downstream system react independently. That is the main reason this project uses both direct actor references and a shared event layer instead of trying to force everything through one approach.

### Can I explain the event flow with one concrete gameplay example?

Yes. A sword hit on a skeleton is the clearest example:
- the attack collider reaches the target in `HitBoxManager.OnTriggerEnter2D()`
- `HitBoxManager` creates `HitData`, including runtime damage, knockback direction, and feedback payload
- `EventManager.RaiseHitOccured(data)` broadcasts the hit
- an enemy combat or health layer reacts and reduces health
- `CombatFeedbackManager` reacts with hit stop and camera shake
- `AudioFeedbackManager` reacts with impact audio
- UI reacts later through the health event broadcast from the health manager

That flow is a good example of why events were useful here: one gameplay moment fans out into multiple presentation and rule systems without making the hit source call every one directly.

## 4. Why create SkeletonCoordinator instead of making each skeleton independent?

The skeleton encounter needed behavior that no single skeleton could own alone. The point of that fight is not just “several enemies chasing the player.” It is a coordinated group with role assignment, replacement, and pressure from different angles. That is why `SkeletonCoordinator` exists on top of the local skeleton FSMs.

Roles like `Frontliner`, `Flanker`, and `Backuper` improved the fight because they created different tactical jobs:
- frontliners maintain direct melee pressure
- the flanker changes spacing and attack timing from another angle
- backupers hold until promoted, which prevents the encounter from collapsing once active roles die

The coordination work was hard because it required tracking live combat state across multiple enemies at once: who is dead, who is weak enough to replace, who should become the next flanker, and how role reassignment changes attack behavior. The implementation evidence for this is in `script/EnemyAI/Skeleton_New/SkeletonCoordinator.cs`, where the current frontliners and flanker are tracked explicitly and replaced when needed.

The main edge cases came from runtime reassignment rather than from the local FSM itself. Promotion logic has to avoid dead skeletons, avoid assigning the wrong role twice, and keep encounter pressure readable while the roster changes. Even when those bugs are solvable, they are the hardest part of this encounter to reason about because the logic is distributed across both coordinator and unit-level behavior.

## 5. Why split Evil Wizard into phase-specific controllers?

The Evil Wizard was split into two controllers because phase 1 and phase 2 are meaningfully different actors in practice, not just two small variants of the same attack loop. Phase 1 focuses on teaching telegraphs and melee/ranged mixups. Phase 2 adds a different state set, different attack families, summons, homing lasers, laser wall logic, and a different escalation structure. Keeping all of that in one giant boss controller would have created a much harder class to reason about.

The split made the boss easier to understand because each controller owns a tighter state set:
- `EvilWizard` + `EW1CombatManager` + phase 1 states
- `EvilWizardPhase2` + `EW2CombatManager` + phase 2 states

That reduced the amount of phase-branching logic inside a single update path and let the existing FSM pattern stay readable. It also matched the production reality that the animation and attack behavior between the two phases are substantially different.

The tradeoff is duplication and extra coordination cost. Two controllers means more objects, more serialized references, and more setup for the phase transition itself. If I rebuilt this today, I would likely keep the phase split, but I would push harder on reusing shared boss utilities and configuration data so the split stays clean without duplicating as much runtime plumbing.

## 6. Why separate combat, health, stamina, and feedback?

I did not want attack code to own every consequence of an action. If attack logic also handled damage resolution, stamina rules, UI updates, hit stop, camera shake, audio, death, and progression reactions directly, the combat layer would become brittle very quickly. Separating these concerns made the runtime pipeline much easier to read and extend.

In practice the flow is:
- combat intent and attack timing come from state + `CombatManager`
- hit detection becomes `HitData` in `HitBoxManager`
- damage resolution lands in combat/health layers
- stamina costs live in `StaminaManager`
- feedback reacts through event subscribers

That separation is why the project can route one hit into multiple outcomes without collapsing everything into one function.

The parts that still feel tightly coupled are the player-side runtime checks. Movement, stamina, combat flags, and hurt/death rules all need to agree for the character to feel responsive, so those systems still reference each other frequently. The cleanest boundaries are health/stamina broadcasting and the event-driven feedback layer. The weakest boundary is the player action layer, where responsiveness required a lot of direct coordination between managers.

## 7. Why use trigger-driven arena orchestration?

`GamePlayCoordinator` exists because encounter flow is a scene-level concern, not a boss-specific concern. The game uses predefined levels and curated encounters, so it made more sense to centralize arena activation, barriers, camera changes, and cleanup than to make every encounter reinvent that logic separately.

`ArenaSetUp` packages together the encounter-specific data that the coordinator needs, such as barriers, boss object, and encounter camera. That makes the trigger system reusable across multiple fights because the orchestration logic can stay mostly the same while the concrete arena data changes.

What this simplified:
- enter-trigger handling
- barrier toggling
- encounter camera priority changes
- boss activation
- fight cleanup and return to exploration
- broadcasting boss-fight lifecycle events to other systems

This also scales better across encounters because the shared lifecycle is centralized while the clear condition can remain encounter-specific. For example, different encounters can raise `RaiseExitBossFight(curArena)` when they are done, and `GamePlayCoordinator` still owns the common cleanup response.

If I redesigned this area, I would make the trigger/cleanup lifecycle more data-driven and less dependent on manually wired scene references. The current setup works for the size of this project, but it would need stronger tooling and validation if the game expanded.

## 8. What would I redesign if I rebuilt this project?

I would keep the core architectural direction:
- actor-owned FSMs
- separation of movement/combat/health/stamina responsibilities
- event-driven cross-system reactions
- coordinator-style encounter orchestration

I would simplify and strengthen three areas in particular.

First, I would improve reuse across boss systems. The current split between Dark Wolf and Evil Wizard works, but phase logic, vulnerability windows, cooldown-driven decisions, and attack orchestration could benefit from more shared patterns.

Second, I would add clearer interfaces and namespaces. The repo already has strong folder-based domain boundaries, but many systems still live in the global namespace and depend on direct references. More explicit interfaces would make high-risk gameplay rules easier to test and swap.

Third, I would add more data-driven configuration. ScriptableObjects would be a good fit for tunable gameplay values such as health, stamina costs, damage tables, encounter configuration, and maybe even some attack or phase parameters. Right now many of those rules are hard-coded or inspector-wired in ways that are fine for a solo project but less scalable.

I would also add automated tests where the rules are most brittle:
- stamina deduction and regeneration behavior
- health/death transitions
- state-transition guard logic
- encounter completion and cleanup conditions

The main architectural lesson from this project was that explicit structure matters most when gameplay complexity starts compounding. FSMs, separated managers, and event-driven reactions all helped keep the project understandable. At the same time, the project also taught me where “modular” is not the same as “loosely coupled,” and where the next step would be stronger interfaces, cleaner shared abstractions, and better test coverage.

### How did the architecture evolve over time?

This project began in February 2026 as a much simpler platformer prototype built around basic jumping and movement. Most of the real gameplay and architecture work happened during the summer, from June through August 2026, when the project expanded from a small prototype into a combat-focused action game.

The implementation order shaped the architecture. I started with the player first, because movement, combat flow, and state control defined the rest of the game. After that foundation was working, I built the bosses and encounter logic on top of it. Once the core gameplay loop was stable, I added the surrounding presentation and support layers: UI, combat feedback, animation polish, ambience, cameras, and background presentation systems.

That sequence is why the architecture feels layered. The early work established the actor and combat foundations first, while later work introduced the event-driven feedback, encounter orchestration, and presentation systems needed to make the game feel complete.
