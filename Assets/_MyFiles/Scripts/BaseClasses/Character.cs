using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Character : MonoBehaviour
{
    private CharacterStats CharacterStatsProfile;
    [SerializeField] private EUnitType UnitType;
    [SerializeField] private int DiceNumber;
    
    private InventoryComponent _characterInventory;
    private CharacterEqupment _characterEqupment;

    private void Awake()
    {
        CharacterStatsProfile = gameObject.AddComponent<CharacterStats>();
        _characterInventory = gameObject.AddComponent<InventoryComponent>();
        _characterEqupment = gameObject.AddComponent<CharacterEqupment>();
        _characterInventory.SetInventorySpace(15);
    }

    public CharacterStats GetCharacterStats() { return CharacterStatsProfile; }
    public EUnitType GetUnitType() { return UnitType; }
    public int GetDiceNumber() { return DiceNumber; }
    public void SetDiceNumber(int valuseToSet) { DiceNumber = valuseToSet; }
    public InventoryComponent GetInventory() { return _characterInventory; }
    public CharacterEqupment GetCharEqquipment() { return _characterEqupment; }

    public void SetInventory(List<Item> itemsToSet)
    {
        _characterInventory.GetItemList().Clear();
        _characterInventory.GetItemList().AddRange(itemsToSet);
    }
}

public enum EUnitType { Player, Enemy, NPC }
