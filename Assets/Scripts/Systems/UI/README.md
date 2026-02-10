# UI State Management - Player Input Disabling

This document explains the implementation for disabling player attack input when UI elements are active.

## Problem
When the player interacts with UI elements (modals, inventories, menus), the attack input would still be processed, causing NullReference errors with the attack pool.

## Solution
A centralized UIStateManager singleton tracks when any UI element is open and PlayerInput checks this state before processing attack input.

## Implementation Details

### Components

#### 1. UIStateManager (`Assets/Scripts/Systems/UI/UIStateManager.cs`)
- **Singleton pattern**: Ensures only one instance exists
- **Counter-based tracking**: Tracks number of open UI elements (not just boolean)
- **Auto-creation**: Creates itself via `RuntimeInitializeOnLoadMethod` if not in scene
- **Event system**: Fires `OnUIActiveStateChanged` event when state changes

Key Methods:
- `RegisterUIOpen()`: Increments counter when UI opens
- `RegisterUIClose()`: Decrements counter when UI closes
- `IsUIActive`: Property that returns true if any UI is open

#### 2. PlayerInput Modifications (`Assets/Scripts/Components/Input/PlayerInput.cs`)
Modified `Handle_Attack_Input()` to check UI state:
```csharp
if (UIStateManager.Instance != null && UIStateManager.Instance.IsUIActive)
{
    return; // Block attack input when UI is active
}
```

#### 3. PageUI Integration (`Assets/Scripts/Systems/UI/Page/PageUI.cs`)
- Tracks registration state with `isRegisteredAsOpen` flag
- Registers on `Start()` if canvas is already enabled
- Registers/unregisters in `ToggleVisibility()` method
- Prevents double registration

#### 4. ModalContext Integration (`Assets/UI Toolkit/Data/Modal/ModalContext.cs`)
- Checks previous state before registering
- Uses helper method `HandleUIStateRegistration()` to avoid code duplication
- All three `Open()` overloads properly register UI state
- `Close()` method unregisters UI state

## Testing

### Manual Testing
1. Attach `UIStateTest.cs` to a GameObject in the scene
2. Press 'T' to check current UI state
3. Press 'U' to simulate opening UI (should disable attack input)
4. Press 'I' to simulate closing UI (should enable attack input)
5. Try left-clicking when UI is open - attacks should not trigger

### Expected Behavior
- Opening inventory/modal should prevent attack input
- Closing all UI elements should restore attack input
- Multiple UI elements can be open simultaneously
- Attack input is only restored when all UI elements are closed

## Debug Logging
- UIStateManager logs warnings when unbalanced open/close calls detected
- PlayerInput logs when input is disabled (if InputEnabled is false)
- PageUI logs when pages are added
- UIStateTest provides detailed state information

## Architecture Benefits
1. **Centralized state**: Single source of truth for UI state
2. **Decoupled**: UI components don't need direct reference to player
3. **Scalable**: Easy to add more UI elements or input types
4. **Safe**: Guards against double registration and negative counters
5. **Flexible**: Event system allows other systems to react to UI state changes

## Future Enhancements
- Could extend to disable movement input as well
- Could add priority levels for different UI types
- Could track which specific UI elements are open for debugging
- Could add analytics for UI interaction patterns
