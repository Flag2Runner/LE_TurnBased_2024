using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{

    public virtual void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
        if (transform.childCount == 0)
        { 
            draggableItem.SetParentAfterDrag(transform);
            draggableItem.SetIsInShop(false);
            return;
        }

        SwapCheck(draggableItem);
    }

    public void SwapCheck(DraggableItem draggableItem)
    {
        GameObject currentChild = transform.GetChild(0).gameObject;
        DraggableItem childDraggableItem = currentChild.GetComponent<DraggableItem>();

        GameObject newSlot = draggableItem.GetCurrentInventorySlot();

        //Checks if the other item is in a slot to begin with
        if (newSlot == null) { return; }

        ShopSlot newSlotComponent = newSlot.GetComponent<ShopSlot>();

        // Only proceed with swap if new slot is not a ShopSlot
        if (newSlotComponent != null) { return; }

        // Swap the items' positions
        childDraggableItem.SetParentAfterDrag(newSlot.transform); // Move current child to the new slot
        childDraggableItem.SetCurrentIventorySlot(newSlot); // Update current child's inventory slot

        // Move the dragged item to this slot
        draggableItem.SetParentAfterDrag(transform);
        draggableItem.SetCurrentIventorySlot(gameObject);
    }


}
