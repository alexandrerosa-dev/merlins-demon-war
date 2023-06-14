using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Deck
{
    public List<CardData> cardDatas = new List<CardData>();

    public void Create()
    {
        // first create a list of carddata for the pack / primeiro cria uma lista do carddata para o pack
        List<CardData> cardDataInOrder = new List<CardData>();
        foreach(CardData cardData in GameController.instance.cards)
        {
            for (int i = 0; i < cardData.numberInDeck; i++)
                cardDataInOrder.Add(cardData);
        }

        // then randomize the order / depois deixa de a ordem de forma aleatória
        while (cardDataInOrder.Count > 0)
        {
            int randomIndex = Random.Range(0, cardDataInOrder.Count);
            cardDatas.Add(cardDataInOrder[randomIndex]);
            cardDataInOrder.RemoveAt(randomIndex);
        }
    }
}
