# GAME DESIGN

## Project Overview

**Knight of Cinders** is a 2D Souls-like action game built in Unity. The player takes the role of a noble knight fighting through a ruined kingdom to defeat an evil wizard and reclaim the realm from a dark army.

The project is positioned around deliberate melee combat, stamina-driven decision making, readable enemy telegraphs, and escalating boss encounters. The intended experience is challenging but fair, with emphasis on timing, spacing, and player mastery.

## Core Design Pillars

### 1. Deliberate Combat
- Every offensive and defensive action has commitment, cost, and counterplay.
- Light attacks, heavy attacks, blocks, rolls, and shield strikes each fill a distinct tactical role.
- Combat feedback uses hit stop, camera shake, sound, and knockback to reinforce impact.

### 2. Resource Tension
- Stamina is the central combat resource.
- Strong actions and defensive mistakes create short-term vulnerability.
- Recovery pacing rewards restraint rather than button mashing.

### 3. Readable Challenge
- Enemies use recognizable attack patterns, role logic, and phase transitions.
- Boss escalation is designed to teach first, pressure second, and test mastery last.
- Telegraphs and recovery windows aim to keep difficulty fair and learnable.

### 4. Atmosphere and Presentation
- The game combines pixel-art level building, parallax backgrounds, weather effects, ambient storytelling, and cutscene support.
- Smooth camera behavior and polished UI support clarity during high-pressure combat.

## Player Combat Design

### Player Moveset
The knight is designed as a versatile melee character with seven core attack types:

- `Light1`, `Light2`, `Light3`
- `Heavy1`, `Heavy2`
- Run Attack
- Jump Attack

Additional defensive and utility actions include:

- **Defend / Block**: frontal damage negation when stamina is sufficient
- **Roll**: full invincibility for dodge and repositioning
- **Shield Strike**: low-damage spacing tool with pushback, stun, and super armor
- **Sprint**: movement option tied into stamina pressure

### Idle and Recovery States
- The player has two idle states.
- **Idle 1** is the standard standing state.
- **Idle 2** triggers after roughly two seconds of inactivity and transitions the knight into a seated pose.

These idle states are not cosmetic only. They support pacing and recovery:

- Stamina regenerates faster while idle than during active movement.
- In the seated idle state, both stamina and health recover more quickly.
- Fully draining stamina triggers a delayed regeneration penalty, encouraging measured play.

### Combo Structure
The combat system supports chained attacks with timing-based follow-through.

**Light combo flow**
- First input: upward swipe
- Second input: downward swipe
- Third input: stab finisher

**Heavy combo flow**
- First input: top-down slash
- Second input: horizontal slash

The system tracks combo count and timing windows so players can reliably chain attacks into finishers. Heavy and light attacks also use hit stop and camera shake to improve perceived weight and responsiveness.

### Defensive Rules and Combat Response
Combat response is state-sensitive to make player choices meaningful.

- **Light attacks**: interrupted on hit; player takes full damage and enters hurt state
- **Heavy attacks**: interrupted on hit; player takes full damage and enters hurt state
- **Block from front with enough stamina**: zero damage, slight knockback only
- **Block from front with low stamina**: partial damage and hurt state
- **Hit from behind while blocking**: full damage and hurt state
- **Roll**: full invincibility during active dodge window
- **Idle, run, jump, jump attack**: interrupted on hit and take full damage

This structure reinforces intentional risk. High-commitment attacks are rewarding, but only when used with timing and positioning discipline.

## Stamina System

### Function
Stamina is the global limiter across mobility, offense, and defense. It exists to pace combat, prevent degenerate repetition, and create windows where both the player and enemies can capitalize.

### Primary Costs
- Light Attack: roughly 3-5%
- Heavy Attack: roughly 5-8%
- Run: roughly 3-5%
- Blocked Attack: roughly 10%
- Roll: roughly 15%
- Shield Strike: roughly 5%

### Rules
- All stamina-consuming actions pull from one shared bar.
- Regeneration runs continuously during normal recovery.
- If stamina drops to zero, regeneration is delayed by about one second.
- Values are clamped between zero and maximum capacity.
- If stamina is insufficient, actions are cancelled or prevented cleanly.

This system is meant to create a Souls-like rhythm: commit, disengage, recover, and re-engage.

## Encounter and Boss Design

The game currently features four major boss-type encounters:

- Trap Skeleton
- Skeleton Army
- Dark Wolf
- Evil Wizard Phase 1 / Phase 2

Each encounter is built to teach or test a different layer of player mastery.

### Trap Skeleton
The Trap Skeleton functions as the tutorial boss and first combat gate.

**Purpose**
- Introduces boss presentation and arena lock-in
- Teaches basic melee flow, blocking, and dodging
- Establishes the game’s pacing expectations early

**Implementation-facing design**
- Triggered when the player enters a detection radius
- Opens with a unique "Bone Awakening" style intro animation
- Health bar presentation is synchronized with the intro
- Progression is blocked until the encounter is cleared

### Skeleton Army
The Skeleton Army is a multi-enemy encounter designed around coordinated pressure and tactical role switching. Around seven skeletons participate in the fight once the arena locks.

**Role structure**
- **Frontliners**: two melee pressure units that constantly pursue the player
- **Flanker**: one faster unit that stays at mid-range, probes for openings, and strikes opportunistically
- **Back-upers**: reserve units that hold formation, defend space, and replace fallen active roles

**Behavior goals**
- Frontliners maintain direct pressure and stop passive play
- The flanker forces awareness of facing and spacing, especially from behind
- Back-upers keep the encounter dynamic by substituting into active roles

**Threat shifting**
- Enemies re-evaluate roles on a timed interval
- Low-health frontliners can be replaced
- A dead flanker can be reassigned from the reserve group

This prevents the player from trivializing the fight by isolating one unit type and supports the feeling of a coordinated undead force.

### Dark Wolf
The Dark Wolf is an early boss centered on mobility, spacing, and phase escalation.

**Phase 1**
- Transparent strike and lunge behavior
- Teleport-assisted engagement if the player stays too far away
- Fast charge attack

**Phase 2**
- Triggered below 50% health
- Retains charge pressure
- Adds a more punishing body slam with stronger knockback and damage

**Design intent**
- Teaches players to react to sudden gap-closing tools
- Rewards disciplined dodging and stamina conservation
- Introduces the idea that boss behavior changes meaningfully as health drops

### Evil Wizard
The Evil Wizard is the final boss and the game’s capstone mastery check. The encounter is built across two major phases and a staged escalation structure.

#### Phase 1: Teaching Phase
- Basic run and idle patterning
- Upward and downward melee strikes
- Top-down laser when the player stays at range
- Multi-teleport finisher into a heavy strike with major damage and knockback

**Purpose**
- Trains recognition of telegraphs
- Punishes passive distancing
- Establishes the wizard’s ruleset without full overload

After defeat, the wizard enters a collapse state and transitions into a rebirth sequence for Phase 2.

#### Phase 2: Escalation and Mastery Test

**Stage 1: HP above 70%**
- Fast sickle pressure
- Homing lasers that track movement

**Stage 2: HP roughly 40-70%**
- Summons a Dark Wolf in its advanced form
- Gains partial invincibility
- Continues ranged pressure with reduced laser strength

**Stage 3: HP below 40%**
- Arena-filling laser wall barrage
- Telegraph before major casts for fairness
- Vulnerable recovery state after large attacks, with increased incoming damage
- Continued summon pressure when conditions allow

**Design intent**
- Stage 1 tests single-target survival under pressure
- Stage 2 adds battlefield complexity through summons and reduced damage windows
- Stage 3 becomes a spectacle survival test that rewards pattern recognition, patience, and punish timing

## Difficulty Curve and Learning Structure

The encounter order is structured around progressive mastery:

- **Trap Skeleton** teaches basic combat language
- **Dark Wolf** tests dodging, spacing, and adaptation to phase changes
- **Skeleton Army** tests crowd control, situational awareness, and stamina discipline
- **Evil Wizard** combines all prior lessons into a final layered boss fight

This sequence is intended to make the difficulty feel cumulative rather than arbitrary.

## Presentation and Technical Direction

### Presentation Features
- Parallax backgrounds
- Atmospheric weather effects using Unity particle systems
- Storytelling animations and ambient scene work
- Smooth camera behavior
- Interactive polished UI
- Combat feedback polish through hit effects and synchronized sound

### Technical Direction
- Unity with C# for gameplay systems
- Finite State Machine architecture for enemy AI and player state handling
- Sprite-based 2D animation pipeline
- Audio feedback integrated through Unity audio management
- Codebase structured around object-oriented design and SOLID principles

## Summary

**Knight of Cinders** is designed as a compact but polished 2D action game focused on readable challenge, layered boss design, and stamina-driven melee combat. Its player kit, encounter structure, and presentation systems are aligned around a single goal: delivering a fair but demanding combat experience that rewards mastery, patience, and mechanical understanding.
