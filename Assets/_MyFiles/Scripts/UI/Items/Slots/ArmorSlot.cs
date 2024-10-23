using UnityEngine;
using UnityEngine.EventSystems;

public class ArmorSlot : InventorySlot
{
    public override void OnDrop(PointerEventData eventData)
    {
        if (transform.GetChild(0).GetComponent<DraggableItem>().GetItemType() == EEquipmentType.Armor)
        {
            base.OnDrop(eventData);
        }
    }
}
