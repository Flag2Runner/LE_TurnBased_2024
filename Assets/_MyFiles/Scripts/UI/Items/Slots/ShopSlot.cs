using UnityEngine;
using UnityEngine.EventSystems;

public class ShopSlot : InventorySlot, IPointerClickHandler
{
    private ShopUI m_shopUI;
    private bool isLocked = false; // New field to track if the slot is locked

    private void Start()
    {
        m_shopUI = GetComponentInParent<ShopUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Check if the slot is empty (i.e., no child item)
            if (transform.childCount == 0)
            {
                isLocked = false;
                Debug.Log("Cannot lock an empty slot.");
                return; // Exit if the slot is empty
            }

            // Toggle lock status when right-clicked
            isLocked = !isLocked;
            Debug.Log($"Slot locked status: {isLocked}");
        }

        if (eventData.button == PointerEventData.InputButton.Left && transform.childCount > 0)
        {
            // Try to buy the item in the slot and add it to the inventory or have it drag with the player.
            InventorySlot freeInventorySlot = m_shopUI.GetFreeInventorySlot();
            Debug.Log($"{freeInventorySlot}");
            if (freeInventorySlot == null) { return; }

            GameObject clicked = eventData.pointerClick;
            DraggableItem draggableItem = clicked.GetComponent<DraggableItem>();
            Debug.Log($"{clicked}: item is : {draggableItem}");
            if (draggableItem)
            {
                draggableItem.SetParentAfterDrag(freeInventorySlot.transform);
                draggableItem.SetIsInShop(false);
            }
            Debug.Log($"Item has been {eventData.button} clicked");
        }
    }

    public bool IsLocked()
    {
        return isLocked;
    }
    public override void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        if (dropped == null) return; // Safety check

        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
        if (draggableItem == null) return; // Safety check

        // Use the fucntion method to check if the item is from another ShopSlot
        if (IsItemFromAnotherShopSlot(draggableItem))
        {
            Debug.Log("Cannot move items from one shop slot to another.");
            return;
        }

        // Allow drop if it didn't originate from a ShopSlot
        if (draggableItem.GetIsInShop() == true)
        {
            draggableItem.SetParentAfterDrag(transform);
            isLocked = false;
        }
    }
    //checks if the droped item is from another ShopSlto and returns a bool
    private bool IsItemFromAnotherShopSlot(DraggableItem item)
    {
        if (item.GetCurrentInventorySlot() != null && item.GetCurrentInventorySlot().GetComponent<ShopSlot>() != null)
        {
            return true;
        }
        return false;
    }

}
