using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Character : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int health = 100;
    [SerializeField] private int attackPower = 10;
    [SerializeField] private string characterType;
    [SerializeField] private EUnitType unitType;
    [SerializeField] private int diceNumber;

    private CharacterStats characterStats;
    private InventoryComponent inventory;
    private CharacterEquipment equipment;  // Renamed for clarity

    private void Awake()
    {
        characterStats = gameObject.AddComponent<CharacterStats>();
        inventory = gameObject.AddComponent<InventoryComponent>();
        equipment = gameObject.AddComponent<CharacterEquipment>();

        if (unitType == EUnitType.Player)
        {
            inventory.SetInventorySpace(15);
        }
    }

    public CharacterStats GetCharacterStats() => characterStats;
    public EUnitType GetUnitType() => unitType;
    public int GetDiceNumber() => diceNumber;
    public void SetDiceNumber(int value) => diceNumber = value;
    public InventoryComponent GetInventory() => inventory;

    // Re-added method for accessing CharacterEquipment
    public CharacterEquipment GetCharacterEquipment() => equipment;

    public void TakeDamage(int amount)
    {
        characterStats.UpdateHealth(-amount);
        if (characterStats.GetCurrentHealth() <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (unitType == EUnitType.Enemy)
        {
            int goldReward = Random.Range(3, 10);
            GameManager.m_Instance.GetUIManager().GetPlayerStatsUI().AddGold(goldReward);
            Debug.Log($"{characterType} died and rewarded {goldReward} gold!");
        }
        Destroy(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (unitType == EUnitType.Enemy && GameManager.m_Instance.GetBattleManager().GetBattleState() == EBattleState.PlayerTurn)
        {
            GameManager.m_Instance.GetBattleManager().SelectEnemy(this);
        }
        else if (unitType != EUnitType.Enemy)
        {
            Debug.Log("You can only select enemies during the player turn.");
        }
    }

    public void SetInventory(List<Item> itemsToSet)
    {
        if (unitType == EUnitType.Player)
        {
            inventory.GetItemList().Clear();
            inventory.GetItemList().AddRange(itemsToSet);
        }
    }
}

public enum EUnitType { Player, Enemy, NPC }
