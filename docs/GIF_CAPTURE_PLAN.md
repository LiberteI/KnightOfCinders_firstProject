# GIF Capture Plan

This plan assumes you already have one full master gameplay video and want to cut doc-ready GIFs from it rather than stage fresh captures.

## Goal
- Fill the GIF placeholders already referenced in the docs and README
- Prioritize clips that help a recruiter understand systems quickly
- Prefer clean, readable footage over exhaustive coverage

## Delivery Order

### Tier 1: README and strongest proof-of-work clips
These are the first exports to make because they support the top-level project pitch.

| Filename | Primary placement | Target length | What the clip must show | Reject if |
|---|---|---:|---|---|
| `player-combat-combo-demo.gif` | `README.md`, `docs/COMBAT_SYSTEM.md` | 6-9s | light combo, heavy hit, roll reposition, shield strike, visible stamina drain | player whiffs repeatedly, enemies crowd the screen, or HUD is missing |
| `skeleton-role-system-demo.gif` | `README.md`, `docs/ENEMY_AI_AND_BOSSES.md` | 7-10s | two skeletons pressure frontally while another repositions or attacks from the side | it looks like a generic mob pile with no readable role separation |
| `dark-wolf-boss-phase-demo.gif` | `README.md`, `docs/ENEMY_AI_AND_BOSSES.md` | 8-12s | phase 1 pressure, health threshold change, then a phase 2 punish window after charge pressure | the phase change is unclear or the punish window is not visible |
| `arena-lifecycle-demo.gif` | `README.md`, `docs/SCENE_FLOW_AND_PRESENTATION.md` | 8-12s | trigger entry, camera lock-in, barriers active, fight clear, arena opens again | barriers or camera changes are hard to notice |

### Tier 2: technical doc support clips
These deepen the architecture and boss docs after the README set is done.

| Filename | Primary placement | Target length | What the clip must show | Reject if |
|---|---|---:|---|---|
| `combat-event-pipeline-demo.gif` | `docs/ARCHITECTURE.md`, `docs/COMBAT_SYSTEM.md` | 5-8s | confirmed hit, enemy health response, visible hit stop or camera shake, UI reaction | impact feedback is too subtle to read at GIF speed |
| `evil-wizard-phase-transition-demo.gif` | `docs/ENEMY_AI_AND_BOSSES.md` | 7-11s | phase 1 defeat, collapse/rebirth handoff, clear entry into phase 2 | the cut happens before the rebirth read lands |
| `evil-wizard-laser-wall-demo.gif` | `docs/ENEMY_AI_AND_BOSSES.md` | 6-10s | laser-wall pattern and the boss vulnerability window after it | only the attack is shown and not the recovery/punish state |
| `cutscene-handoff-demo.gif` | `docs/SCENE_FLOW_AND_PRESENTATION.md` | 6-10s | victory or defeat result, gameplay disabled, cutscene object or slide sequence begins | transition feels like a hard edit instead of an in-game handoff |

### Tier 3: nice-to-have support clips
Useful if the master video contains clean footage, but not worth forcing from weak material.

| Filename | Primary placement | Target length | What the clip must show | Reject if |
|---|---|---:|---|---|
| `trap-skeleton-awakening-demo.gif` | `docs/ENEMY_AI_AND_BOSSES.md` | 5-8s | dormant skeleton wakes, intro animation, then enters combat with health bar active | awakening and combat start are separated by a dead pause |
| `player-fsm-state-demo.gif` | `docs/ARCHITECTURE.md` | 6-8s | movement into attack, roll/guard, then hurt or recovery state in one readable sequence | state changes are too subtle to communicate visually |
| `hit-feedback-demo.gif` | `docs/COMBAT_SYSTEM.md` | 4-6s | one especially weighty hit that clearly sells hit stop and impact | effect is invisible without slow motion |

## Extraction Strategy From The Master Video

### 1. Cut the README set first
- Start with the clips that support `README.md`.
- If a sequence can also serve a doc placeholder, reuse the same exported GIF.
- Do not spend time perfecting low-priority footage before the README set is complete.

### 2. Trim to one idea per GIF
- Each GIF should answer one question fast:
  - combat feel
  - squad coordination
  - boss escalation
  - arena orchestration
- If a clip tries to show two systems equally, split it or drop one.

### 3. Cut on action, not on setup
- Enter the GIF as late as possible while preserving context.
- Remove travel time, idle waiting, menu time, and failed attempts.
- End shortly after the system payoff is visible.

### 4. Favor clean readability over mechanical completeness
- A shorter clip with one obvious system beat is better than a longer “full fight” excerpt.
- For boss clips, show the transition or punish window, not a general exchange.
- For AI clips, show separation of roles, not raw enemy count.

## Recommended Shot Selection Rules

### Combat clips
- Keep the HUD visible when stamina or health response matters.
- Prefer 1v1 or low-chaos moments so hits and recovery are readable.
- Avoid clips where the player is stuck in corners or taking messy damage with no clear takeaway.

### Enemy AI clips
- Show enemy spacing and role contrast in frame at the same time.
- Avoid zoomed or cramped footage where flanking behavior looks accidental.

### Boss clips
- Include the telegraph, the attack, and the recovery or phase payoff.
- Do not use footage where the player dies before the boss behavior is explained.

### Arena / presentation clips
- Keep enough lead-in for the camera or barrier change to register.
- If the trigger is subtle, start slightly earlier than you would for combat clips.

## Export Guidelines
- Format: GIF
- Length: usually 5-10 seconds; hard cap 12 seconds
- Crop: keep gameplay area readable; do not crop out relevant HUD for combat/UI clips
- Loopability: prefer sequences that reset cleanly or end on a stable frame
- Playback: optimize for normal-speed readability first; only use slight speed-up if the source is sluggish

## Placeholder Map
- `README.md`
  - `player-combat-combo-demo.gif`
  - `skeleton-role-system-demo.gif`
  - `dark-wolf-boss-phase-demo.gif`
  - `arena-lifecycle-demo.gif`
- `docs/COMBAT_SYSTEM.md`
  - `player-combat-combo-demo.gif`
  - `hit-feedback-demo.gif`
- `docs/ARCHITECTURE.md`
  - `player-fsm-state-demo.gif`
  - `combat-event-pipeline-demo.gif`
- `docs/ENEMY_AI_AND_BOSSES.md`
  - `skeleton-role-system-demo.gif`
  - `trap-skeleton-awakening-demo.gif`
  - `dark-wolf-boss-phase-demo.gif`
  - `evil-wizard-phase-transition-demo.gif`
  - `evil-wizard-laser-wall-demo.gif`
- `docs/SCENE_FLOW_AND_PRESENTATION.md`
  - `arena-lifecycle-demo.gif`
  - `cutscene-handoff-demo.gif`

## Practical Editing Order
1. Scrub the master video once and note timecodes only for Tier 1 candidates.
2. Export the four README GIFs.
3. Scrub again for the best boss-transition and combat-feedback moments.
4. Export Tier 2 only where the footage is already strong.
5. Treat Tier 3 as optional polish, not required coverage.

## Final Quality Check
- Can someone understand the system with the GIF muted?
- Is the key gameplay beat visible within the first 2 seconds?
- Does the clip support the exact claim made in the doc section where it appears?
- Would a recruiter learn something concrete from this, or is it just “gameplay exists” footage?
