using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjs/Stat Container", fileName = "new stat container")]
public class SO_StatContainer : ScriptableObject
{
    public float health;
    public float maxHealth;
    public float mana;
    public float maxMana;
    public float stamina;
    public float maxStamina;
    public float attack;
    public float defense;
    public float dexterity;
    public float speed;
    public float gold;
}
