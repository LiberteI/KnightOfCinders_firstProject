# Engineering Decisions

This document records the major design decisions behind Knight of Cinders. It is intentionally written as a reflection space so I can explain not only what I built, but why I built it this way.

## 1. Why use finite state machines for actors?

Questions to answer:
- What problems did FSMs solve for player and enemy behavior?
1. It provides a more reusable, more extenable system for new actions. it also provides explicite control for behaviors like animations, audio synchronization, and feedback. 
- Why was this better than putting all behavior in one controller?
1. Before FSM, There was a script called User controller. It produced weird bugs like animation blending.
- What became easier after each actor owned explicit states?
1. all animations and states own their own segragated state, preventing mixture.
- What were the downsides?
1. when there are more and more states as player's actions become more complex, we might need to introduce new architectures like child states, parent states to group similar states. So for example: idle-move or related states can integrate into a big block and expose a centred state to transition to states built for other concerns like attack etc.

## 2. Why use manager components instead of one large controller?

Questions to answer:
- Which responsibilities were separated?
1. UI logic, Health, combat, movement, etc
- How did this help debugging or iteration?
1. this decouple different layers of logic. easier to dubug
- Where did this approach still create coupling?
1. referencing.
- Which manager boundaries worked best in practice?
1. might be health manager and movement manager

## 3. Why use an EventManager?

Questions to answer:
- Which systems needed to react to the same gameplay event?
1. UI, health, audio
- How did events help separate combat, UI, feedback, audio, and progression?
1. event sends boardcast and other layers subscribe to it. This creates decoupling.
- What risks did a global event bus introduce?
1. bug is hard to trace because of event driven design.
- Where did direct references still remain necessary?
1. Unity Inspector for game obj.

## 4. Why create SkeletonCoordinator instead of making each skeleton independent?

Questions to answer:
- What encounter behavior required squad-level coordination?
1. algro switch, role assignment. dynamic attack patterns.
- How did roles like Frontliner, Flanker, and Backuper improve the fight?
1. making the roles more dynamic
- What was hard about coordinating multiple enemies?
1. live status was hard to visualise. I did this mainly by logging
- What bugs or edge cases came from role reassignment?
1. cannot think of any

## 5. Why split Evil Wizard into phase-specific controllers?

Questions to answer:
- What problems would one giant boss controller have created?
1. state coupling. Hard to reuse the old FSM architecture. creating a new object was easier to implement and cleaner because 2 phases' animation are drastically different.
- How did phase-specific controllers make the boss easier to reason about?
1. explicitly control aniamtions etc. used polymorphism instead of if else blocks.
- What tradeoffs did this introduce?
1. more self standing objects rather than reuse resources.
- If rebuilt today, would the split stay the same?
1. I will make sure some resources are reusable.

## 6. Why separate combat, health, stamina, and feedback?

Questions to answer:
- Why not resolve everything directly inside attack code?
1. violate 
- How did this make the combat pipeline easier to extend?
- What parts still felt tightly coupled?
- Which boundaries were cleanest and which were the weakest?

## 7. Why use trigger-driven arena orchestration?

Questions to answer:
- What did `GamePlayCoordinator` simplify?
- Why package encounter data into `ArenaSetUp`?
- What made this easier to scale across multiple encounters?
- What would you redesign about the trigger / cleanup lifecycle?

## 8. What would I redesign if I rebuilt this project?

Questions to answer:
- Which systems would you keep?
- Which systems would you simplify?
- Where would you add interfaces, namespaces, tests, or ScriptableObjects?
- What did this project teach you about software architecture?
