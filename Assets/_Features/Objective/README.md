# Objective System

The objective system is a modular, event-driven framework designed to guide players through a sequence of tasks (e.g., a tutorial). It uses **ScriptableObjects** for data definition and a **Context-based** approach to link scene-specific objects with global systems.

---

## Core Components

### 1. `ObjectiveManager`
The central brain of the system.
- **Responsibility**: Manages the lifecycle of objectives (Activation, Completion, Advancement).
- **Workflow**: 
    - Holds an ordered list of `Objective` objects.
    - Listens for the `OnTutorialObjectiveRequest` event via the `EventBus` to begin.
    - Tracks the current active objective and ensures only one is active at a time.
    - When an objective is completed, it automatically advances to the next one in the list.

### 2. `Objective` & `ObjectiveData`
- **`Objective`**: A serializable class used in the Inspector to pair an `ObjectiveData` asset with its current `ObjectiveState` (Inactive, Active, Complete, etc.).
- **`ObjectiveData` (ScriptableObject)**: Defines the static information for an objective:
    - **Display Info**: Title and description text for the UI.
    - **HighlightableContext**: Reference to a world object that should be visually highlighted when the objective starts.
    - **ObjectivePointContext**: Reference to the specific trigger or interaction point that completes the objective.

### 3. `ObjectivePoint`
A component placed on GameObjects in the scene (e.g., a trigger zone, an NPC, or a button).
- **Responsibility**: Signals to the `ObjectiveManager` that the player has fulfilled the requirement.
- **Usage**: Typically triggered by a UnityEvent (like `OnTriggerEnter` or `OnInteract`). Calling `CompleteObjective()` on this component will advance the system.

### 4. `Highlightable`
A visual feedback component.
- **Responsibility**: Draws the player's attention to a specific object.
- **Features**: Can toggle meshes, activate "Objective Arrows," or play effects. It is automatically triggered when its associated objective becomes active.

### 5. `ObjectiveViewSystem` (UI)
- **Responsibility**: Listens to the `ObjectiveManager` and manages the UI representation of objectives.
- **Workflow**: Instantiates `ObjectiveView` prefabs into a UI list when an objective is activated and removes/updates them when completed.

---

## System Flow

1.  **Initialization**: The `ObjectiveManager` registers to the `EventBus`.
2.  **Trigger**: An external script (like a level loader or `TutorialObjectiveRequester`) fires the `OnTutorialObjectiveRequest` event.
3.  **Activation**: 
    - `ObjectiveManager` picks the next `INACTIVE` objective.
    - It triggers the **Highlight** on the target object via `HighlightableContext`.
    - It assigns itself to the **ObjectivePoint** via `ObjectivePointContext`.
    - It fires `OnObjectiveActivated`.
4.  **UI Update**: `ObjectiveViewSystem` catches the event and creates a UI element showing the objective text to the player.
5.  **Completion**: The player interacts with the `ObjectivePoint` in the world. The point calls `CompleteObjective()`.
6.  **Succession**: `ObjectiveManager` marks the objective as `COMPLETE`, stops the highlight, and repeats the process for the next objective in the list.
7.  **Finalization**: Once the list is exhausted, `OnTutorialComplete` is invoked, and the global tutorial state is saved.

---

## How to Add a New Objective

1.  **Create Data**: Right-click in Project -> `System/Objective/ObjectiveData`. Fill in the Name and Text.
2.  **Setup Scene Object**: 
    - Add a `Highlightable` component to the object the player should look at.
    - Add an `ObjectivePoint` component to the object the player should interact with.
3.  **Link Contexts**:
    - Ensure there are `ContextInitializers` in the scene for both the Highlightable and the ObjectivePoint.
    - Assign these Context assets to your `ObjectiveData`.
4.  **Register**: Add your new `ObjectiveData` to the `tutorialObjectives` list on the `ObjectiveManager` component in the scene.
