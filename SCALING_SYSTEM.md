# Boss Scaling System Documentation

## Overview
The Boss Scaling System automatically scales boss statistics based on the current round number, making each subsequent boss encounter more challenging as the player progresses through the game.

## Implementation

### Components
- **BossScalingSystem.cs**: MonoBehaviour component that should be attached to boss entities
- Located in: `Assets/Scripts/Systems/GameManagement/BossScalingSystem.cs`

### How It Works

1. **Automatic Detection**: The system automatically detects if an entity is a boss by checking the `EntityType.BOSS_MONSTER` on the `Damageable` component
2. **Round-Based Scaling**: Stats are scaled based on the `GameManagerComponent.roundNumber`
3. **Compound Growth**: Uses a compound multiplier formula: `newStat = baseStat × (multiplier ^ effectiveRounds)` where `effectiveRounds = currentRound - 1` (scaling starts from round 2)
4. **Stat Modifiers**: Applies scaling through the existing StatMediator system, ensuring compatibility with other stat modifications
5. **One-Time Application**: Scaling is applied once during `Awake()` and protected by a flag to prevent double-scaling

### Scaled Statistics

| Stat | Default Multiplier | Growth per Round |
|------|-------------------|------------------|
| Health | 1.15 | +15% compound |
| Attack | 1.10 | +10% compound |
| Defense | 1.08 | +8% compound |

### Configuration

The following parameters can be configured via the Inspector:

- **Health Scaling Multiplier** (default: 1.15): Health growth rate per round
- **Attack Scaling Multiplier** (default: 1.10): Attack growth rate per round
- **Defense Scaling Multiplier** (default: 1.08): Defense growth rate per round
- **Max Round Number** (default: 10): Maximum round for scaling calculations

### Usage

1. Attach the `BossScalingSystem` component to any boss prefab
2. Ensure the boss has:
   - `StatComponent` (required for stat management)
   - `Damageable` component with `EntityType.BOSS_MONSTER` (required for boss detection)
3. (Optional) Adjust scaling multipliers in the Inspector if default values need tuning

### Example Scaling

For a boss with base stats of **100 HP, 10 ATK, 5 DEF**:

| Round | Health | Attack | Defense |
|-------|--------|--------|---------|
| 1 | 100 | 10 | 5.0 |
| 2 | 115 | 11 | 5.4 |
| 3 | 132 | 12.1 | 5.8 |
| 5 | 175 | 14.6 | 6.8 |
| 10 | 404 | 23.6 | 10.0 |

### Integration with Game Loop

- Scaling is applied in the `Awake()` method of the boss entity
- The current round number is retrieved from `GameManagerComponent.Instance.roundNumber`
- Round progression happens in `GameManagerComponent.HandleOnBossRoomState()`

## Bug Fixes

### ATK Stat Application Fix

**Issue**: Player attacks from `RingAttack` and `ClusterAttack` were not applying the ATK stat to damage calculations.

**Root Cause**: When creating new `AttackContext` objects for spawned attacks, the `AtkStat` and `PercentageDamageIncrease` fields were not being copied from the original context.

**Fix Applied**:
- **RingAttack.cs**: Modified `Spawn()` method to include `AtkStat` and `PercentageDamageIncrease` in the `AttackContext`
- **ClusterAttack.cs**: Modified `Split()` method to copy `AtkStat` and `PercentageDamageIncrease` from `originalContext`

**Impact**: Player attacks now correctly apply the ATK stat from the player's StatComponent, resulting in proper damage scaling based on player equipment and buffs.

## Testing Recommendations

1. **ATK Stat Bug Fix**:
   - Equip weapons with different ATK values
   - Use RingAttack or ClusterAttack abilities
   - Verify damage numbers match expected values (BaseDamage + ATK stat)

2. **Boss Scaling**:
   - Progress through multiple rounds
   - Observe boss health bars and damage output increasing
   - Check debug logs for scaling information (when Debug.isDebugBuild is true)

## Future Enhancements

Potential improvements for the scaling system:
- Add scaling for Speed (SPD) and Dexterity (DEX) stats
- Implement difficulty modifiers (Easy/Normal/Hard modes)
- Add visual indicators showing boss round/difficulty level
- Create scaling presets for different boss types
- Add diminishing returns for very high round numbers
