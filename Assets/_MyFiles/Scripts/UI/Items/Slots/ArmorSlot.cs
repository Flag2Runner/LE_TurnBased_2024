using UnityEngine;
using UnityEngine.EventSystems;

public class ArmorSlot : InventorySlot
{
    [SerializeField] private EEquipmentType eEquipmentType;
    public override void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
        if (transform.childCount == 1)
        {
            if (transform.GetChild(0).GetComponent<DraggableItem>().GetItemType() == eEquipmentType)
            {
                SwapCheck(draggableItem);
            }
        }


        if (draggableItem != null) { return; }
        if(draggableItem.GetItemType() == eEquipmentType && transform.childCount == 0)
            base.OnDrop(eventData);

        
    }
}
