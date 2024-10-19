using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEqupment : MonoBehaviour
{
    [SerializeField] Equipmet[] CurrentEqupment = new Equipmet[System.Enum.GetNames(typeof(EEquipmentType)).Length];

    public void Equip(Equipmet gear)
    {
        int equipmentIndex = (int)gear.GetEqupmentType();
        if (CurrentEqupment[equipmentIndex] == null)
        {
            Equipmet oldItem = null;

            oldItem = CurrentEqupment[equipmentIndex];
            UnEqupMods(oldItem);
        }

        CurrentEqupment[equipmentIndex] = gear;

        EqupMods(gear);
    }


    private void EqupMods(Equipmet gear)
    {
        foreach (var item in CurrentEqupment)
        {
            //FIX THIS
            GameManager.m_Instance.GetPlayer().GetComponent<Character>().GetCharacterStats().GetStat(EStatType.MaxMana).AddModifier(gear.MaxManaMod);
            GameManager.m_Instance.GetPlayer().GetComponent<Character>().GetCharacterStats().GetStat(EStatType.Mana).AddModifier(gear.ManaMod);
            GameManager.m_Instance.GetPlayer().GetComponent<Character>().GetCharacterStats().GetStat(EStatType.Intelligence).AddModifier(gear.IntelligenceMod);
            GameManager.m_Instance.GetPlayer().GetComponent<Character>().GetCharacterStats().GetStat(EStatType.Speed).AddModifier(gear.SpeedMod);
            GameManager.m_Instance.GetPlayer().GetComponent<Character>().GetCharacterStats().GetStat(EStatType.Defense).AddModifier(gear.DefenseMod);
            GameManager.m_Instance.GetPlayer().GetComponent<Character>().GetCharacterStats().GetStat(EStatType.Health).AddModifier(gear.HealthMod);
            GameManager.m_Instance.GetPlayer().GetComponent<Character>().GetCharacterStats().GetStat(EStatType.MaxHealth).AddModifier(gear.MaxHealthMod);
            GameManager.m_Instance.GetPlayer().GetComponent<Character>().GetCharacterStats().GetStat(EStatType.Power).AddModifier(gear.PowerMod);
            //
        }
    }
    private void UnEqupMods(Equipmet gear)
    {
        foreach (var item in CurrentEqupment)
        {
            //FIX THIS
            GameManager.m_Instance.GetPlayer().GetComponent<Character>().GetCharacterStats().GetStat(EStatType.MaxMana).RemoveModifier(gear.MaxManaMod);
            GameManager.m_Instance.GetPlayer().GetComponent<Character>().GetCharacterStats().GetStat(EStatType.Mana).RemoveModifier(gear.ManaMod);
            GameManager.m_Instance.GetPlayer().GetComponent<Character>().GetCharacterStats().GetStat(EStatType.Intelligence).RemoveModifier(gear.IntelligenceMod);
            GameManager.m_Instance.GetPlayer().GetComponent<Character>().GetCharacterStats().GetStat(EStatType.Speed).RemoveModifier(gear.SpeedMod);
            GameManager.m_Instance.GetPlayer().GetComponent<Character>().GetCharacterStats().GetStat(EStatType.Defense).RemoveModifier(gear.DefenseMod);
            GameManager.m_Instance.GetPlayer().GetComponent<Character>().GetCharacterStats().GetStat(EStatType.Health).RemoveModifier(gear.HealthMod);
            GameManager.m_Instance.GetPlayer().GetComponent<Character>().GetCharacterStats().GetStat(EStatType.MaxHealth).RemoveModifier(gear.MaxHealthMod);
            GameManager.m_Instance.GetPlayer().GetComponent<Character>().GetCharacterStats().GetStat(EStatType.Power).RemoveModifier(gear.PowerMod);
            //
        }
    }
    public void UnEquip(int equipmentIndex)
    {
        if (CurrentEqupment[equipmentIndex])
        {
            Equipmet oldItem = CurrentEqupment[equipmentIndex];
            GameManager.m_Instance.GetPlayer().GetComponent<Character>().GetInventory().AddItem(oldItem);
            CurrentEqupment[equipmentIndex] = null;
        }
    }
}