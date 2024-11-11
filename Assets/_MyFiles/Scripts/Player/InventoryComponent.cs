using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class InventoryComponent : MonoBehaviour
{
    [Range(1, 20)][SerializeField] private int inventorySpace;

    [SerializeField] private List<Item> items = new List<Item>();
    [SerializeField] private List<InventorySlot> inventorySlots = new();

    private void Awake()
    {
        InventorySlot[] slots = GetComponentsInChildren<InventorySlot>();
        inventorySlots.AddRange(slots);
    }

    public int GetInventorySpace() { return inventorySpace; }

    public void SetInventorySpace(int spaceToSet) { inventorySpace = spaceToSet; }
    public List<Item> GetItemList() { return items; }
    public bool AddItem(Item itemToAdd)
    {
        if (itemToAdd != null && items.Count < inventorySpace)
        {
            items.Add(itemToAdd);
            GameManager.m_Instance.GetUIManager().UpdateUI(items);
            return true;
        }
        else
        {
            Debug.Log("Inventory Full!");
            return false;
        }
    }

    public void RemoveItem(Item itemToRemove)
    {
        Debug.Log("Removing Item...");
        if (itemToRemove != null)
        {
            items.Remove(itemToRemove);
            GameManager.m_Instance.GetUIManager().UpdateUI(items);

        }
        else { Debug.Log("Item Removed!"); }
    }
    public void ClearInventory() { items.Clear(); }

    internal List<InventorySlot> GetInventoryList()
    {
        return inventorySlots;
    }
}
