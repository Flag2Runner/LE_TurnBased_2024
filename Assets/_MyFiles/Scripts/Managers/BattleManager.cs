using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> TurnOrder = new List<GameObject>();
    [SerializeField] private bool BattleStarted = false;
    [SerializeField] private EBattleState BattleState = EBattleState.Wait;
    [SerializeField] private float TransitionTime = 1.5f;
    [SerializeField] private Transform EnemyPos;
    [SerializeField] private int EnemyDead;
    [SerializeField] private int PlayerDead;
    [SerializeField] private Wave[] waves;

    [SerializeField] private GameObject CurrentSelection;
    [SerializeField] private EActionType ActionType = EActionType.None;

    private List<GameObject> EnemyList = new List<GameObject>();
    private int currentWave = 0;
    private int Floor = 1;
    public EBattleState GetBattleState() { return BattleState; }
    public void SetActionType(EActionType actionTypeToSet) { ActionType = actionTypeToSet; }
    public void SetCurrentSelection(GameObject unitToSet) { CurrentSelection = unitToSet; }

    public void SetBattleState(EBattleState stateToSet)
    {
        BattleState = stateToSet;

        switch (stateToSet)
        {
            case EBattleState.StartBattle:
                StartCoroutine(BattleStart());
                break;
            case EBattleState.ChooseTurn:
                ChooseTurn();
                break;
            case EBattleState.PlayerTurn:
                PlayerTurn();
                break;
            case EBattleState.EnemyTurn:
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
            default:
                ChooseTurn();
                break;
        }
    }

    public void InitializeBattle(List<GameObject> enemyBattleList)
    {
        if (!BattleStarted)
        {
            Debug.Log("Initializing Battle...");
            BattleStarted = true;
            EnemyList.Clear();
            EnemyList.AddRange(enemyBattleList);

            GatherUnits();
            OrderByDiceRoll();

            SetBattleState(EBattleState.StartBattle);
        }
    }

    private void GatherUnits()
    {
        foreach (GameObject unit in EnemyList)
        {
            GameObject unitClone = Instantiate(unit, new Vector3(0f, -1000f, 0f), transform.rotation, transform);
            TurnOrder.Add(unitClone);
        }
    }

    private void OrderByDiceRoll()
    {
        TurnOrder = TurnOrder.OrderByDescending(x => Random.Range(1, 101)).ToList(); // Simulated dice roll
    }

    private IEnumerator BattleStart()
    {
        Time.timeScale = 0.05f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        yield return new WaitForSeconds(0.075f);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = Time.timeScale;
        yield return new WaitForSeconds(TransitionTime);


        SetBattleState(EBattleState.ChooseTurn);
    }

    private void ChooseTurn()
    {
        Character currentTurn = TurnOrder[0].GetComponent<Character>();

        if (PlayerDead >= TurnOrder.Count / 2)
        {
            SetBattleState(EBattleState.BattleLost);
            return;
        }
        if (EnemyDead >= TurnOrder.Count / 2)
        {
            SetBattleState(EBattleState.BattleWon);
            return;
        }

        BattleState = currentTurn.GetUnitType() == EUnitType.Player ? EBattleState.PlayerTurn : EBattleState.EnemyTurn;
        SetBattleState(BattleState);
    }

    private void PlayerTurn()
    {
        if (TurnOrder[0].GetComponent<Character>().GetCharacterStats().GetCurrentHealth() <= 0)
        {
            EndTurn();
        }
    }

    private IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(2f);
        if (TurnOrder[0].GetComponent<Character>().GetCharacterStats().GetCurrentHealth() > 0)
        {
            List<GameObject> playerObjects = TurnOrder.Where(unit => unit.CompareTag("BattlePlayer")).ToList();
            if (playerObjects.Any())
            {
                int diceRoll = Random.Range(0, playerObjects.Count);
                SetCurrentSelection(playerObjects[diceRoll]);
                TurnOrder[0].GetComponent<IBattleActions>()?.Attack(CurrentSelection.GetComponent<Character>());
                Debug.Log("Enemy attacking...");
                Attack();
            }
        }
        else
        {
            Debug.Log("Enemy is dead");
            EndTurn();
        }
    }

    public void EndTurn()
    {
        Debug.Log("Ending Turn...");
        GameObject firstIndex = TurnOrder[0];
        TurnOrder.RemoveAt(0);
        TurnOrder.Add(firstIndex);
        SetBattleState(EBattleState.ChooseTurn);
    }

    public void Attack()
    {
        Debug.Log("Executing Attack...");

        CharacterStats attackerStats = TurnOrder[0].GetComponent<Character>().GetCharacterStats();
        CharacterStats targetStats = CurrentSelection.GetComponent<Character>().GetCharacterStats();

        targetStats.Attack(attackerStats.GetAttackDamage());

        TurnOrder[0].GetComponent<Animator>().SetTrigger("Attack");
        if (targetStats.GetCurrentHealth() <= 0)
        {
            CurrentSelection.GetComponent<Animator>().SetTrigger("IsDead");
            if (CurrentSelection.GetComponent<Character>().GetUnitType() == EUnitType.Player)
                PlayerDead++;
            else
                EnemyDead++;
        }

        EndTurn();
    }

    public void Guard()
    {
        Debug.Log("Guarding...");
        CharacterStats playerStats = TurnOrder[0].GetComponent<Character>().GetCharacterStats();
        playerStats.RegenArmor(10);
        EndTurn();
    }

    public void ConvertShopItemsToAttack()
    {
        ShopUI shopUI = GameManager.m_Instance.GetUIManager().GetShopUI().GetComponent<ShopUI>();
        Debug.Log("Converting shop items to attack modifiers...");
        List<DraggableItem> itemsInShop = shopUI.GetAllShopItems();
        int totalModifier = 0;

        foreach (DraggableItem item in itemsInShop)
        {
            if (item != null)
            {
                totalModifier += item.GetModifierValue(); // Example method
                Destroy(item.gameObject);
            }
        }

        CharacterStats playerStats = TurnOrder[0].GetComponent<Character>().GetCharacterStats();
        playerStats.AddTemporaryStatsModifier(totalModifier);
        Debug.Log($"Total attack bonus this turn: {totalModifier}");

        Attack();
    }

    private IEnumerator BattleWon()
    {
        Time.timeScale = 0.05f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        yield return new WaitForSeconds(0.075f);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = Time.timeScale;
        yield return new WaitForSeconds(TransitionTime);

        // Display victory UI here
    }

    private IEnumerator BattleLost()
    {
        Time.timeScale = 0.05f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        yield return new WaitForSeconds(0.075f);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = Time.timeScale;
        yield return new WaitForSeconds(TransitionTime);

        // Display defeat UI here
    }

    internal void Pass()
    {
        Debug.Log($"Passing turn");
        EndTurn();
    }
}

public enum EBattleState { Wait, StartBattle, ChooseTurn, PlayerTurn, EnemyTurn, BattleWon, BattleLost, COUNT }
public enum EActionType { None, Pass, Attack, Heal, Gaurd }

