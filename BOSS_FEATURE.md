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
