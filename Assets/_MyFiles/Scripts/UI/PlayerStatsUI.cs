using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI armorText;
    [SerializeField] private TextMeshProUGUI goldText;

    private int health = 100;
    private int armor = 50;
    [SerializeField] private int gold = 100;

    private void Start()
    {
        UpdateStatsUI();
    }

    public void UpdateHealth(int amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0, 100); // Adjust max health as needed
        UpdateStatsUI();
    }

    public void UpdateArmor(int amount)
    {
        armor += amount;
        armor = Mathf.Max(0, armor); // Armor can't go below 0
        UpdateStatsUI();
    }

    public void UpdateGold(int amount)
    {
        gold += amount;
        gold = Mathf.Max(0, gold); // Gold can't be negative
        UpdateStatsUI();
    }

    private void UpdateStatsUI()
    {
        healthText.text = $"{health}";
        armorText.text = $"{armor}";
        goldText.text = $"{gold}g";
    }

    public int GetGold()
    {
        return gold;
    }

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            UpdateStatsUI();
            return true;
        }
        Debug.Log("Not enough gold!");
        return false;
    }
}
