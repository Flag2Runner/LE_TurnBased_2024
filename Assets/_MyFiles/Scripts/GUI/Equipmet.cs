using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "ScriptableObjects/Equipent")]
public class Equipmet : Item
{

    [SerializeField] private EEquipmentType EquipmentType;
    //Fix this
    public int MaxHealthMod, HealthMod, MaxManaMod, ManaMod, AttackMod, MaxArmorMod, ArmorMod;

    public override void Use()
    {
        base.Use();

        //put the item on
        GameManager.m_Instance.GetPlayer().GetComponent<Character>().GetCharacterEquipment().Equip(this);

        RemoveItem();

    }
    public override void RemoveItem()
    {
        base.RemoveItem();

        Debug.Log("Removing Equipment..");
    }
    public EEquipmentType GetEqupmentType() { return EquipmentType; }

}
public enum EEquipmentType { Helmet, Chest, Pants, Shoes, Weapon, Amulet, Ring, None, Count }