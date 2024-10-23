using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : InventorySlot
{
    public override void OnDrop(PointerEventData eventData)
    {
        if (transform.GetChild(0).GetComponent<DraggableItem>().GetItemType() == EEquipmentType.Item)
        {
            base.OnDrop(eventData);
        }
    }
}
