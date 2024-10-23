using UnityEngine;
using UnityEngine.EventSystems;

public class ShopSlot : InventorySlot, IPointerClickHandler
{
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && transform.childCount > 0)
        {
            //Lock the slot if there is a item in it
            Debug.Log($"Item has been {eventData.button} clicked");
        }

        if (eventData.button == PointerEventData.InputButton.Left && transform.childCount > 0)
        {
            //Try to buy the item in the slot and add it to the inventory or have it drag with the player.
            Debug.Log($"Item has been {eventData.button} clicked");
        }
    }

    public override void OnDrop(PointerEventData eventData)
    {
        //Empty to overide the one on the default one and have it not take items into it only give if the player
        // can pay for it
    }
}
