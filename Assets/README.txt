UNITY PROJECT FILE PLACEMENT CHEATSHEET

FOLDER STRUCTURE
Assets/
  Core/
  Systems/
  Features/

----------------------------------------
CORE = Foundation (minimal, global)

Put it in Core if:
- It initializes the game
- Everything depends on it
- It is NOT gameplay

Examples:
- GameManager
- GameBootstrap

If you edit it often → it’s probably NOT Core.

----------------------------------------
SYSTEMS = Reusable Services

Put it in Systems if:
- Multiple features use it
- It’s infrastructure
- It is NOT a gameplay mechanic
- It could be reused in another project

If I copied this file into a completely different game, would it still make sense?

Examples:
- ObjectPool
- AudioManager
- SaveSystem
- SceneLoader
- EventBus

Systems must NOT depend on Features.

Systems should be:
 - Dumb
 - Generic
 - Reusable
 - Unaware of gameplay rules

----------------------------------------
FEATURES = Gameplay Mechanics

Put it in Features if:
- Removing it removes a mechanic
- The player experiences it directly
- It defines your game

Examples:
- Player
- Enemies
- Weapons
- Projectiles
- Waves
- Upgrades
- Loot

Features should:
 - Contain gameplay rules.
 - Decide when systems are used.
----------------------------------------
FEATURE STRUCTURE

Features/FeatureName/
  Scripts/    (MonoBehaviours, controllers, runtime logic)
  Data/       (ScriptableObjects, scripts that inherit from ScriptableObject)
  Prefabs/
  UI/         (if feature-specific)

----------------------------------------
QUICK DECISION TEST

Delete it — what happens?

Game loses a mechanic → Feature
Many things break but no mechanic lost → System
Everything breaks → Core

----------------------------------------
EXAMPLES

PlayerMovement → Feature/Player
EnemyAI → Feature/Enemies
Weapon → Feature/Weapons
Projectile → Feature/Projectiles
UpgradeManager → Feature/Upgrades
ObjectPool → Systems
AudioManager → Systems
GameManager → Core