using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageNumberText;
    [SerializeField] private Color damageNumberColor = Color.red;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float destroyTime = 1f;
    [SerializeField] private float damageNumber = 0f;

    private void Awake()
    {
        Assert.IsNotNull(damageNumberText, "Damage number needs text component to reflect state.");
    }

    public void SetDamageNumber(float number)
    {
        damageNumberText.text = "- " +number.ToString();
        damageNumberText.color = damageNumberColor;
        damageNumber = number;
    }

    public void StartDestroyTimer()
    {
        StartCoroutine(DestroyTimer());
    }

    private void FixedUpdate()
    {
        transform.position += Vector3.down * moveSpeed * Time.fixedDeltaTime;
    }

    // TODO: use object pooling instead of destryoing and instantiating
    private IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }
}
