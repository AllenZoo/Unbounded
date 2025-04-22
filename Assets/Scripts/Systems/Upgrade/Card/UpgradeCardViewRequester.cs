using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCardViewRequester : MonoBehaviour
{
    [SerializeField] private List<UpgradeCardData> cardsToRequestDisplay = new List<UpgradeCardData>();
    [SerializeField] private bool requestOnStart = false;

    private void Start()
    {
        if (requestOnStart) { 
            RequestDisplay();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RequestDisplay();
        }
    }

    private void RequestDisplay()
    {
        EventBus<OnDisplayUpgradeCardsRequest>.Call(new OnDisplayUpgradeCardsRequest() { upgradeCards = cardsToRequestDisplay });
    }
}
