Architect overview
1. high-level architecture style:
This project uses a pragmatic Unity gameplay architecture built from `MonoBehaviour` components. Runtime actors such as the player, skeletons, Dark Wolf, and Evil Wizard are composed from focused managers for movement, combat, health, stamina, UI, audio, and feedback rather than a single monolithic controller. On top of that component model, the core gameplay loop is state-driven: each major actor owns an explicit finite state machine that controls behavior transitions such as idle, movement, attack, hurt, death, and boss-specific phases. Cross-system coordination is handled through a lightweight event-driven layer (`EventManager`) for health, stamina, hits, scene changes, and boss-fight lifecycle events, while scene-level flow is orchestrated by coordinator-style scripts such as `GamePlayCoordinator` and `SkeletonCoordinator`. In short, the codebase is best described as a component-based Unity architecture centered on explicit FSMs, event broadcasting, and manager-style orchestration.
2. Core architectural pattern: the FSM framework
- Base State class/interface (what methods does it expose — Enter(), Update(), Exit()?)
- How states are owned/switched (a StateMachine component per character, or one centralized manager?)
- Is it the same FSM base class reused for Player, Skeletons, Dark Wolf, and Evil Wizard, or separate implementations?
3. system boundaries / responsibilities
eg: PlayerController: (input -> state requests)
4. communication pattern between systems: 
- do systems talk via direct references, UnityEvents, a custom event bus, or polling? 
- E.g., does the Stamina system fire an event the UI listens to, or does the UI poll it every frame? Same question for combat hits → state machine.
5. Data flow diagram: a simple box-and-arrow diagram:
eg: Input → Player FSM → Stamina Check → Animation → Hitbox → Enemy FSM → Health/UI. 
6. Folder/namespace structure

Project folder structure


Key scripts/classes


Design pattern used
1. singleton
2. event-driven


System interations 
first-tier: core components
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
secondary: for a complete game
6. Scene switching, camera system
7. Cutscene triggering

Known Issues:
Context: this is the first project. I was not very proficient with git workflow. the comprehensive project was deleted by accident. The code and structure on github is the only remaining stuff.
1. only source code, scenes and level exist making it hard to reproduce local development environment. Luckily, a Mac executable survived.
2. no automated approach
3. this is not a fully decoupled architecture. Many systems still depend on serialized references and direct component access, so I would avoid overselling it as “clean architecture.”

Testing approach
1. manual playtesting only

credit:
I spent 2025 6-8 2 months to complete this game. coding 10 hours everyday. so ~600h effort for this game.
