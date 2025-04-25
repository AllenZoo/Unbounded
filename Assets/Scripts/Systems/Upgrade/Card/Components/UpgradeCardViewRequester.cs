using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCardViewRequester : SerializedMonoBehaviour
{
    [OdinSerialize, SerializeField] private HashSet<UpgradeCardData> cardsToRequestDisplay = new HashSet<UpgradeCardData>();
    [SerializeField] private bool requestOnStart = false;

    private void Start()
    {
        if (requestOnStart)
        {
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
