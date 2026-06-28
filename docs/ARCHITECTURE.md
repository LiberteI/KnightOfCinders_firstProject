# Architecture

## Overview
Knight of Cinders uses a component-based Unity architecture with actor-specific finite state machines and event-driven cross-system communication.

Key runtime structure:
- Actor roots own state transitions
- Manager components own movement, combat, health, stamina, audio, UI, and feedback
- `EventManager` distributes combat, UI, and progression events
- Coordinators manage encounters, arena flow, and scene context

## High-Level System Architecture
```mermaid
flowchart TD
    Input[Player Input] --> PlayerFSM[Player FSM]
    PlayerFSM --> PlayerManagers[Movement / Combat / Stamina / Health]
    PlayerManagers --> Hitbox[HitBoxManager]
    Hitbox --> EventBus[EventManager]
    EventBus --> EnemyCombat[Enemy Combat Managers]
    EnemyCombat --> EnemyFSM[Enemy / Boss FSMs]
    EventBus --> Feedback[Combat Feedback]
    EventBus --> UI[UI Manager]
    EventBus --> Progression[GamePlayCoordinator]
    Progression --> Cameras[Cinemachine Cameras]
    Progression --> Arena[Arena Setup]
    EventBus --> Ambience[Ambience / BGM]
```

## Architecture Style
- Component-based Unity gameplay code using focused `MonoBehaviour` managers
- FSM-driven actor behavior for player, enemies, and bosses
- Event-driven publish/subscribe for combat, UI, scene, and lifecycle events
- Coordinator-style orchestration for encounters and progression

## FSM Framework
Each actor owns:
- an enum of valid states
- a state dictionary
- a `TransitionState(...)` method
- a current state reference

Player uses `PlayerStateInterface` with:
- `OnEnter()`
- `OnUpdate()`
- `OnExit()`
- `HandleInput()`
- `OnFixedUpdate()`
- `OnGetHit(HitData data)`

Enemies and bosses use `StateTransitionInterface` with:
- `OnEnter()`
- `OnUpdate()`
- `OnExit()`

### Player FSM Flow
```mermaid
flowchart TD
    Input[Input] --> Locomotion[Locomotion States]
    Input --> Combat[Combat States]
    Input --> Defense[Defense / Roll States]

    Locomotion --> Move[Idle / Walk / Run / Jump]
    Combat --> Attacks[Light Combo / Heavy Combo / Run Attack / Jump Attack]
    Defense --> Guard[Defend / Roll]

    Attacks --> Recovery[Recovery / Return to Movement]
    Guard --> Recovery
    Move --> Hurt[Hurt]
    Attacks --> Hurt
    Guard --> Hurt
    Hurt --> DeathCheck{Health <= 0?}
    DeathCheck -->|No| Recovery
    DeathCheck -->|Yes| Die[Die]
```

**GIF: Player Roll Dodge Demo**
![Player roll dodge demo](gifs/player-roll-dodge-demo.gif)

## Responsibility Boundaries
### Actor Roots
- `Knight`
- `NewSkeleton`
- `DarkWolf`
- `EvilWizard`
- `EvilWizardPhase2`

Responsibility:
- own state registration
- store current state
- perform transitions

### Player Runtime
- `KnightStates`: player decision layer
- `MovementManager`: locomotion and motion gating
- `CombatManager`: attacks, block rules, hit response
- `StaminaManager`: action costs and regeneration
- `HealthManager`: health, healing, death

### Enemy Runtime
- `EnemyCombatManager`
- `EnemyMovementManager`
- `EnemyHealthManager`

Specialized by enemy family:
- skeleton combat/roles
- wolf combat/phases
- wizard phase controllers

### Global / Cross-Cutting
- `EventManager`: event hub
- `HitBoxManager`: collision-to-hit payload translation
- `UIManager`: health/stamina presentation
- `CombatFeedbackManager`: hit stop and camera shake
- `GamePlayCoordinator`: arena lifecycle and scene context

## Data Flow
Key payloads:
- input commands
- state enums
- `HitData`
- `FeedbackData`
- health values
- stamina values
- damage values
- skeleton roles
- boss phase status
- `ArenaSetUp`
- scene labels
- camera priorities

## Combat Event Pipeline
```mermaid
flowchart LR
    Attack[Attack State / Animation] --> Collider[Hit Collider]
    Collider --> Hitbox[HitBoxManager]
    Hitbox --> HitData[Build HitData + FeedbackData]
    HitData --> EventBus[EventManager.OnHitOccured]
    EventBus --> Resolve[CombatManager / EnemyCombatManager]
    Resolve --> Health[HealthManager / EnemyHealthManager]
    Health --> UI[UIManager]
    EventBus --> Feedback[CombatFeedbackManager]
    EventBus --> Audio[AudioFeedbackManager]
```

**GIF: Combat Hit Feedback Demo**
![Combat hit feedback demo](gifs/combat-hit-feedback-demo.gif)

## Design Patterns
Formal patterns clearly present:
- Observer: `EventManager` publishes gameplay events, and multiple systems subscribe without direct coupling. Examples include `UIManager`, `CombatFeedbackManager`, `AudioFeedbackManager`, `AmbienceManager`, `BackGroundMusicManager`, and `GamePlayCoordinator` reacting to `OnHitOccured`, health, scene, and boss-fight events.
- Strategy-like behavior objects: actor behavior is split into interchangeable state objects behind shared interfaces. `Knight` stores `PlayerStateInterface` implementations in a state dictionary, while enemies and bosses do the same through `StateTransitionInterface` in classes like `NewSkeleton`, `DarkWolf`, `EvilWizard`, and `EvilWizardPhase2`.
- Facade-like manager layer: actor roots and states delegate subsystem work through narrower manager APIs instead of owning every rule directly. Typical calls include `movementManager`, `combatManager`, `healthManager`, and `staminaManager` operations from `KnightStates`, plus shared enemy movement/combat helpers used across skeletons, Dark Wolf, and Evil Wizard.
- Singleton-like global access through static classes: `EventManager` acts as a global event hub via static events and static raise methods, making it accessible across combat, UI, audio, cutscene, and progression systems without instance lookup.

## Employer-Relevant Takeaway
The architecture shows evidence of:
- runtime system decomposition
- behavior-oriented design with FSMs
- event-driven coordination
- gameplay-focused object-oriented programming
