# Engineering Decisions

This document records the major design decisions behind Knight of Cinders. It is intentionally written as a reflection space so I can explain not only what I built, but why I built it this way.

## 1. Why use finite state machines for actors?

Questions to answer:
- What problems did FSMs solve for player and enemy behavior?
- Why was this better than putting all behavior in one controller?
- What became easier after each actor owned explicit states?
- What were the downsides?

## 2. Why use manager components instead of one large controller?

Questions to answer:
- Which responsibilities were separated?
- How did this help debugging or iteration?
- Where did this approach still create coupling?
- Which manager boundaries worked best in practice?

## 3. Why use an EventManager?

Questions to answer:
- Which systems needed to react to the same gameplay event?
- How did events help separate combat, UI, feedback, audio, and progression?
- What risks did a global event bus introduce?
- Where did direct references still remain necessary?

## 4. Why create SkeletonCoordinator instead of making each skeleton independent?

Questions to answer:
- What encounter behavior required squad-level coordination?
- How did roles like Frontliner, Flanker, and Backuper improve the fight?
- What was hard about coordinating multiple enemies?
- What bugs or edge cases came from role reassignment?

## 5. Why split Evil Wizard into phase-specific controllers?

Questions to answer:
- What problems would one giant boss controller have created?
- How did phase-specific controllers make the boss easier to reason about?
- What tradeoffs did this introduce?
- If rebuilt today, would the split stay the same?

## 6. Why separate combat, health, stamina, and feedback?

Questions to answer:
- Why not resolve everything directly inside attack code?
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
