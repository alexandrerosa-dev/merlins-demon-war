using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BurnZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject obj = eventData.pointerDrag;
        Card card = obj.GetComponent<Card>();
        if (card != null)
        {
            GameController.instance.playersHand.RemoveCard(card);
            GameController.instance.NextPlayerTurn();
        }
    }
}
