using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] private string CharacterName;
    [SerializeField] private string CharacterDescription;

    [SerializeField] private List<Stat> StatList = new List<Stat>();

    private void Awake()
    {
        for (int i = 0; i < (int)EStatType.COUNT; i++)
        {
            EStatType type = (EStatType)i;
            Stat newStat = new Stat();
            if (type == EStatType.Health || type == EStatType.MaxHealth ||
                type == EStatType.Mana || type == EStatType.MaxMana)
            {
                newStat.SetBaseValue(100);
            }
            else
            {
                newStat.SetBaseValue(3);
            }
            newStat.SetStatName(type.ToString());
            StatList.Add(newStat);
        }
    }

    public int GetMaxHealth()
    {
        Stat maxHealthStat = GetStat(EStatType.MaxHealth);
        return maxHealthStat != null ? maxHealthStat.GetValue() : 0;
    }

    public int GetCurrentHealth()
    {
        Stat healthStat = GetStat(EStatType.Health);
        return healthStat != null ? healthStat.GetValue() : 0;
    }

    public void Attack(int damage)
    {
        Stat healthStat = GetStat(EStatType.Health);
        if (healthStat != null)
        {
            // Reduce health by the specified amount
            healthStat.ModifyValue(-damage);

            // Ensure health doesn't go below zero
            healthStat.SetRawValue(Mathf.Max(0, healthStat.GetValue()));
        }
    }

    public void Heal(CharacterStats target, int amount)
    {
        Stat healthStat = target.GetStat(EStatType.Health);
        if (healthStat != null)
        {
            // Increase health of the target by the specified amount, up to the maximum
            healthStat.ModifyValue(amount);
            healthStat.SetRawValue(Mathf.Min(healthStat.GetValue(), target.GetMaxHealth()));
        }
    }

    public Stat GetStat(EStatType statToGet)
    {
        string nameToGet = statToGet.ToString();
        foreach (Stat stat in StatList)
        {
            if (stat.GetStatName() == nameToGet)
            {
                return stat;
            }
        }
        return null;
    }
}

public enum EStatType { MaxHealth, Health, MaxMana, Mana, Power, Intelligence, Speed, Defense, COUNT }
