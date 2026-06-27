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
- boss machine overview
- user state machine overview (one example)
2. Skeleton-FSM + squad-role system
3. Arena / encounter progression coordinator
4. Combat event pipeline
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
