using UnityEngine;

public class CharacterEquipment : MonoBehaviour
{
    [SerializeField] Equipmet[] equippedItems = new Equipmet[System.Enum.GetNames(typeof(EEquipmentType)).Length];
    private CharacterStats characterStats;

    private void Awake()
    {
        characterStats = GetComponent<Character>().GetCharacterStats();
    }

    public void Equip(Equipmet gear)
    {
        int equipmentIndex = (int)gear.GetEqupmentType();
        Equipmet oldItem = equippedItems[equipmentIndex];

        if (oldItem != null)
        {
            UnEquipModifiers(oldItem);
        }

        equippedItems[equipmentIndex] = gear;
        ApplyModifiers(gear);
    }

    public void UnEquip(int equipmentIndex)
    {
        if (equippedItems[equipmentIndex] != null)
        {
            Equipmet oldItem = equippedItems[equipmentIndex];
            GameManager.m_Instance.GetPlayer().GetComponent<Character>().GetInventory().AddItem(oldItem);
            UnEquipModifiers(oldItem);
            equippedItems[equipmentIndex] = null;
        }
    }

    private void ApplyModifiers(Equipmet gear)
    {
        characterStats.GetStat(EStatType.MaxHealth).AddModifier(gear.MaxHealthMod);
        characterStats.GetStat(EStatType.Health).AddModifier(gear.HealthMod);
        characterStats.GetStat(EStatType.MaxMana).AddModifier(gear.MaxManaMod);
        characterStats.GetStat(EStatType.Mana).AddModifier(gear.ManaMod);
        characterStats.GetStat(EStatType.Attack).AddModifier(gear.AttackMod);
        characterStats.GetStat(EStatType.MaxArmor).AddModifier(gear.MaxArmorMod);
        characterStats.GetStat(EStatType.Armor).AddModifier(gear.ArmorMod);
    }

    private void UnEquipModifiers(Equipmet gear)
    {
        characterStats.GetStat(EStatType.MaxHealth).RemoveModifier(gear.MaxHealthMod);
        characterStats.GetStat(EStatType.Health).RemoveModifier(gear.HealthMod);
        characterStats.GetStat(EStatType.MaxMana).RemoveModifier(gear.MaxManaMod);
        characterStats.GetStat(EStatType.Mana).RemoveModifier(gear.ManaMod);
        characterStats.GetStat(EStatType.Attack).RemoveModifier(gear.AttackMod);
        characterStats.GetStat(EStatType.MaxArmor).RemoveModifier(gear.MaxArmorMod);
        characterStats.GetStat(EStatType.Armor).RemoveModifier(gear.ArmorMod);
    }
}
