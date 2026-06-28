# Enemy AI and Bosses

## Overview
Enemy behavior is built from reusable enemy base managers plus actor-specific finite state machines and combat logic. The strongest implementation areas are the skeleton squad-role encounter and the two boss pipelines: Dark Wolf and Evil Wizard.

## Enemy Runtime Structure
- Shared base layer:
  - `EnemyCombatManager`
  - `EnemyMovementManager`
  - `EnemyHealthManager`
- Specialized families:
  - skeletons
  - Dark Wolf
  - Evil Wizard phase 1 / phase 2

## Skeleton Squad Role System
Each skeleton owns a local FSM, but the encounter is coordinated at squad level.

Skeleton states:
- `Defend`
- `Idle`
- `Walk`
- `Attack`
- `Sneak`
- `Hurt`
- `Die`
- `Trap`

Roles:
- `Frontliner`
- `Flanker`
- `Backuper`

### Skeleton Role System Diagram
```mermaid
flowchart TD
    Spawn[Spawn Skeleton Group] --> Backuper[All Start as Backuper]
    Backuper --> FrontlinerA[Promote Frontliner 1]
    Backuper --> FrontlinerB[Promote Frontliner 2]
    Backuper --> Flanker[Promote Flanker]
    FrontlinerA --> Fight[Direct Pressure]
    FrontlinerB --> Fight
    Flanker --> Sneak[Sneak / Spacing / Probe Attack]
    Fight --> LowHP{Low HP or Death?}
    Sneak --> DeadFlanker{Flanker Dead?}
    LowHP -->|Yes| ReplaceFrontliner[Promote Backuper]
    DeadFlanker -->|Yes| ReplaceFlanker[Promote Backuper]
    ReplaceFrontliner --> Fight
    ReplaceFlanker --> Sneak
```

![Skeleton role separation demo](gifs/skeleton-role-separation-demo.gif)

### Runtime Summary
- Frontliners push direct melee pressure
- Flanker manages spacing and timing from a different attack loop
- Backupers hold defensive positions until promoted
- `SkeletonCoordinator` reassigns roles as units weaken or die

![Skeleton role replacement demo](gifs/skeleton-role-replacement-demo.gif)

## Trap Skeleton
The tutorial skeleton uses the same actor framework but starts in `Trap` state and is activated by `TrapDetector`.

Runtime flow:
- player enters detector radius
- awakening animation plays
- skeleton transitions into active state
- health bar is enabled

![Trap skeleton awakening demo](gifs/trap-skeleton-awakening-demo.gif)

## Dark Wolf
Dark Wolf is an early boss centered on mobility, charge pressure, and a clear phase escalation.

Dark Wolf states:
- `Idle`
- `Walk`
- `Run`
- `Attack`
- `Decide`
- `Berserk`
- `Charge`
- `Vulnerable`
- `Hurt`
- `Die`

### Dark Wolf Boss Flow
```mermaid
flowchart TD
    Idle --> Decide
    Decide --> Walk
    Decide --> Run
    Decide --> Charge
    Walk --> Attack
    Run --> Attack
    Charge --> Decide
    Charge --> Vulnerable
    Vulnerable --> Decide
    Attack --> Decide
    Decide --> Berserk
    Berserk --> Decide
    Hurt --> Decide
    Decide --> Die
```

Dark Wolf-specific features:
- health-threshold phase change
- charge windup and collision payoff
- vulnerable punish window after failed charge outcomes

![Dark Wolf phase transition demo](gifs/dark-wolf-phase-transition-demo.gif)

![Dark Wolf punish window demo](gifs/dark-wolf-punish-window-demo.gif)

## Evil Wizard
The final boss is split into two separate controllers.

### Phase 1
Focus:
- teaching telegraphs
- spacing pressure
- melee pattern recognition
- teleport finisher

States:
- `IdleMode1`
- `Run`
- `AttackMode1_1`
- `AttackMode1_2`
- `Decide`
- `Vulnerable`
- `Hurt`
- `DeathMode1`

### Phase 2
Focus:
- escalation
- summon pressure
- homing lasers
- laser-wall survival
- vulnerability windows

States:
- `Start`
- `Phase2Decide`
- `Walk`
- `AttackWithEffect`
- `AttackWithoutEffect`
- `HomingLaser`
- `SummonWolf`
- `LaserWall`
- `Phase2Vulnerable`
- `Hurt`
- `Phase2Death`

### Evil Wizard Phase Logic
```mermaid
flowchart TD
    P1Idle[Phase 1 Idle] --> P1Decide
    P1Decide --> P1Run
    P1Decide --> P1AttackA
    P1Decide --> P1AttackB
    P1AttackB --> P1Vulnerable
    P1Vulnerable --> P1Decide
    P1Run --> P1AttackA
    P1AttackA --> P1Decide
    P1Decide --> P1Death
    P1Death --> P2Start
    P2Start --> P2Decide
    P2Decide --> P2Walk
    P2Decide --> HomingLaser
    P2Decide --> SummonWolf
    P2Decide --> LaserWall
    P2Walk --> P2Attack
    P2Attack --> P2Decide
    LaserWall --> P2Vulnerable
    P2Vulnerable --> P2Decide
    SummonWolf --> P2Decide
    HomingLaser --> P2Decide
```

![Evil Wizard phase transition demo](gifs/evil-wizard-phase-transition-demo.gif)

![Evil Wizard homing laser demo](gifs/evil-wizard-homing-laser-demo.gif)

![Evil Wizard wolf summon demo](gifs/evil-wizard-wolf-summon-demo.gif)

![Evil Wizard laser wall demo](gifs/evil-wizard-laser-wall-demo.gif)

## Employer-Relevant Takeaway
This subsystem demonstrates:
- enemy AI specialization on top of shared bases
- encounter-level orchestration
- boss-phase design and implementation
- state-driven behavior with tactical variation
