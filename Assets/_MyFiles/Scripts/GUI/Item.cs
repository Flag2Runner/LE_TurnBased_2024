using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item")]
public class Item : ScriptableObject
{
    public string m_ItemName { get; private set; }
    public Sprite m_ItemIcon = null;
    [TextArea][SerializeField] string EquipmentDescription;

    public virtual void Use() { Debug.Log(m_ItemName + " is being used!"); }
    public virtual void RemoveItem()
    {
        Debug.Log("Removing Item...");
        GameManager.m_Instance.GetPlayer().GetComponent<Character>().GetInventory().RemoveItem(this);
        Debug.Log("Item Removed!");
    }
}