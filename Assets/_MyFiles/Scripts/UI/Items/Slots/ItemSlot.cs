using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : InventorySlot
{
    [SerializeField] private EEquipmentType itemType = EEquipmentType.Weapon;
    public override void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
        if(draggableItem.GetItemType() == itemType)
        {
            base.OnDrop(eventData);
            draggableItem.SetIsInShop(false);
        }
    }
}
