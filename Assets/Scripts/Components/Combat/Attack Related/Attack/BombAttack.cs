using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class BombAttack : IAttack
{
    public AttackData AttackData { get => bombAttackData; set => bombAttackData = (BombAttackData) value; }

    [Required, OdinSerialize] private BombAttackData bombAttackData;

    public BombAttack() { }

    public void Hit(Damageable hit, Transform hitMaker)
    {
        // if not landed yet, dont do anything.
        throw new System.NotImplementedException();
    }

    public void OnLaunch()
    {
        throw new System.NotImplementedException();
    }

    public void OnLand(AttackComponent ac)
    {
        // Start fuse timer.
        //bombAttackData.fuseTime;
        ac.StartCoroutine(OnLandEnumerator(ac));
    }

    public IEnumerator OnLandEnumerator(AttackComponent ac)
    {
        yield return new WaitForSeconds(bombAttackData.fuseTime);
        Explode(ac);
    }

    public void Explode(AttackComponent ac)
    {
        // Change Sprite
        SpriteRenderer spriteRenderer = ac.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
            spriteRenderer.sprite = bombAttackData.explosionSprite;
            spriteRenderer.enabled = true;
        }

        // Explode and do damage to any entity in radius. (e.g. collider)

        // Option 1: Do this by spawning an explosion attack overtop.

        // Option 2: Allow for Triggers to occur.

    }


    public void Reset(AttackComponent ac)
    {
        // Change Sprite back to original.
        SpriteRenderer spriteRenderer = ac.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
            spriteRenderer.sprite = bombAttackData.initSprite;
            spriteRenderer.enabled = true;
        }
    }

    public void SetModifiers(float atkStat, double percentageDamageIncrease)
    {
        throw new System.NotImplementedException();
    }
}
