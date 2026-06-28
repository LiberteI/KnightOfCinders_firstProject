# GIF Capture Plan

## Priority 1
- **Filename:** `player-combat-combo-demo.gif`
  **Record:** light combo chain, heavy combo, roll cancel/reposition, shield strike
  **System:** player combat system
  **Why it matters:** quickly shows gameplay responsiveness, combo logic, stamina-gated combat, and animation-driven action sequencing

- **Filename:** `combat-event-pipeline-demo.gif`
  **Record:** player lands hit, enemy takes damage, health bar updates, camera shake and hit stop are visible
  **System:** combat event pipeline
  **Why it matters:** demonstrates that gameplay, UI, and feedback are integrated rather than isolated

- **Filename:** `skeleton-role-system-demo.gif`
  **Record:** skeleton squad encounter showing frontliners pressuring while flanker repositions and attacks from another angle
  **System:** skeleton FSM + squad-role orchestration
  **Why it matters:** shows tactical enemy coordination, not just simple chase AI

- **Filename:** `dark-wolf-boss-phase-demo.gif`
  **Record:** phase 1 behavior, health threshold transition, and a phase 2 charge / punish window
  **System:** boss phase logic
  **Why it matters:** demonstrates multi-phase encounter implementation and readable escalation

## Priority 2
- **Filename:** `evil-wizard-phase-transition-demo.gif`
  **Record:** phase 1 death / rebirth transition into phase 2
  **System:** boss pipeline and scene presentation
  **Why it matters:** shows encounter scripting, multi-controller boss logic, and presentation handoff

- **Filename:** `evil-wizard-laser-wall-demo.gif`
  **Record:** phase 2 laser-wall attack followed by vulnerability window
  **System:** advanced boss attack sequencing
  **Why it matters:** shows staged boss design, cooldown-driven attacks, and punish windows

- **Filename:** `arena-lifecycle-demo.gif`
  **Record:** player enters encounter trigger, barriers activate, boss camera locks, fight clears, arena reopens
  **System:** arena / encounter progression coordinator
  **Why it matters:** demonstrates progression orchestration across cameras, barriers, activation, and cleanup

- **Filename:** `cutscene-handoff-demo.gif`
  **Record:** win or defeat event causing gameplay to stop and a cutscene object to activate
  **System:** cutscene triggering and gameplay handoff
  **Why it matters:** shows event-driven presentation flow beyond raw combat implementation

## Priority 3
- **Filename:** `scene-camera-transition-demo.gif`
  **Record:** player moving between outside, sewer, and dungeon regions with camera/ambience changes
  **System:** scene context and camera priority switching
  **Why it matters:** demonstrates scene-direction systems and spatial presentation control

- **Filename:** `ui-health-stamina-demo.gif`
  **Record:** stamina draining and regenerating, health bar damage response, white/red bar lag behavior
  **System:** UI presentation
  **Why it matters:** shows polish and event-driven HUD behavior

- **Filename:** `trap-skeleton-awakening-demo.gif`
  **Record:** hidden skeleton tutorial enemy awakening and entering combat
  **System:** encounter-specific activation flow
  **Why it matters:** demonstrates reuse of base enemy systems with custom presentation logic

## Recording Notes
- Keep each GIF short: 5 to 12 seconds
- Show one system per GIF whenever possible
- Capture HUD when it helps explain the system
- Prefer readable encounters over flashy but chaotic footage
- Use filenames consistently with the placeholder comments in the docs
