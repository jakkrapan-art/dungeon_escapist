using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickQueueCard : Minigame
{
    [SerializeField] private PickQueueCardUIController ui;

    public List<QueueCardInfo> queueCards { get; private set; } = new List<QueueCardInfo>();
    private void OnEnable()
    {
        StartCoroutine(nameof(pickQueueCard));
    }

    private IEnumerator pickQueueCard()
    {
        yield return new WaitUntil(() => setUpMiniGame());

        ui.spawnCardUI(queueCards);
    }

    private bool setUpMiniGame()
    {
        List<int> neverUsedNumber = new List<int>();

        //Debug.Log($"Total player: {TotalPlayer}");
        //for (int i = 1; i <= TotalPlayer; i++)
        //{
        //    neverUsedNumber.Add(i);
        //}

        int loopTimes = neverUsedNumber.Count;

        for (int i = 0; i < loopTimes; i++)
        {
            var card = new QueueCardInfo();
            int randomNumberIndex = Random.Range(0, neverUsedNumber.Count);

            card.setQueueNumber(neverUsedNumber[randomNumberIndex]);

            queueCards.Add(card);

            neverUsedNumber.RemoveAt(randomNumberIndex);
        }

        return true;
    }

    public bool isAllPlayerPickCard()
    {
        foreach (var card in queueCards)
        {
            if (card.owner == null)
            {
                return false;
            }
        }

        return true;
    }
}
