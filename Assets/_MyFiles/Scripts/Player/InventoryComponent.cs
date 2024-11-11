using System.Collections.Generic;
using UnityEngine;

public class InventoryComponent : MonoBehaviour
{
    [Range(1, 20)]
    [SerializeField] private int inventorySpace;
    [SerializeField] private List<Item> items = new List<Item>();
    [SerializeField] private List<InventorySlot> inventorySlots = new List<InventorySlot>();

    private void Awake()
    {
        InventorySlot[] slots = GetComponentsInChildren<InventorySlot>();
        inventorySlots.AddRange(slots);
    }

    public int GetInventorySpace() => inventorySpace;
    public void SetInventorySpace(int spaceToSet) => inventorySpace = spaceToSet;
    public List<Item> GetItemList() => items;

    public bool AddItem(Item itemToAdd)
    {
        if (itemToAdd != null && items.Count < inventorySpace)
        {
            items.Add(itemToAdd);
            GameManager.m_Instance.GetUIManager().UpdateUI(items);
            return true;
        }
        Debug.Log("Inventory Full!");
        return false;
    }

    public void RemoveItem(Item itemToRemove)
    {
        if (itemToRemove != null && items.Contains(itemToRemove))
        {
            items.Remove(itemToRemove);
            GameManager.m_Instance.GetUIManager().UpdateUI(items);
        }
    }

    public void ClearInventory() => items.Clear();
    internal List<InventorySlot> GetInventoryList() => inventorySlots;
}
