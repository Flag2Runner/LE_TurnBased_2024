using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private Button rerollButton;
    [SerializeField] private ShopSlot[] shopSlots;
    [SerializeField] private GameObject[] itemPool;

    [Header("[READ ONLY]")]
    [SerializeField] private List<InventorySlot> inventorySlots = new();
    
    private int rerollCost = 1; // Initial cost starts at 1 gold
    private void Start()
    {
        rerollButton.onClick.AddListener(RerollShop);
    }

    public void SetInventoryList(List<InventorySlot> newSlots) 
    {
        inventorySlots = newSlots;
    }

    public InventorySlot GetFreeInventorySlot()
    {
            
        for (int currentSlot = 0; currentSlot > inventorySlots.ToArray().Length; currentSlot++)
        {
            if (inventorySlots[currentSlot].GetComponentInChildren<DraggableItem>() == null)
                return inventorySlots[currentSlot];
        }

        return null;
    }

    //public because the gamemanager will call it after every turn.
    public void RerollOlditems()
    {
        foreach (ShopSlot slot in shopSlots)
        {
            // Skip rerolling if the slot is locked
            if (slot.IsLocked())
            {
                Debug.Log($"Slot at {slot.transform.name} is locked and won't be rerolled.");
                continue;
            }

            // Get the existing DraggableItem in the slot
            DraggableItem existingItem = slot.GetComponentInChildren<DraggableItem>();

            // Only reroll if there is an existing item in the slot
            if (existingItem != null)
            {
                // Destroy the old item
                Destroy(existingItem.gameObject);

                // Create a new item
                int newItemVal = UnityEngine.Random.Range(0, itemPool.Length - 1);
                GameObject newItem = Instantiate(itemPool[newItemVal], slot.gameObject.transform, false);

                // Check if the new item has a DraggableItem component before setting its slot
                DraggableItem newItemDraggable = newItem.GetComponent<DraggableItem>();
                if (newItemDraggable != null)
                {
                    newItemDraggable.SetCurrentIventorySlot(this.gameObject);
                }
            }
        }
    }


    private void RerollShop()
    {
        PlayerStatsUI playerstatsUI = GetComponentInParent<UIManager>().GetPlayerStatsUI();
        if (playerstatsUI == null)
        {
            Debug.Log("PlayerStatsUI is null when RerollShop() is called.");
        }

        if (!playerstatsUI.SpendGold(rerollCost))
        {
            Debug.Log("Not enough gold to reroll the shop.");
            return;
        }


        Debug.Log($"Rerolled the shop for {rerollCost} gold.");

        foreach (ShopSlot slot in shopSlots)
        {
            // Skip rerolling if the slot is locked
            if (slot.IsLocked())
            {
                Debug.Log($"Slot at {slot.transform.name} is locked and won't be rerolled.");
                continue;
            }

            DraggableItem oldItem = slot.GetComponentInChildren<DraggableItem>();
            if (oldItem != null)
            {
                Destroy(oldItem.gameObject);
            }

            int newItemVal = UnityEngine.Random.Range(0, itemPool.Length - 1);
            GameObject newItem = Instantiate(itemPool[newItemVal], slot.gameObject.transform, false);

            DraggableItem newItemDraggable = newItem.GetComponent<DraggableItem>();
            if (newItemDraggable != null) 
            {
                newItemDraggable.SetCurrentIventorySlot(this.gameObject);
            }
        }

        // Increase the reroll cost exponentially (e.g., double the cost each time)
        rerollCost *= 2;
    }
}
