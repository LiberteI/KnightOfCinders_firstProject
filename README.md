## Architect Overview

### 1. High-Level Architecture Style
This project uses a pragmatic Unity gameplay architecture built from `MonoBehaviour` components. Runtime actors such as the player, skeletons, Dark Wolf, and Evil Wizard are composed from focused managers for movement, combat, health, stamina, UI, audio, and feedback rather than a single monolithic controller. On top of that component model, the core gameplay loop is state-driven: each major actor owns an explicit finite state machine that controls behavior transitions such as idle, movement, attack, hurt, death, and boss-specific phases. Cross-system coordination is handled through a lightweight event-driven layer (`EventManager`) for health, stamina, hits, scene changes, and boss-fight lifecycle events, while scene-level flow is orchestrated by coordinator-style scripts such as `GamePlayCoordinator` and `SkeletonCoordinator`. In short, the codebase is best described as a component-based Unity architecture centered on explicit FSMs, event broadcasting, and manager-style orchestration.

### 2. Core Architectural Pattern: The FSM Framework
The main behavioral framework in the project is a hand-written finite state machine pattern. Each major actor owns its own state registry as a `Dictionary<Enum, State>` and exposes a `TransitionState(...)` method that calls `OnExit()` on the current state, swaps the state object, and then calls `OnEnter()` on the new one. This ownership model is decentralized: there is no single global state machine manager. Instead, the player, each enemy family, and each boss controller keep their own local FSM and update it in their own `Update()` loop.

The project uses two closely related state interfaces rather than one universal base class. The enemy side uses `StateTransitionInterface`, which exposes `OnEnter()`, `OnUpdate()`, and `OnExit()`. The player side uses a richer `PlayerStateInterface`, which adds `HandleInput()`, `OnFixedUpdate()`, and `OnGetHit(HitData data)` on top of the standard lifecycle methods. That difference reflects the fact that the player FSM owns direct input handling and physics-timing responsibilities, while enemy FSMs are driven mostly by AI logic and manager conditions.

The pattern is reused consistently across actor families, but each family has its own enum and concrete state set. The player FSM covers locomotion, defense, combos, jump, roll, hurt, death, and invulnerability. Skeletons use a simpler combat-state model with role-sensitive states such as `Defend`, `Idle`, `Walk`, `Attack`, `Sneak`, `Hurt`, `Die`, and `Trap`. Dark Wolf extends the pattern with boss-specific decision and phase logic such as `Decide`, `Berserk`, `Charge`, and `Vulnerable`. Evil Wizard is split into two separate FSM controllers: one for phase 1 and another for phase 2, each with its own state enum and attack loop. So architecturally, this is not one shared polymorphic FSM framework in the strictest sense; it is a repeated FSM pattern with lightweight shared conventions and actor-specific implementations.

### 3. System Boundaries / Responsibilities
The codebase separates responsibilities reasonably well at the gameplay-feature level. The boundaries are not enforced by formal layers or dependency inversion, but most runtime concerns are still grouped into dedicated components with clear intent.

- **Actor root controllers (`Knight`, `NewSkeleton`, `DarkWolf`, `EvilWizard`, `EvilWizardPhase2`)**
  Own state registration, track the current state, and perform state transitions. These scripts act as the local "brains" for each actor type rather than containing all gameplay logic themselves.

- **Player state layer (`KnightStates`)**
  Converts player input and runtime conditions into state transitions such as idle, walk, run, attack, defend, roll, jump, hurt, and death. This is the main decision layer for player behavior.

- **Player movement (`MovementManager`)**
  Owns locomotion rules and low-level movement execution: horizontal movement, running, jumping, rolling, swimming, facing direction, and movement gating conditions. It does not decide high-level intent; it executes movement once the state machine allows it.

- **Player combat (`CombatManager`)**
  Owns player-side attack execution, combo timing, attack flags, runtime damage values, hit reaction routing, and death transition requests. It acts as the combat rule layer for the knight.

- **Player stamina (`StaminaManager`)**
  Owns stamina costs, regeneration, depletion delay, clamping, and stamina broadcasts to other systems such as UI. It is a distinct resource system rather than being folded into combat or movement.

- **Player health (`HealthManager`)**
  Owns health storage, damage intake, passive regeneration, healing delay rules, death detection, and health-change broadcasts. Health is kept separate from the combat manager, which is a good responsibility boundary.

- **Enemy base capability layer (`EnemyCombatManager`, `EnemyMovementManager`, `EnemyHealthManager`)**
  Provides shared enemy-side primitives such as melee range checks, hit handling hooks, movement helpers, and enemy health lifecycle. This creates a reusable base layer for different enemy families even though each family extends it differently.

- **Skeleton encounter combat logic (`NSCombatManager`)**
  Owns skeleton-specific combat behavior, flanker spacing logic, role-sensitive attack selection, hurt handling, temporary invulnerability windows, and role-driven runtime behavior. This is a strong boundary because squad logic is not hardcoded into the generic enemy base classes.

- **Skeleton squad orchestration (`SkeletonCoordinator`)**
  Owns multi-agent encounter setup: spawning skeletons, assigning frontliner/flanker/backuper roles, monitoring attrition, and promoting replacements over time. This is one of the clearest higher-level orchestration boundaries in the repo.

- **Dark Wolf boss logic (`DWCombatManager`, `DWMovementManager`, `DarkWolfStates`)**
  Owns wolf-specific combat rules, mode switching, charge logic, berserk transition, vulnerability windows, and boss cleanup behavior. Boss-specific rules are kept outside the generic enemy layer.

- **Evil Wizard phase logic (`EW1CombatManager`, `EW1MovementManager`, `EW2CombatManager`, `EW2MovementManager`, wizard states)**
  Owns phase-specific attacks, cooldowns, summoning, lasers, vulnerability windows, phase escalation, and final-boss runtime orchestration. Splitting phase 1 and phase 2 into separate controllers is a clear responsibility choice that keeps the final boss from collapsing into one oversized class.

- **Hit detection and damage dispatch (`HitBoxManager`)**
  Converts overlap events into `HitData`, computes knockback direction, attaches feedback metadata, and raises the global hit event. This is the bridge between animation/hitboxes and downstream combat or health systems.

- **Global event routing (`EventManager`)**
  Broadcasts cross-cutting gameplay events such as health changes, stamina changes, hit events, enemy death, boss fight entry/exit, scene changes, victory, and defeat. This is the project’s main decoupling mechanism between combat, UI, feedback, and progression systems.

- **Combat feedback (`CombatFeedbackManager`, `HitStopManager`, `CameraShakeManager`, `AudioFeedbackManager`)**
  Owns game-feel response to combat events, including hit stop, camera shake, and some impact/audio response. This is a good separation because feedback is triggered by combat but not embedded directly inside every combat script.

- **UI presentation (`UIManager`, `BarFollow`)**
  Owns health/stamina bar rendering, delayed bar animations, target-tracking UI, and cleanup of health bars after death. The UI mostly reacts to events rather than computing combat results itself.

- **Encounter and scene progression (`GamePlayCoordinator`)**
  Owns arena activation, barrier toggling, boss activation, camera priority changes, fight cleanup, and scene-environment detection. This script acts as a scene-level coordinator rather than an actor controller.

- **Audio and ambience (`AmbienceManager`, `BackGroundMusicManager`, footstep/sound managers, boss sound managers)**
  Owns environment audio, background music, footstep playback, and actor-specific sound effects. Audio is distributed across several targeted managers instead of one monolithic sound script.

- **Cutscene flow (`CutSceneManager`, `CutSceneAmbienceManager`, `CutSceneAwakener`, `GameObjectDisabler`)**
  Owns slide sequencing, typewriter text flow, ambience timing, and cutscene-triggered activation/deactivation. This is a distinct presentation-flow subsystem, separate from gameplay combat flow.

- **Menu and front-end presentation (`MainMenu`, `MainMenuBGM`, `MenuParallax`)**
  Own menu behavior, front-end ambience, and menu visual motion. These are separated from gameplay systems cleanly.

- **Visual environment systems (`BackgroundManager`, `LighteningSystem`)**
  Own non-combat scene presentation such as environmental motion and weather/light effects. These are presentation-side helpers rather than gameplay controllers.

At a higher level, the repo shows a real attempt at separating gameplay domains into movement, combat, resources, AI, UI, feedback, audio, and encounter coordination. The boundary quality is strongest at the feature/component level. The main limitation is that these systems still depend heavily on serialized references, shared runtime objects, and direct component access, so the architecture is modular in practice but not deeply abstracted.

### 4. Data Flow Diagram
Before drawing the box-and-arrow diagram, it helps to identify the main kinds of data that move through the game:

- **Player input data**
  Keyboard input such as movement, sprint, jump, attack, defend, roll, and quit commands. This data is read primarily by the player state layer and movement systems.

- **State transition data**
  Enum-based state identifiers such as `KnightStateTypes`, `NSStateType`, `DarkWolfStateType`, `EvilWizardStateTypes`, and `Phase2StatesTypes`. These values drive actor behavior changes inside the FSM controllers.

- **Combat hit payloads**
  `HitData` is the core combat-transfer object. It carries the attack initiator, target hurt box, runtime damage, knockback direction, and attached feedback data.

- **Combat feedback payloads**
  `FeedbackData` carries hit stop and camera shake parameters such as stop time, amplitude, frequency, and duration. This is attached to hit events and consumed by feedback systems.

- **Health values**
  Current health, maximum health, dead/alive state, and damage/healing updates for both player and enemies. These values are broadcast to UI and used by combat and death logic.

- **Stamina values**
  Current stamina, maximum stamina, depletion state, regeneration timing, and stamina-cost deductions for actions such as running, heavy attacks, shield strike, blocking, and rolling.

- **Damage values**
  Runtime attack damage, damage multipliers, boss vulnerability bonuses, attack-specific damage constants, and knockback decisions. These values are computed by combat managers and passed into hit resolution.

- **Enemy role data**
  Skeleton squad role assignments such as `Frontliner`, `Flanker`, and `Backuper`. This data affects movement style, attack selection, and promotion/replacement logic in the squad encounter.

- **Boss phase and status data**
  Health thresholds, current mode, vulnerability flags, cooldown timers, summon state, and penalty windows for Dark Wolf and Evil Wizard encounters.

- **Encounter setup data**
  `ArenaSetUp` carries the current barriers, boss object, and arena camera for each encounter. It is used by progression and boss-fight lifecycle logic.

- **Scene and environment data**
  Scene labels such as `outside`, `sewer`, `dungeon`, `finalRoom`, and `rainScene`, plus trigger tags and environmental toggles like rain/bounds. These values drive ambience and camera behavior.

- **Camera priority data**
  Cinemachine camera priority values that determine which exploration or boss camera is active at a given time.

- **Event payloads**
  Global event-bus messages for health changes, stamina changes, hit events, enemy death, boss-fight entry/exit, scene changes, victory, and defeat.

- **Animation and timing data**
  Animation state names, normalized time checks, coroutine timers, cooldowns, and invulnerability windows. These values drive sequencing and state exit conditions.

- **UI presentation data**
  Health-bar ratios, stamina-bar ratios, tracked entities, and delayed bar animation targets used by the HUD and enemy health bars.

#### Data-Specific Flow Sketches

- **Player input data**  
  `Keyboard Input -> KnightStates.HandleInput() -> Knight.TransitionState(...) -> MovementManager / CombatManager -> Animator / Rigidbody2D`

- **State transition data**  
  `Runtime Condition / Input / AI Decision -> State Enum Value -> Actor TransitionState(...) -> Current State Swap -> OnEnter() / OnUpdate() / OnExit()`

- **Combat hit payloads (`HitData`)**  
  `Attack Animation / Active Hitbox -> HitBoxManager.OnTriggerEnter2D() -> Build HitData -> EventManager.RaiseHitOccured(data) -> Combat / Health / Feedback listeners`

- **Combat feedback payloads (`FeedbackData`)**  
  `CombatManager / EnemyCombatManager.sourceFeedback -> HitBoxManager.AssignFeedbackData() -> HitData.feedbackData -> CombatFeedbackManager / AudioFeedbackManager`

- **Health values**  
  `Incoming Damage / Passive Heal -> HealthManager or EnemyHealthManager -> curHealth / maxHealth update -> EventManager health broadcast -> UIManager / death logic / state transitions`

- **Stamina values**  
  `Action Request / Regen Tick -> StaminaManager.DeductStamina() or RegenerateStamina() -> curStamina update -> EventManager.KnightStaminaChanged() -> UIManager`

- **Damage values**  
  `Current Attack State / Boss Vulnerability / Multiplier -> CombatManager or EnemyCombatManager.damage -> HitData.damage -> Target HealthManager.TakeDamage()`

- **Enemy role data**  
  `SkeletonCoordinator.AssignRole() / TrackCurSkeletonSquat() -> currentRole (Frontliner / Flanker / Backuper) -> NSStates / NSCombatManager behavior branch -> movement and attack style`

- **Boss phase and status data**  
  `Boss Health / Cooldowns / Vulnerability Flags -> DWCombatManager or EW1/EW2CombatManager -> boss FSM TransitionState(...) -> phase-specific attacks / penalties / summons`

- **Encounter setup data (`ArenaSetUp`)**  
  `Trigger Collider -> GamePlayCoordinator.TriggerArena(...) -> ArenaSetUp (camera + barriers + boss) -> RaiseEnterBossFight() -> fight lifecycle / cleanup`

- **Scene and environment data**  
  `Scene Trigger / OverlapCircle() -> GamePlayCoordinator.DetermineScene() -> currentScene string -> EventManager.RaiseSceneChanged() -> AmbienceManager / environment toggles`

- **Camera priority data**  
  `Arena Trigger / Region Trigger -> GamePlayCoordinator -> CinemachineCamera.Priority update -> active camera changes for exploration or boss fight`

- **Event payloads**  
  `System State Change -> EventManager.Raise...() -> subscribed listeners (UI / combat / ambience / cutscene / coordinator) -> secondary reactions`

- **Animation and timing data**  
  `State Enter / Coroutine Start -> Animator.Play() / timer values / cooldown counters -> normalizedTime / timer checks -> next transition or effect window`

- **UI presentation data**  
  `Health / Stamina event -> UIManager.UpdateHealthBar() / UpdateStaminaBar() -> bar ratios and delayed animation -> player HUD / enemy health bars`


### 5. Folder / Namespace Structure
The project is organized primarily by gameplay domain and Unity feature area rather than by formal C# namespaces. Most scripts live in the global namespace and are grouped physically by folder. In practice, the folder structure carries the architectural meaning that namespaces would often carry in a larger C# codebase.

#### Project Folder Structure
```text
KnightOfCinders_firstProject
├── Scenes
│   ├── BeginningCutScene.unity
│   ├── MainMenu.unity
│   └── level1.unity                # Main gameplay scene
├── doc
│   ├── Archive_Docs
│   └── GAME_DESIGN.md
└── script
    ├── Player
    │   ├── Knight.cs               # Player actor root and FSM owner
    │   ├── KnightStates.cs         # Concrete player state implementations and transitions
    │   ├── MovementManager.cs      # Player locomotion execution and movement gating
    │   ├── CombatManager.cs        # Player attacks, combos, damage, and hit reactions
    │   ├── StaminaManager.cs       # Stamina costs, regeneration, depletion, UI broadcasts
    │   ├── HealthManager.cs        # Player health, healing, death detection, event broadcasts
    │   ├── PlayerStateInterface.cs # Player state contract
    │   └── BarFollow.cs            # World-space UI tracking helper
    ├── EnemyAI
    │   ├── EnemyCombatManager.cs   # Shared enemy combat base behavior
    │   ├── EnemyMovementManager.cs # Shared enemy movement helpers
    │   ├── EnemyHealthManager.cs   # Shared enemy health and death lifecycle
    │   ├── StateTransitionInterface.cs # Enemy state contract
    │   ├── Skeleton_New
    │   │   ├── NewSkeleton.cs         # Skeleton actor root and FSM owner
    │   │   ├── NSStates.cs            # Skeleton states: defend, sneak, attack, hurt, trap
    │   │   ├── NSCombatManager.cs     # Skeleton-specific combat and flanker logic
    │   │   ├── SkeletonCoordinator.cs # Squad orchestration, spawning, role replacement
    │   │   └── TrapDetector.cs        # Tutorial/trap encounter trigger
    │   ├── DarkWolf
    │   │   ├── DarkWolf.cs        # Dark Wolf boss root and FSM owner
    │   │   ├── DarkWolfStates.cs  # Dark Wolf state graph implementation
    │   │   ├── DWCombatManager.cs # Wolf-specific combat, berserk, charge, vulnerability
    │   │   └── DWMovementManager.cs # Wolf movement specialization
    │   └── EvilWizard
    │       ├── EvilWizard.cs         # Evil Wizard phase 1 root and FSM owner
    │       ├── EvilWizardPhase2.cs   # Evil Wizard phase 2 root and FSM owner
    │       ├── States.cs             # Concrete states for both wizard phases
    │       ├── EW1CombatManager.cs   # Phase 1 combat logic and attacks
    │       ├── EW1MovementManager.cs # Phase 1 movement behavior
    │       ├── EW2CombatManager.cs   # Phase 2 combat, cooldowns, summons, escalation
    │       └── EW2MovementManager.cs # Phase 2 movement behavior
    ├── CombatFeedback
    │   ├── CombatFeedbackManager.cs  # Applies hit stop and camera shake from combat events
    │   ├── HitStopManager.cs         # Time-stop feedback controller
    │   ├── CameraShakeManager.cs     # Camera shake feedback controller
    │   ├── AudioFeedbackManager.cs   # Hit/audio reaction feedback
    │   └── EnemySuperArmourManager.cs # Enemy super-armour helper
    ├── AmbientAudio
    │   ├── AmbienceManager.cs         # Scene ambience switching and playback
    │   ├── BackGroundMusicManager.cs  # Background music control
    │   ├── PlayerSoundManager.cs      # Player-specific sound effects
    │   ├── SkeletonSoundManager.cs    # Skeleton-specific sound effects
    │   ├── WizardSoundManager.cs      # Wizard phase 1 sound effects
    │   └── WolfSoundManager.cs        # Wolf-specific sound effects
    ├── CutScene
    │   ├── CutSceneManager.cs         # Slide sequencing, typewriter text, scene handoff
    │   ├── CutSceneAmbienceManager.cs # Cutscene ambience control
    │   ├── CutSceneAwakener.cs        # Win/lose cutscene activation
    │   └── GameObjectDisabler.cs      # Cutscene object toggling helper
    ├── Menu
    │   ├── MainMenu.cs    # Main menu runtime behavior
    │   ├── MainMenuBGM.cs # Menu background music
    │   └── MenuParallax.cs # Menu parallax presentation
    ├── BeautyStuff
    │   ├── BackgroundManager.cs # Background presentation helper
    │   └── LighteningSystem.cs  # Weather / lightning presentation helper
    ├── EventManager.cs          # Global event bus for combat, UI, progression, lifecycle
    ├── GamePlayCoordinator.cs   # Arena activation, barriers, camera switching, cleanup
    ├── HitBoxManager.cs         # Collision-to-hit translation and HitData dispatch
    └── UIManager.cs             # Health/stamina UI presentation and bar animation
```

There is no strong namespace hierarchy in the code at the moment. If this project were expanded further, namespaces would be a natural next step to formalize the same domain boundaries that are already present in the folder structure.

## Design Pattern Used
Using the formal object-oriented pattern vocabulary, the repo most clearly shows the following:

- **Observer**
  This is the clearest formal pattern in the codebase. `EventManager` acts as a subject/event hub, while systems like `UIManager`, `CombatFeedbackManager`, `CombatManager`, `AmbienceManager`, and cutscene helpers subscribe to events such as health changes, stamina changes, hits, scene changes, and victory/defeat.

- **Strategy-like behavior objects**
  The FSM state objects function close to a Strategy pattern. Each state encapsulates a different behavior implementation for the same actor, and the actor swaps between them at runtime through `TransitionState(...)`. In stricter terminology this is also closely related to the State pattern, but since your list does not include `State`, `Strategy` is the nearest named match.

- **Singleton**
  The project uses static/global access through `EventManager` and `StaminaCostTable`, but that is not the same as implementing classic instance-managed singletons with controlled lifecycle and a single access point object.

## System Interactions

### First-Tier: Core Components
1. state machines
The state machine layer is the main runtime decision system in the project. Each major actor owns its own local FSM, stores state objects in a dictionary keyed by an enum, and swaps behavior through a `TransitionState(...)` method. The player uses `PlayerStateInterface`, which includes input handling and fixed-update hooks, while enemies and bosses use the lighter `StateTransitionInterface` with `OnEnter()`, `OnUpdate()`, and `OnExit()`.

The pattern is reused across all major actor families:
- `Knight` manages the player FSM
- `NewSkeleton` manages the skeleton FSM
- `DarkWolf` manages the wolf boss FSM
- `EvilWizard` manages phase 1 of the final boss
- `EvilWizardPhase2` manages phase 2 of the final boss

This gives the codebase a consistent interaction model: high-level intent is decided in state objects, while movement, combat, health, stamina, and animation are delegated to specialized managers.

**Player state machine overview**
- Primary states include `Idle`, `Walk`, `Run`, `Jump`, `Defend`, `Roll`, `Attack1-3`, `HeavyAttack1-2`, `RunAttack`, `JumpAttack`, `Hurt`, `Die`, and `Invulnerable`.
- `KnightStates` reads input and runtime conditions, then decides whether to request movement, defend, spend stamina for an action, or transition to another combat state.
- The player FSM is tightly integrated with `MovementManager`, `CombatManager`, `StaminaManager`, and `HealthManager`.

Typical player interaction flow:
`Input -> KnightStates.HandleInput() -> stamina / movement checks -> Knight.TransitionState(...) -> CombatManager or MovementManager execution -> animation -> hitbox / recovery -> return to Idle / Walk / Run`

**Boss state machine overview**
- Dark Wolf uses a compact but expressive boss FSM with `Idle`, `Walk`, `Run`, `Attack`, `Decide`, `Berserk`, `Charge`, `Vulnerable`, `Hurt`, and `Die`.
- Evil Wizard phase 1 uses a teaching-phase FSM with `IdleMode1`, `Run`, `AttackMode1_1`, `AttackMode1_2`, `Decide`, `Vulnerable`, `Hurt`, and `DeathMode1`.
- Evil Wizard phase 2 uses a larger escalation FSM with `Start`, `Phase2Decide`, `Walk`, `AttackWithEffect`, `AttackWithoutEffect`, `HomingLaser`, `SummonWolf`, `LaserWall`, `Phase2Vulnerable`, `Hurt`, and `Phase2Death`.

Typical boss interaction flow:
`distance / health threshold / cooldown / vulnerability state -> boss combat manager or decide state -> boss TransitionState(...) -> movement or attack coroutine -> player hit / recovery / penalty window -> next decide state`

Architecturally, the important point is that the FSMs do not try to do everything themselves. They serve as orchestration layers that choose behavior, while the actual execution is handed off to combat, movement, health, audio, and feedback managers.

2. Skeleton-FSM + squad-role system
The skeleton encounter adds a second layer of interaction on top of the normal enemy FSM. Each skeleton is individually state-driven, but the group is also coordinated by a separate squad controller that assigns and reassigns combat roles over time. This is one of the strongest examples in the repo of combining local actor logic with encounter-level orchestration.

**Per-skeleton FSM**
- Each skeleton owns a local FSM through `NewSkeleton`.
- Core states are `Defend`, `Idle`, `Walk`, `Attack`, `Sneak`, `Hurt`, `Die`, and `Trap`.
- `Trap` is used for the tutorial-style hidden skeleton setup before the enemy fully activates.
- `Defend` acts as the holding state for reserve units.
- `Sneak` is the flanker-only state that maintains spacing and probes for attack timing.

Typical per-unit interaction flow:
`currentRole -> NSStates.OnUpdate() -> movement / distance / range check -> TransitionState(...) -> attack or defend behavior -> hurt / death / recovery`

**Role system**
- The squad uses three explicit roles: `Frontliner`, `Flanker`, and `Backuper`.
- `Frontliners` are the direct pressure units. They move toward the player and cycle through normal attack states.
- `Flanker` uses modified stats and a different behavior loop: it maintains mid-range spacing, attacks periodically, and prefers role-specific attack selection based on whether it is in front of or behind the player.
- `Backuper` stays in a defensive holding pattern, blocks space, and only attacks opportunistically until promoted into an active combat role.

**Role-driven behavior branching**
- The same skeleton FSM behaves differently depending on `currentRole`.
- In `NSDefendState`, role determines whether the unit remains defensive, transitions to `Idle` as a frontliner, or enters `Sneak` as a flanker.
- In `NSAttackState`, role determines both attack selection and post-attack transition:
  - `Backuper -> Attack -> Defend`
  - `Frontliner -> Attack -> Idle`
  - `Flanker -> Attack -> Sneak`
- `NSCombatManager` also changes runtime damage based on role, making the flanker more threatening than a normal skeleton.

**Squad coordination**
- `SkeletonCoordinator` spawns the group, assigns initial targets and health-bar tracking, and then promotes specific members into the first active combat roles.
- The initial setup is:
  - 2 frontliners
  - 1 flanker
  - the rest as backupers
- The flanker is further modified at runtime with higher speed, lower health, and higher damage to create a distinct tactical role.

**Agro switch / replacement logic**
- The squad coordinator periodically re-evaluates the active group.
- If a frontliner dies or falls below a low-health threshold, a backup unit is promoted into the frontliner role.
- If the flanker dies, a backup unit is promoted into the flanker role.
- This prevents the encounter from collapsing after one or two kills and keeps pressure patterns changing during the fight.

Typical squad-level interaction flow:
`Spawn skeletons -> assign all as Backuper -> promote 2 Frontliners + 1 Flanker -> per-unit FSM behavior -> periodic health/death checks -> promote replacement units -> continue until all defeated`

**Tutorial / trap interaction**
- The tutorial skeleton uses the same underlying skeleton actor but begins in the `Trap` state.
- `TrapDetector` monitors the player’s proximity, plays the awakening animation, then transitions the skeleton into its active state and enables the health bar.
- This reuses the skeleton FSM while wrapping it in encounter-specific presentation logic.

Architecturally, this subsystem is valuable because it is not just “enemy AI.” It shows a layered interaction model:
- individual behavior is owned by the skeleton FSM
- role-specific tactical behavior is owned by `NSCombatManager`
- group composition and replacement logic is owned by `SkeletonCoordinator`

That separation makes the encounter more dynamic than a standard single-enemy state machine while still keeping the logic understandable.
3. Arena / encounter progression coordinator
The arena / progression layer is responsible for turning exploration into structured encounters. Instead of each boss or encounter independently controlling cameras, barriers, and fight lifecycle, `GamePlayCoordinator` acts as the scene-level orchestrator that activates encounters, tracks which arenas have already been triggered, and restores the scene after a fight ends.

**Core responsibility**
- Detect the player entering key trigger volumes
- Lock the camera to the active arena
- Enable temporary arena barriers
- Activate the encounter’s boss or enemy group
- Track whether the player is currently inside a boss fight
- Listen for encounter completion and clean up the arena
- Detect broader scene regions for ambience and camera transitions

**Shared arena data model**
- Each major encounter is packaged into an `ArenaSetUp` object.
- `ArenaSetUp` contains:
  - left barrier
  - right barrier
  - arena camera
  - current boss / encounter root object
- This creates a reusable setup structure for the training skeleton, Dark Wolf, skeleton squad, and Evil Wizard encounters.

**Trigger-to-encounter lifecycle**
- `GamePlayCoordinator.OnTriggerEnter2D()` listens for tagged trigger volumes such as:
  - `DungeonTrigger`
  - `WolfTrigger`
  - `SSTrigger`
  - `FinalBossTrigger`
- Each trigger routes into `TriggerArena(...)`.
- `TriggerArena(...)` checks:
  - whether the arena config exists
  - whether the encounter has already been triggered
- If valid, it:
  - sets the encounter’s one-shot flag
  - raises the arena camera priority
  - enables left and right barriers
  - activates the boss or encounter root object
  - broadcasts `EventManager.RaiseEnterBossFight(currentArena)`
  - stores `curArena` as the active encounter context

Typical activation flow:
`Player enters trigger -> GamePlayCoordinator.TriggerArena(...) -> camera lock + barriers on + boss active -> RaiseEnterBossFight(curArena) -> fight systems react`

**Event-driven fight lifecycle**
- `GamePlayCoordinator` subscribes to:
  - `OnEnterBossFight`
  - `OnExitBossFight`
- On enter, it marks `isInBossFight = true`.
- On exit, it:
  - disables the current barriers
  - lowers the arena camera priority
  - marks `isInBossFight = false`

This means the coordinator owns the arena shell, while the encounter scripts only need to signal when the fight is over.

**Encounter-local completion reporting**
The actual “fight finished” condition is not centralized in `GamePlayCoordinator`. Instead, each encounter-specific system determines when it is cleared and then raises `EventManager.RaiseExitBossFight(gpCoordinator.curArena)`.

Examples:
- `TrapDetector` raises exit when the tutorial skeleton dies
- `DWCombatManager` raises exit when Dark Wolf is cleared
- `SkeletonCoordinator` raises exit when all skeletons are defeated
- the final boss chain uses its own death/win flow and progression signaling

This split is useful architecturally:
- encounter scripts own combat completion conditions
- `GamePlayCoordinator` owns the shared response to encounter completion

Typical completion flow:
`Encounter-specific clear condition -> RaiseExitBossFight(curArena) -> GamePlayCoordinator.CleanUpArena(...) -> barriers off + camera reset + exploration resumes`

**Scene-region coordination**
The same coordinator also handles non-boss scene flow:
- region triggers such as `SewerRange`, `DungeonRange`, and `FinalRoom` raise or lower camera priorities
- `DetermineScene()` uses overlap checks and scene labels such as `outside`, `sewer`, `dungeon`, `finalRoom`, and `rainScene`
- when the scene label changes, it broadcasts `RaiseSceneChanged(curScene)` so ambience systems can react

Typical exploration flow:
`Player moves through region -> GamePlayCoordinator updates camera priority / currentScene -> EventManager.RaiseSceneChanged(...) -> ambience and presentation systems update`

**Why this matters architecturally**
- The coordinator prevents boss scripts from duplicating camera and barrier logic
- It gives all encounters the same lifecycle shape: trigger, lock-in, activate, clear, clean up
- It separates exploration flow from combat resolution
- It provides a single integration point between triggers, cameras, barriers, ambience, and encounter activation

This subsystem is a good example of practical orchestration code in a game project. It is not a generic framework, but it does establish a consistent progression pipeline across multiple encounters.
4. Combat event pipeline
The combat event pipeline is the project’s central runtime interaction path for attacks landing, damage being resolved, and audiovisual feedback reacting to the result. It is one of the clearest examples of how the codebase combines direct component access with event-driven fan-out.

**Core idea**
- A hitbox collision does not directly update every downstream system.
- Instead, collision is translated into a structured combat payload (`HitData`), then broadcast through `EventManager`.
- Combat logic, UI, audio, and feedback systems subscribe to that event and react from their own responsibility boundaries.

**Hit payload structure**
- `HitData` is the main combat-transfer object.
- It carries:
  - `initiator`
  - `targetHurtBox`
  - `damage`
  - `knockBackDir`
  - `feedbackData`
- `FeedbackData` is embedded inside `HitData` and carries hit stop and camera shake parameters.

This means one collision event can transport both gameplay consequences and feel/presentation data in the same packet.

**Pipeline entry point: hit detection**
- `HitBoxManager.OnTriggerEnter2D()` is the main entry point.
- It distinguishes between:
  - enemy hitboxes colliding with player hurtboxes
  - player hitboxes colliding with enemy hurtboxes
- It then:
  - finds the runtime damage source
  - builds a `HitData` object
  - calculates knockback direction from relative positions
  - copies combat feedback settings into `feedbackData`
  - validates whether the hit should occur
  - broadcasts `EventManager.RaiseHitOccured(data)`

Typical entry flow:
`Attack animation -> active collider -> HitBoxManager -> build HitData -> RaiseHitOccured(data)`

**Primary gameplay listeners**
- `CombatManager` listens for hit events affecting the player.
- Enemy combat managers such as `NSCombatManager`, `DWCombatManager`, `EW1CombatManager`, and `EW2CombatManager` listen for hit events affecting their own hurt boxes.
- These listeners decide:
  - whether the hit is valid for this actor
  - whether the actor is invincible or shielded
  - whether defending changes the result
  - whether to take damage, transition to hurt, or ignore the hit

For the player specifically:
- `CombatManager.GetHit(...)` checks invulnerability and defend state
- block direction and stamina can reduce or negate damage
- failed defense can halve damage and force a hurt transition
- accepted hits transition the knight into `Hurt` and call `OnGetHit(data)` on the current state

For enemies specifically:
- each enemy combat manager filters by its own hurt box
- enemy-specific rules then apply:
  - skeleton super-armor / hurt-count logic
  - wolf vulnerability and charge-state logic
  - wizard casting, heavy attack, summon, and phase rules

**Damage resolution**
- Once a hit is accepted, the target’s health manager receives the damage value:
  - player side through `HealthManager.TakeDamage(...)`
  - enemy side through `EnemyHealthManager.TakeDamage(...)`
- Health changes are then broadcast again through `EventManager` so presentation and death logic can react independently.

Typical damage resolution flow:
`HitData accepted -> target combat manager resolves block / invulnerability / modifiers -> HealthManager.TakeDamage(...) -> health event broadcast`

**Feedback fan-out**
- `CombatFeedbackManager` listens to `OnHitOccured` and applies:
  - hit stop
  - camera shake
- `AudioFeedbackManager` also listens to the same event and chooses impact audio based on:
  - who initiated the hit
  - which hurt box was struck
  - special runtime states such as wolf charging or wizard attack mode

This is a strong interaction design point: the same combat event drives both gameplay and game-feel without requiring the attacker or defender scripts to manually call every feedback system.

**UI fan-out**
- The hit event itself does not update the UI directly.
- Instead, damage changes health.
- Health managers then broadcast updated values.
- `UIManager` subscribes to those health and stamina events and updates:
  - player health bar
  - enemy health bars
  - stamina bar
  - death cleanup for tracked health bars

This gives the combat pipeline a layered structure:
`collision -> hit event -> combat resolution -> health/stamina event -> UI update`

**Why this pipeline matters architecturally**
- It separates collision detection from combat resolution
- It lets multiple systems react to one combat event without tight direct coupling
- It centralizes hit payload construction in one place
- It keeps feedback and UI as subscribers rather than embedding them into every attack coroutine
- It still allows actor-specific combat rules through per-enemy combat managers

**Typical end-to-end combat pipeline**
`Attack State / Animation -> HitBoxManager -> HitData -> EventManager.OnHitOccured -> CombatManager / EnemyCombatManager -> HealthManager / EnemyHealthManager -> health events -> UIManager + death logic`

**Typical end-to-end feedback pipeline**
`HitData.feedbackData -> CombatFeedbackManager -> HitStopManager + CameraShakeManager`

**Typical end-to-end audio pipeline**
`HitData -> AudioFeedbackManager -> choose clip by initiator / target / attack context -> play impact sound`
5. combat system
- health
- stamina
- audio
- animation

### Secondary: For a Complete Game
6. Scene switching, camera system
7. Cutscene triggering

## Known Issues
Context: this is the first project. I was not very proficient with git workflow. the comprehensive project was deleted by accident. The code and structure on github is the only remaining stuff.

1. only source code, scenes and level exist making it hard to reproduce local development environment. Luckily, a Mac executable survived.
2. no automated approach
3. this is not a fully decoupled architecture. Many systems still depend on serialized references and direct component access, so I would avoid overselling it as “clean architecture.”

## Testing Approach
1. manual playtesting only

## Credit
I spent 2025 6-8 2 months to complete this game. coding 10 hours everyday. so ~600h effort for this game.
