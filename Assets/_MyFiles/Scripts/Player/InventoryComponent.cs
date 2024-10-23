using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class InventoryComponent : MonoBehaviour
{
    [Range(1, 20)][SerializeField] private int InventorySpace;

    [SerializeField] private List<Item> Items = new List<Item>();


    public int GetInventorySpace() { return InventorySpace; }

    public void SetInventorySpace(int spaceToSet) { InventorySpace = spaceToSet; }
    public List<Item> GetItemList() { return Items; }
    public bool AddItem(Item itemToAdd)
    {
        if (itemToAdd != null && Items.Count < InventorySpace)
        {
            Items.Add(itemToAdd);
            GameManager.m_Instance.GetInventoryUIManager().UpdateUI(Items);
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
            Items.Remove(itemToRemove);
            GameManager.m_Instance.GetInventoryUIManager().UpdateUI(Items);

        }
        else { Debug.Log("Item Removed!"); }
    }
    public void ClearInventory() { Items.Clear(); }
}
