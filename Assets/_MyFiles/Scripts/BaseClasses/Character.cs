using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Character : MonoBehaviour
{
    private CharacterStats CharacterStatsProfile;
    [SerializeField] private EUnitType UnitType;
    [SerializeField] private int DiceNumber;
    [SerializeField] private InventoryComponent CharacterInventory;
    [SerializeField] private CharacterEqupment CharacterEqupment;
    [SerializeField] private Slider HealthSlider;

    private void Awake()
    {
        CharacterStatsProfile = gameObject.AddComponent<CharacterStats>();
        CharacterInventory = gameObject.AddComponent<InventoryComponent>();
        CharacterEqupment = gameObject.AddComponent<CharacterEqupment>();
        CharacterInventory.SetInventorySpace(15);
    }
    private void FixedUpdate()
    {

        if (CharacterStatsProfile != null)
        {
            HealthSlider.value = GetCharacterStats().GetCurrentHealth();
        }
        else
        {
            return;
        }
    }

    public CharacterStats GetCharacterStats() { return CharacterStatsProfile; }
    public EUnitType GetUnitType() { return UnitType; }
    public int GetDiceNumber() { return DiceNumber; }
    public void SetDiceNumber(int valuseToSet) { DiceNumber = valuseToSet; }
    public InventoryComponent GetInventory() { return CharacterInventory; }
    public CharacterEqupment GetCharEqquipment() { return CharacterEqupment; }

    public void SetInventory(List<Item> itemsToSet)
    {
        CharacterInventory.GetItemList().Clear();
        CharacterInventory.GetItemList().AddRange(itemsToSet);
    }
}

public enum EUnitType { Player, Partner, Enemy, NPC }
