# ⚔️ Player Combat System

## 💤 Idle System

- **Two Idle States**  
  - **Idle 1** → Standard standing pose.  
  - **Idle 2** → Triggered if player remains inactive for **2+ seconds**; knight sits down.  

- **Recovery Mechanics**  
  - While idling, **stamina regenerates faster** than in active states.  
  - In **Idle 2 (sitting state)**, both **stamina and health regenerate more quickly**, rewarding players who pause between fights.  

- **Stamina Drain Penalty**  
  - If stamina is fully drained, there is a **delay before regeneration begins**, forcing players to manage resources strategically.  

## 🪓 Attacks
- **7 attack types implemented**  
  - Light Attacks: `Light1`, `Light2`, `Light3` (combo chain)  
  - Heavy Attacks: `Heavy1`, `Heavy2`  
  - Run Attack  
  - Jump Attack  

- **Shield Strike**  
  - Low damage, designed for spacing  
  - **Pushes enemies back** and applies stun  
  - Grants **super armor** during execution  

- **Defend (Blocking)**  
  - Grants **frontal invincibility**  
  - Negates damage, applies slight knockback  
  - Vulnerable from behind  
  - Block breaks if stamina < required threshold  

- **Rolling**  
  - Grants **full invincibility**  
  - Used for dodging and repositioning  

- **Jump & Run Attacks**  
  - Stronger than light attacks  
  - Effective for **initiating fights** or **surprise strikes**  

---

## 🎮 Combo System
- **Light Attack Flow**  
  - J → Swipe Up Attack  
  - J → Swipe Down Attack  
  - J → Stab Finisher  

- **Heavy Attack Flow**  
  - U → Top-Down Slash  
  - U → Horizontal Slash  

- **Combo Counter & Timer**  
  - Tracks consecutive light attacks  
  - Successful chaining unlocks **combo finishers**  
  - Timer extended to allow smooth chaining  

- **Special Feedback**  
  - **Heavy** and **Light** → include **hit stop** + **camera shake** for extra impact  

---

## 🛡️ Combat Response Rules
1. **Light Attacks**  
   - Interrupted when hit  
   - Full damage taken → Hurt state  

2. **Heavy Attacks**  
   - Interrupted when hit  
   - Full damage taken → Hurt state  

3. **Defending**  
   - Front + enough stamina → 0 damage, knockback only  
   - Front + low stamina → 50% damage, Hurt state  
   - Back → full damage, Hurt state  

4. **Rolling**  
   - Full invincibility  

5. **Idle / Run / Jump / Jump Attack**  
   - If hit → interrupted, full damage, Hurt state  
---
# 💨 Stamina System

## 🔋 Stamina Costs
- **Light Attack** → 3–5% per use  
- **Heavy Attack** → 5–8% per use  
- **Run** → 3–5% per use  
- **Blocked Attack** → 10% per occurrence  
- **Roll (Dodge)** → 15% per use  
- **Shield Strike** → 5% per use  

---

## ⚙️ Stamina Protocols
1. **Global Stamina Bar**  
   - All stamina-related actions are tied to a single global bar.  

2. **Regeneration**  
   - Stamina regenerates continuously every frame.  

3. **Consumption**  
   - Running, heavy attacks, blocking while under attack, and Shield Strike reduce stamina.  

4. **Depletion Penalty**  
   - If stamina drops below 0, it is reset to 0.  
   - Regeneration resumes only after a **1-second delay** (punishment for overuse).  

5. **Clamping**  
   - Stamina values are always capped between **0 and Max**.  

6. **Insufficient Stamina Handling**  
   - If stamina is too low to perform an action, the system **gracefully cancels or prevents the move**.  
