# 💀 Skeleton Boss Fights

## 1. Trap Skeleton (Tutorial Encounter)
- **Cinematic Introduction**  
  - Skeleton rises from a pile of wrecked bones with a unique “Bone Awakening” animation.  
  - Health bar fades in as the animation completes.  
  - Designed to teach the player basic combat flow and boss fight pacing.  

- **Mechanics**  
  - Triggered when the player enters a detection radius (`TrapDetector`).  
  - Player cannot proceed until the skeleton is defeated.  
  - Transitioned back into Idle state once animation finishes.  

- **Design Purpose**  
  - Functions as a **first boss / tutorial gate**.  
  - Teaches combat basics (light attacks, blocking, dodging).  
  - Introduces health bar + arena lock-in mechanic.  

---

# 🪓 Skeleton Army Encounter

## 📖 Scenario
A coordinated group of ~7 skeletons blocks the player’s path once the camera locks into the arena.  
Skeletons cooperate using different **combat roles** (frontliner, flanker, back-uper) and **dynamic threat switching** to create a varied and adaptive fight.  

---

## 👥 Roles & Behaviors

### **Frontliners (2 units)**
- Constantly pursue the player at normal speed.  
- Act as **primary pressure units** to keep player engaged in melee.  

---

### **Flanker (1 unit)**
- **Faster but weaker** skeleton with lower HP and higher damage.  
- Behavior:  
  - Maintains **mid-range distance** (sneaky state).  
  - Evaluates positioning every **0.5s**:  
    - Too close → retreats.  
    - Too far → advances.  
    - Mid-range → idles.  
  - Retreats instantly if hit.  
- **Attack Behavior** (every 3–5 seconds):  
  - If in front → dashes in with light attack, then retreats.  
  - If behind → dashes in with heavy attack, then retreats (unless player turns around).  
- Always transitions back to **sneaky state** after attacking or getting hit.  

---

### **Back-upers (4 units initially)**
- Wait behind the frontline, blocking path.  
- Play **defensive animations** and counterattack if player approaches them directly.  
- **Dynamic Substitution**:  
  - Replace fallen frontliners or flanker to keep pressure constant.  

---

## 🔄 Threat Level Shifting (Agro Switching)
- Every **2 seconds**, skeletons re-evaluate their roles:  
  - If a frontliner’s HP < 10% → a back-uper joins as a new frontliner.  
  - If the original flanker dies → a back-uper becomes the new flanker.  
- Purpose: keeps combat **dynamic and adaptive**, prevents player from cheesing or isolating enemies.  

---

## 🎯 Design Goals
- Create **tactical variety** within a single fight.  
- Force players to manage both **constant melee pressure** (frontliners) and **opportunistic strikes** (flanker).  
- Add tension with **role substitution**, making the battle feel alive and reactive.

# 🐺 Dark Wolf Boss

## Overview
The **Dark Wolf** is a small but aggressive boss that introduces players to reactive combat.  
It has **two phases** based on its health threshold, evolving from calculated strikes to a berserk frenzy.  

---

## Phase 1 (Healthy)
- **Transparent Strike**  
  - Wolf briefly turns transparent and lunges at the player.  
  - If the player is too far, the wolf will **teleport** close before attacking.  
- **Charge Attack**  
  - Wolf charges straight toward the player with high speed.  

---

## Phase 2 (Berserk – HP < 50%)
- **Charge Attack** *(same as Phase 1)*  
- **Body Slam**  
  - Wolf performs a violent charge, slamming its body into the player.  
  - Higher knockback and damage than Phase 1 attacks.  

---

## Design Notes
- **Phase Transition**: At 50% HP, the wolf becomes visibly more aggressive.  
- **Player Learning Curve**:  
  - Phase 1 teaches dodging & spacing (teleports, charges).  
  - Phase 2 tests stamina & defensive discipline with heavier knockback.  
- **Purpose**: Acts as an **early-game boss**, teaching players how enemies can escalate in intensity.  


## 🧙 Evil Wizard – Final Boss

The **Evil Wizard** is the climactic battle, fought across two major phases with escalating complexity.  

---

### Phase 1 – Teaching Phase
- **Abilities:**  
  - Run / Idle  
  - Upward and downward melee strikes  
  - Top-down laser if player is far  
  - **Stutter Teleport Finisher** → multiple teleports, then a heavy strike for 30%+ damage and knockback  
- **Design Purpose:**  
  - Trains players to dodge, find safe attack windows, and recognize telegraphs.  
  - Hints at future laser danger without overwhelming.  
- **Transition:**  
  - After death, Wizard collapses (DeathMode1).  
  - A soul-like aura rises, signaling rebirth into Phase 2.  

---

### Phase 2 – Power Spike (Bringer of Death)

#### 🟢 Stage 1 (HP > 70%)
- **Melee Sickle Attacks** → fast attack with high pressure. Enchanted if cannot reach the player within time.
- **Homing Lasers** → magical bolts that track player’s movement.  

#### 🟡 Stage 2 (HP 40–70%)
- **Summon Dark Wolf** → brings one Dark Wolf (Mode 2 form) into the arena.  
- **Partial Invincibility** → Wizard becomes 20% transparent and cannot be damaged.
- **Nerfed Homing Lasers** → magical bolts that track player’s movement.

#### 🔴 Stage 3 (HP < 40%)
- **Laser Wall Barrage** → sweeping walls of lasers fill the arena.  
  - Always preceded by a telegraphing animation for fairness.  
  - After casting, Wizard enters a **vulnerable state** where incoming damage is doubled.  
- **Summons** may continue if wolf is not active.  

**Design Purpose:**  
- Escalates from single-target pressure (Stage 1) → multi-enemy tension (Stage 2) → spectacle survival (Stage 3).  
- Vulnerability windows reward patience and punish reckless play.  
- Final test of the player’s mastery of dodging, stamina, and positioning.  

---

# 🎯 Boss Design Philosophy
- **Dark Wolf** → Introduces dodging, spacing, and escalation mechanics.  
- **Skeleton Army** → Tests crowd control, stamina management, and awareness of enemy roles.  
- **Evil Wizard** → Culmination of all skills, teaching → escalating → mastery test.  
