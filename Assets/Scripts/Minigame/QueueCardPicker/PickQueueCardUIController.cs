using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PickQueueCardUIController : NetworkBehaviour
{
    public List<QueueCard> cards { get; private set; } = new List<QueueCard>();

    [SerializeField] private Transform spawnPoint = null;
    [SerializeField] private GameObject queueCardPrefab = null;

    [Server]
    public void spawnCardUI(List<QueueCardInfo> cardInfos)
    {
        Debug.Log($"Card info count: {cardInfos.Count}");
        foreach (var card in cardInfos)
        {
            var queueCardInstance = Instantiate(queueCardPrefab, Vector3.zero, Quaternion.identity);

            queueCardInstance.transform.SetParent(spawnPoint);

            var queueCardComponent = queueCardInstance.GetComponent<QueueCard>();

            queueCardComponent.setCardInfo(card);

            cards.Add(queueCardComponent);

            NetworkServer.Spawn(queueCardInstance);
            Debug.Log($"Spawn card number [{queueCardComponent.Info.queueNumber}]");
        }
    }
}
