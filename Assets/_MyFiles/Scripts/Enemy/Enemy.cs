using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int health = 100; // Default health, can be set in Inspector
    [SerializeField] private int attackPower = 10; // Default attack power, can be set in Inspector
    [SerializeField] private string enemyType; // Type defined on the prefab

    public int Health { get; private set; }
    public int AttackPower { get; private set; }
    public string EnemyType => enemyType; // Public getter for type

    private void Awake()
    {
        // Initialize health and attack power using values from Inspector
        Health = health;
        AttackPower = attackPower;
    }

    public void TakeDamage(int amount)
    {
        Health -= amount;
        if (Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        int goldReward = UnityEngine.Random.Range(3, 10);
        GameManager.m_Instance.GetUIManager().GetPlayerStatsUI().AddGold(goldReward);
        Destroy(gameObject);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.m_Instance.GetBattleManager().GetBattleState() == EBattleState.PlayerTurn)
        {
            GameManager.m_Instance.GetBattleManager().SelectEnemy(this);
        }
        else
        {
            Debug.Log("Cannot select an enemy during the enemy's turn.");
        }
    }
}
