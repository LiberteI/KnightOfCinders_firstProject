# Knight of Cinders

A solo-built 2D Unity action game focused on melee combat, boss phases, and finite-state-machine-driven enemy behavior.

## Play / Watch
- 🎮 **Play Demo:** [Google Drive download](https://drive.google.com/file/d/1QigchoK-Ckn5wDokIMy8jG526GJFgCHP/view?usp=sharing)
- ▶️ **Watch Trailer:** [YouTube trailer](https://www.youtube.com/watch?v=DgtHopB85VI)
- 🎥 **Watch Full Gameplay:** [YouTube full gameplay](https://www.youtube.com/watch?v=TSEB1rc2AlE)

## Recruiter Quick Scan
| Area | Details |
|---|---|
| Team | Solo project |
| Duration | About 2 months |
| Estimated Effort | ~600 hours |
| Engine / Language | Unity / C# |
| Core Architecture | Component-based managers + actor-specific FSMs |
| Major Encounters | Trap Skeleton, Skeleton Army, Dark Wolf, Evil Wizard |
| FSM-Driven Actor Families | 5: player, tutorial skeleton, skeleton squad, Dark Wolf, Evil Wizard phase 1 / phase 2 |
| Key Systems | Combat, stamina, health, hit feedback, enemy AI, boss phases, arena flow, UI, ambience, cutscenes |
| Technical Docs | 6+ deep-dive Markdown files |
| System Scope | 10+ gameplay and presentation subsystems across combat, AI, UI, audio, scene flow, and cutscenes |

## Project Summary
Knight of Cinders is a 2D Souls-like action game where the player fights through a ruined kingdom to defeat an evil wizard and reclaim the realm. The project emphasizes deliberate melee combat, readable telegraphs, stamina management, and escalating encounter design.

The codebase is structured around actor-specific finite state machines, manager-based gameplay components, and an event-driven combat pipeline. The strongest implementation areas are combat flow, enemy behavior, boss phase logic, and encounter orchestration.

## Tech Stack
- Unity
- C#
- `MonoBehaviour` component architecture
- Cinemachine
- Unity Animator / animation-driven combat
- Unity UI
- Unity Audio
- Mermaid diagrams for technical documentation

## Controls
- `WASD` move
- `J` light attack
- `U` heavy attack
- `K` jump
- `L` roll
- `H` shield strike
- `Left Shift` sprint
- `Esc` quit

## Technical Highlights
- Component-based Unity gameplay architecture using focused `MonoBehaviour` managers
- Actor-specific finite state machines for player, enemies, and bosses
- Event-driven combat pipeline built around `HitData` and a shared `EventManager`
- Skeleton squad role system with `Frontliner`, `Flanker`, and `Backuper` behavior switching
- Multi-phase boss logic for Dark Wolf and Evil Wizard
- Stamina, health, hit stop, camera shake, and combat-audio feedback systems
- Arena lifecycle orchestration with camera lock-in, barriers, activation, and cleanup
- Scene-context switching, ambience control, and cutscene handoff

## Project Highlights
### Combat Responsiveness
<!-- TODO: Add GIF after recording -->
`docs/media/player-combat-combo-demo.gif`

Shows light/heavy combo flow, rolling, shield strike, stamina pressure, and animation-timed hit detection.

### Squad-Based Enemy AI
<!-- TODO: Add GIF after recording -->
`docs/media/skeleton-role-system-demo.gif`

Shows frontliners, flankers, and backupers creating coordinated pressure.

### Boss Phase Escalation
<!-- TODO: Add GIF after recording -->
`docs/media/dark-wolf-boss-phase-demo.gif`

Shows health-threshold phase change, charge pressure, and punish windows.

### Arena Lifecycle
<!-- TODO: Add GIF after recording -->
`docs/media/arena-lifecycle-demo.gif`

Shows trigger activation, camera lock-in, barriers, fight cleanup, and return to exploration.

## What This Demonstrates
- Gameplay programming in Unity with C#
- Object-oriented system decomposition across combat, movement, AI, UI, and presentation
- Finite-state-machine architecture for multiple actor families
- Event-driven runtime design
- Encounter and boss design implementation
- Debugging and manual playtesting discipline

## Key Implementation Challenges
- Building reusable FSM patterns across player, enemies, and multi-phase bosses
- Keeping melee combat readable while supporting combos, directional blocking, rolling, and stamina costs
- Coordinating multi-enemy tactical behavior in the skeleton encounter
- Managing boss fight lifecycle across triggers, cameras, barriers, ambience, and cleanup
- Connecting gameplay outcomes to cutscenes and scene handoff without tightly coupling systems

## Technical Deep Dive
- [Architecture](docs/ARCHITECTURE.md)
- [Combat System](docs/COMBAT_SYSTEM.md)
- [Enemy AI and Bosses](docs/ENEMY_AI_AND_BOSSES.md)
- [Scene Flow and Presentation](docs/SCENE_FLOW_AND_PRESENTATION.md)
- [Known Issues and Testing](docs/KNOWN_ISSUES_AND_TESTING.md)
- [Engineering Decisions](docs/ENGINEERING_DECISIONS.md)
- [GIF Capture Plan](docs/GIF_CAPTURE_PLAN.md)
- [Game Design](docs/GAME_DESIGN.md)

## Known Limitations
- The repository preserves the gameplay code and main scenes, but the original full Unity project context was not fully retained after an accidental deletion.
- Testing is manual only; there are no automated gameplay or regression tests.
- Some systems are modular by responsibility but still rely heavily on serialized references and direct component access.
- A few in-level camera transitions still need tuning, especially around dungeon/exploration handoff.
