using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEditor.Progress;

[CreateAssetMenu(fileName = "Equipment", menuName = "ScriptableObjects/Equipent")]
public class Equipmet : Item
{

    [SerializeField] private EEquipmentType EquipmentType;
    //Fix this
    public int MaxHealthMod, HealthMod, MaxManaMod, ManaMod, PowerMod, IntelligenceMod, SpeedMod, DefenseMod;

    public override void Use()
    {
        base.Use();

        //put the item on
        GameManager.m_Instance.GetPlayer().GetComponent<Character>().GetCharEqquipment().Equip(this);

        RemoveItem();

    }
    public override void RemoveItem()
    {
        base.RemoveItem();

        Debug.Log("Removing Equipment..");
    }
    public EEquipmentType GetEqupmentType() { return EquipmentType; }

}
public enum EEquipmentType { Head, Chest, MainHand_Weapon, OffHandWeapon, Count }