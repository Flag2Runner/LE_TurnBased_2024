using UnityEngine;
using UnityEngine.EventSystems;

public class ArmorSlot : InventorySlot
{
    [SerializeField] private EEquipmentType eEquipmentType = EEquipmentType.Helmet;
    public override void OnDrop(PointerEventData eventData)
    {
        if (transform.GetChild(0).GetComponent<DraggableItem>().GetItemType() == eEquipmentType)
        {
            base.OnDrop(eventData);
        }
    }
}
