using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Sprite imageItem;
    private Image imageComponent;

    [SerializeField] private Equipmet equipmentData;
    [SerializeField] private bool isInShop = false;

    private Transform _parentAfterDrag;
    
    private GameObject _currentInventorySlot;

    private void Start()
    {
        imageComponent = GetComponent<Image>();

        if (imageItem)
            imageComponent.sprite = imageItem;
    }
    public bool GetIsInShop() {  return isInShop; }
    public void SetIsInShop(bool newIsInShop) {isInShop = newIsInShop; }
    
    public void SetCurrentIventorySlot(GameObject newInventorySlot) { _currentInventorySlot = newInventorySlot; }

    public GameObject GetCurrentInventorySlot() { return _currentInventorySlot; }

    public EEquipmentType GetItemType()
    {
        return equipmentData.GetEqupmentType();
    }

    public void SetParentAfterDrag(Transform newParent)
    {
        _parentAfterDrag = newParent;
        SetCurrentIventorySlot(newParent.gameObject);
        transform.SetParent(newParent);
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
        Debug.Log("End Drag");
        transform.SetParent(_parentAfterDrag);
        imageComponent.raycastTarget = true;
    }

    internal int GetModifierValue()
    {
        throw new NotImplementedException();
    }
}