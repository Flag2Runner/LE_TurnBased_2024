using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image imageComponent;

    [SerializeField] private EEquipmentType itemType;
    private Transform _parentAfterDrag;

    public EEquipmentType GetItemType()
    {
        return itemType;
    }

    public void SetParentAfterDrag(Transform newParent)
    {
        _parentAfterDrag = newParent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
        Debug.Log("Begin Drag");
        _parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        imageComponent.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ShopSlot shopSlot = _parentAfterDrag.GetComponent<ShopSlot>();
        if (!shopSlot)
        {
            Debug.Log("End Drag");
            transform.SetParent(_parentAfterDrag);
        }

        imageComponent.raycastTarget = true;
    }
}