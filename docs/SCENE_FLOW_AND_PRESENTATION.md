# Scene Flow and Presentation

## Overview
Scene flow and presentation are handled through a combination of scene loading, in-level context switching, Cinemachine camera priorities, ambience managers, UI updates, and cutscene controllers.

## Scene Flow
Primary flow:
- `MainMenu`
- `BeginningCutScene`
- `level1`
- victory / defeat cutscene return to `MainMenu`

Runtime region labels inside `level1`:
- `outside`
- `sewer`
- `dungeon`
- `finalRoom`
- `rainScene`

## Arena and Camera Lifecycle
`GamePlayCoordinator` manages:
- arena triggers
- boss activation
- barrier toggling
- camera priority changes
- fight cleanup
- scene-context labels

### Arena / Encounter Lifecycle
```mermaid
flowchart LR
    Explore[Exploration] --> Trigger[Trigger Volume]
    Trigger --> Lock[Arena Camera Priority Up]
    Lock --> Barriers[Barriers Enabled]
    Barriers --> Activate[Boss / Encounter Activated]
    Activate --> Fight[Encounter Runtime]
    Fight --> Clear[Encounter Clear Condition]
    Clear --> ExitEvent[RaiseExitBossFight]
    ExitEvent --> Cleanup[Barriers Off + Camera Reset]
    Cleanup --> Explore
```

![Arena camera lock demo](gifs/arena-camera-lock-demo.gif)

## Exploration Cameras
Exploration cameras are switched by region trigger membership:
- sewer range
- dungeon range
- final room range

This uses Cinemachine priority changes instead of hard teleports.

![Exploration camera transition demo](gifs/exploration-camera-transition-demo.gif)

![Rain scene trigger demo](gifs/rain-scene-trigger-demo.gif)

## Boss Cameras
Each arena owns its own encounter camera through `ArenaSetUp`.

At runtime:
- encounter trigger raises arena camera priority
- boss fight starts
- camera shake rebinds to the active arena camera
- cleanup lowers the arena camera priority

![Arena camera unlock demo](gifs/arena-camera-unlock-demo.gif)

## Ambience and Music
`AmbienceManager` reacts to scene-label changes and swaps:
- looped ambience
- one-shot environmental sounds

`BackGroundMusicManager` reacts to:
- enter boss fight
- exit boss fight
- victory
- defeat

![Lightning weather effect demo](gifs/lightning-weather-effect-demo.gif)

## UI
`UIManager` handles:
- player health bar
- enemy health bars
- stamina bar
- delayed bar animations

This presentation reacts to health and stamina events rather than polling combat directly.

![Intro UI presentation demo](gifs/intro-ui-presentation-demo.gif)

## Cutscene Flow
`CutSceneManager` handles:
- slide sequencing
- typewriter text
- continue prompts
- opening / victory / defeat routing

`CutSceneAwakener` activates winning or defeat cutscenes from gameplay events.

`GameObjectDisabler` disables tagged gameplay objects during cutscene handoff.

### Cutscene / Gameplay Handoff
```mermaid
flowchart TD
    Result[Win or Defeat Event] --> Awakener[CutSceneAwakener]
    Awakener --> EnableCS[Enable Cutscene GameObject]
    Result --> DisableGameplay[GameObjectDisabler]
    EnableCS --> CutSceneManager[CutSceneManager]
    CutSceneManager --> Slides[Slides + Typewriter + Ambience]
    Slides --> NextScene[Load Gameplay or Menu Scene]
```

![Victory cutscene handoff demo](gifs/victory-cutscene-handoff-demo.gif)

![Defeat cutscene handoff demo](gifs/defeat-cutscene-handoff-demo.gif)

## Employer-Relevant Takeaway
This subsystem demonstrates:
- scene-flow orchestration
- Cinemachine-based camera direction
- presentation systems tied cleanly to gameplay events
- UI and ambience integration without burying everything in combat code
