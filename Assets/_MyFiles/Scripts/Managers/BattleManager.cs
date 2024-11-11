using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> TurnOrder = new List<GameObject>();
    [SerializeField] private bool BattleStarted = false;
    [SerializeField] private EBattleState BattleState = EBattleState.Wait;
    [SerializeField] private float TransitionTime = 1.5f;

    [SerializeField] private Transform[] rowTransforms; // Array for modular row transforms

    private List<GameObject> EnemyList = new List<GameObject>();
    private int EnemyDead = 0;
    private int PlayerDead = 0;

    private ChoiceUI choiceUI;

    private void Start()
    {
        choiceUI = GameManager.m_Instance.GetUIManager().GetComponentInChildren<ChoiceUI>();
        if (choiceUI != null) choiceUI.DisableAllButtons(); // Start with buttons disabled
    }

    public void InitializeBattle(Wave wave)
    {
        if (!BattleStarted)
        {
            Debug.Log("Initializing Battle...");
            BattleStarted = true;

            SpawnWave(wave);

            GatherUnits();
            OrderByDiceRoll();

            SetBattleState(EBattleState.StartBattle);
        }
    }

    public void SelectEnemy(Enemy enemy)
    {
        if (enemy == null || BattleState != EBattleState.PlayerTurn)
        {
            Debug.LogWarning("Cannot select enemy at this time.");
            return;
        }

        Debug.Log($"Enemy {enemy.EnemyType} selected.");
    }

    private void SpawnWave(Wave wave)
    {
        foreach (Transform rowTransform in rowTransforms)
        {
            foreach (Transform child in rowTransform)
            {
                Destroy(child.gameObject);
            }
        }

        SpawnEnemyRow(wave.row1Prefabs, rowTransforms[0]);
        SpawnEnemyRow(wave.row2Prefabs, rowTransforms[1]);
        SpawnEnemyRow(wave.row3Prefabs, rowTransforms[2]);
    }

    private void SpawnEnemyRow(GameObject[] enemies, Transform rowTransform)
    {
        float spacing = 2f;

        for (int i = 0; i < enemies.Length; i++)
        {
            GameObject enemyObject = Instantiate(enemies[i], rowTransform);
            enemyObject.transform.localPosition = new Vector3(i * spacing, 0, 0);

            Enemy enemyComponent = enemyObject.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                EnemyList.Add(enemyObject);
            }
        }
    }

    private void GatherUnits()
    {
        foreach (GameObject unit in EnemyList)
        {
            TurnOrder.Add(unit);
        }
    }

    private void OrderByDiceRoll()
    {
        TurnOrder.Sort((a, b) => Random.Range(1, 101) - Random.Range(1, 101));
    }

    public void EndTurn()
    {
        Debug.Log("Ending Turn...");
        GameObject firstUnit = TurnOrder[0];
        TurnOrder.RemoveAt(0);
        TurnOrder.Add(firstUnit);
        SetBattleState(EBattleState.ChooseTurn);
    }

    public void SetBattleState(EBattleState state)
    {
        BattleState = state;

        switch (state)
        {
            case EBattleState.StartBattle:
                StartCoroutine(BattleStart());
                break;
            case EBattleState.ChooseTurn:
                ChooseTurn();
                break;
            case EBattleState.PlayerTurn:
                choiceUI.EnableAllButtons(); // Enable UI for player interaction
                break;
            case EBattleState.EnemyTurn:
                choiceUI.DisableAllButtons(); // Disable UI during enemy turn
                if (TurnOrder.Count > 0)
                    StartCoroutine(EnemyTurn());
                else
                    Debug.LogError("TurnOrder is empty in EnemyTurn.");
                break;
            case EBattleState.BattleWon:
                StartCoroutine(BattleWon());
                break;
            case EBattleState.BattleLost:
                StartCoroutine(BattleLost());
                break;
        }
    }

    public EBattleState GetBattleState() { return BattleState; }

    private IEnumerator BattleStart()
    {
        Time.timeScale = 0.05f;
        yield return new WaitForSeconds(TransitionTime * 0.05f);

        Time.timeScale = 1f;
        yield return new WaitForSeconds(TransitionTime);

        SetBattleState(EBattleState.ChooseTurn);
    }

    private void ChooseTurn()
    {
        if (PlayerDead >= TurnOrder.Count / 2)
        {
            SetBattleState(EBattleState.BattleLost);
        }
        else if (EnemyDead >= TurnOrder.Count / 2)
        {
            SetBattleState(EBattleState.BattleWon);
        }
        else
        {
            EBattleState nextState = TurnOrder[0].GetComponent<Character>().GetUnitType() == EUnitType.Player ? EBattleState.PlayerTurn : EBattleState.EnemyTurn;
            SetBattleState(nextState);
        }
    }

    private void PlayerTurn()
    {
        if (TurnOrder[0].GetComponent<Character>().GetCharacterStats().GetCurrentHealth() <= 0)
            EndTurn();
    }

    private IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(2f);
        EndTurn();
    }

    public void Attack(bool endTurnAfterAttack = true)
    {
        if (BattleState != EBattleState.PlayerTurn) return;

        Debug.Log("Player Attack!");

        // Attack logic here, e.g., targeting a selected enemy

        if (endTurnAfterAttack)
        {
            EndTurn();
        }
    }

    public void Guard()
    {
        if (BattleState != EBattleState.PlayerTurn) return;

        Debug.Log("Player Guarding - Regenerating armor");
        CharacterStats playerStats = TurnOrder[0].GetComponent<Character>().GetCharacterStats();
        playerStats.RegenArmor(10);

        EndTurn();
    }

    public void Pass()
    {
        if (BattleState != EBattleState.PlayerTurn) return;

        Debug.Log("Player Passes Turn");
        EndTurn();
    }

    public void ConvertShopItemsToAttack()
    {
        if (BattleState != EBattleState.PlayerTurn) return;

        Debug.Log("Converting Shop Items to Attack Power");

        // Get the shop UI and initialize total attack modifier
        ShopUI shopUI = GameManager.m_Instance.GetUIManager().GetShopUI().GetComponent<ShopUI>();
        int totalAttackModifier = 0;

        // Gather all shop items and sum their modifier values, then destroy them
        foreach (DraggableItem item in shopUI.GetAllShopItems())
        {
            totalAttackModifier += item.GetModifierValue();
            Destroy(item.gameObject); // Remove the item after converting
        }

        // Apply the modifier temporarily
        CharacterStats playerStats = TurnOrder[0].GetComponent<Character>().GetCharacterStats();
        playerStats.AddTemporaryStatsModifier(totalAttackModifier);

        Debug.Log($"Total Attack Bonus this turn: {totalAttackModifier}");

        // Perform the attack with the added modifier, but don't end the turn yet
        Attack(false);

        // Remove the temporary modifier after the attack
        playerStats.AddTemporaryStatsModifier(-totalAttackModifier);

        Debug.Log("Temporary attack modifier removed.");

        // End the player's turn
        EndTurn();
    }

    private IEnumerator BattleWon()
    {
        Time.timeScale = 0.05f;
        yield return new WaitForSeconds(TransitionTime * 0.05f);

        Time.timeScale = 1f;
        yield return new WaitForSeconds(TransitionTime);
        Debug.Log("Battle Won!");

        GameManager.m_Instance.DestroyBattleManager();
    }

    private IEnumerator BattleLost()
    {
        Time.timeScale = 0.05f;
        yield return new WaitForSeconds(TransitionTime * 0.05f);

        Time.timeScale = 1f;
        yield return new WaitForSeconds(TransitionTime);
        Debug.Log("Battle Lost!");

        GameManager.m_Instance.DestroyBattleManager();
    }
}

public enum EBattleState { Wait, StartBattle, ChooseTurn, PlayerTurn, EnemyTurn, BattleWon, BattleLost, COUNT }
public enum EActionType { None, Pass, Attack, Heal, Guard }
