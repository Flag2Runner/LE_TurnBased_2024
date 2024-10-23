using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> TurnOrder = new List<GameObject>();
    [SerializeField] private bool BattleStarted = false;
    [SerializeField] private EBattleState BattleState = EBattleState.Wait;
    [SerializeField] private float TransitionTime = 1.5f;
    [SerializeField] private Transform EnemyPos;
    [SerializeField] private int Enemydead;
    [SerializeField] private int PlayerDead;
    [SerializeField] private Wave[] waves;


    [SerializeField] private GameObject CurrentSelection;
    [SerializeField] private EActionType ActionType = EActionType.None;

    private List<GameObject> EnemyList = new List<GameObject>();

    private int currentWave = 0;
    private int Floor = 1;

    private UIManager BattleUI;
    private GameObject BattleCamera;

    private void Update()
    {
        if (Input.GetMouseButtonDown(2) && CurrentSelection)
        {
            switch (ActionType)
            {
                case EActionType.None:
                    //Maybe Shows Enemy/PlayerStats
                    break;
                case EActionType.Attack:
                    TurnOrder[0].GetComponent<IBattleActions>().Attack(CurrentSelection.GetComponent<Character>());
                    Debug.Log("Attack Animation Playing...");
                    Attack();
                    break;
                case EActionType.Heal:
                    TurnOrder[0].GetComponent<IBattleActions>().Heal();
                    Heal();
                    break;
            }
        }
    }
    public EBattleState GetBattleSet() { return BattleState; }
    public void SetActionType(EActionType actionTypeToSet) { ActionType = actionTypeToSet; }
    public void SetCurentSelection(GameObject unitToSet) { CurrentSelection = unitToSet; }
    public void SetBattleState(EBattleState stateToSet)
    {
        BattleState = stateToSet;

        switch (stateToSet)
        {
            case EBattleState.Wait:
                //Do Nothing
                break;
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
                if (TurnOrder != null && TurnOrder.Count > 0)
                {
                    StartCoroutine(EnemyTurn());
                }
                else
                {
                    Debug.LogError("TurnOrder is null or empty in EnemyTurn.");
                }

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
           //GameManager.m_Instance.GetInventoryUIManager().GetInventoryGRP().SetActive(false);
            GameObject BattleUIClone = Instantiate(GameManager.m_Instance.GetBattleUI(), this.gameObject.transform, false);
            BattleUI = BattleUIClone.GetComponent<UIManager>();
           // BattleUI.GetPlayerUI().SetActive(false);

            BattleCamera = GameObject.FindGameObjectWithTag("BattleCamera");

            EnemyList.Clear();
            EnemyList.AddRange(enemyBattleList);

            GatherUnits();
            OrderByDiceRoll();

            SetBattleState(EBattleState.StartBattle);
        }
    }
    public void EndingBattle()
    {
        //GameManager.m_Instance.GetInventoryUIManager().GetInventoryGRP().SetActive(true);
    }
    public void GatherUnits()
    {
        List<GameObject> tempList = new List<GameObject>();
        tempList.AddRange(EnemyList);

        foreach (GameObject unit in tempList)
        {
            GameObject unitClone = Instantiate(unit, new Vector3(0f, -1000f, 0f), this.transform.rotation, this.transform);
            TurnOrder.Add(unitClone);
        }
    }
    public void OrderByDiceRoll()
    {
        foreach (GameObject unit in TurnOrder)
        {
           //unit.GetComponent<Character>().SetDiceNumber(GameManager.m_Instance.DiceRoll());
        }

       // TurnOrder = TurnOrder.OrderBy(x => x.GetComponent<UnitCharacter>().GetDiceNumber()).ToList();
        TurnOrder.Reverse();
    }
    public IEnumerator BattleStart()
    {
        //Time Slow
        Time.timeScale = 0.05f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        yield return new WaitForSeconds(0.075f);

        //Battle Transition
        //BattleUI.PlayTransition();
        Time.timeScale = 1f;
        Time.fixedDeltaTime = Time.timeScale;

        //Move camera or load Level
        yield return new WaitForSeconds(TransitionTime);
        GameManager.m_Instance.GetPlayer().transform.parent.gameObject.SetActive(false);
        BattleCamera.AddComponent<Camera>();
        //BattleCamera.AddComponent<AudioListener>();
        //Spawn Good Guys and Bad Guys

        //BattleUI.EndTransition();

        SetBattleState(EBattleState.ChooseTurn);
    }
    public void ChooseTurn()
    {
        Character currentTurn = TurnOrder[0].GetComponent<Character>();
        if (PlayerDead == TurnOrder.Count / 2)
        {
            Debug.Log("Battle lost...");
            SetBattleState(EBattleState.BattleLost);
            return;
        }
        
        if (Enemydead == TurnOrder.Count / 2)
        {
            Debug.Log("Battle Won...");
            SetBattleState(EBattleState.BattleWon);
            return;
        }
        
        if (currentTurn.GetUnitType() == EUnitType.Player)
        {
            SetBattleState(EBattleState.PlayerTurn);
        }
        else
        {
            SetBattleState(EBattleState.EnemyTurn);
        }


    }

    public void PlayerTurn()
    {
        if (TurnOrder[0].GetComponent<Character>().GetCharacterStats().GetCurrentHealth() <= 0) { EndTurn(); }
        //BattleUI.GetPlayerUI().SetActive(true);
    }
    public IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(2f);
        Debug.Log("Start Attack...");

        if (TurnOrder[0].GetComponent<Character>().GetCharacterStats().GetCurrentHealth() > 0)
        {
            // Filter the list based on the "BattlePlayer" tag
            List<GameObject> playerObjects = TurnOrder.Where(unit => unit.CompareTag("BattlePlayer")).ToList();

            if (playerObjects.Count >= 1)
            {
                Debug.Log("Player Party List Found.");

                // Choose a random player from the filtered list
                int diceRoll = Random.Range(0, playerObjects.Count);
                SetCurentSelection(playerObjects[diceRoll]);

                // Debug statements to identify the issue
                if (TurnOrder[0] != null)
                {
                    IBattleActions battleActions = TurnOrder[0].GetComponent<IBattleActions>();
                    if (battleActions != null)
                    {
                        Debug.Log("IBattleActions found on TurnOrder[0].");
                    }
                    else
                    {
                        Debug.LogError("IBattleActions missing on TurnOrder[0].");
                    }
                }
                else
                {
                    Debug.LogError("TurnOrder[0] is null in EnemyTurn.");
                }

                if (CurrentSelection != null)
                {
                    Character unitCharacter = CurrentSelection.GetComponent<Character>();
                    if (unitCharacter != null)
                    {
                        Debug.Log("UnitCharacter found on CurrentSelection.");
                    }
                    else
                    {
                        Debug.LogError("UnitCharacter missing on CurrentSelection.");
                    }
                }
                else
                {
                    Debug.LogError("CurrentSelection is null in EnemyTurn.");
                }

                // Continue with the rest of the logic
                if (CurrentSelection != null && TurnOrder[0] != null)
                {
                    // Ensure CurrentSelection is not null before accessing its components
                    if (CurrentSelection != null)
                    {
                        // Ensure TurnOrder[0] is not null before accessing its components
                        if (TurnOrder[0] != null)
                        {
                            // Ensure TurnOrder[0] has the necessary components
                            IBattleActions battleActions = TurnOrder[0].GetComponent<IBattleActions>();
                            Character unitCharacter = CurrentSelection.GetComponent<Character>();

                            if (battleActions != null && unitCharacter != null)
                            {
                                battleActions.Attack(unitCharacter);
                                Debug.Log("Attack Animation Playing...");
                                Attack();
                            }
                            else
                            {
                                Debug.LogError("TurnOrder[0] is missing IBattleActions or CurrentSelection is missing UnitCharacter component.");
                            }
                        }
                        else
                        {
                            Debug.LogError("TurnOrder[0] is null in EnemyTurn.");
                        }
                    }
                    else
                    {
                        Debug.LogError("CurrentSelection is null in EnemyTurn.");
                    }
                }
                else
                {
                    Debug.Log("No players found.");
                }

                Debug.Log("Finish Attack!!");
                playerObjects.Clear();
            }
            else
            {
                Debug.Log("No players found.");
            }
        }
        else
        {
            Debug.Log("Enemy is dead");
            EndTurn();
        }
        yield return null;
    }

    public void EndTurn()
    {
        //List<GameObject> firstIndex;
        //Move the first Instance to the end
        // TurnOrder.FindIndex(0,1,);
        //Move all the other instances up
        //  TurnOrder.RemoveAt(0);
        // TurnOrder.Insert(TurnOrder, firstIndex);
        //BattleUI.GetPlayerUI().SetActive(false);

        Debug.Log("Ending Turn...");
        var firstIndex = TurnOrder[0];
        TurnOrder.RemoveAt(0);
        TurnOrder.Insert(TurnOrder.Count, firstIndex);
        Debug.Log("Turn Ended!");
        string turnOrderString = TurnOrder[0].GetComponent<Character>().GetUnitType().ToString();
        //BattleUI.GetComponent<UIManager>().SetTurnMarker(turnOrderString);
        SetBattleState(EBattleState.ChooseTurn);
    }
    public void Attack()
    {
        Debug.Log("Playing Attack Animation...");

        // Get the health stat of the attacker
        CharacterStats attackerStats = TurnOrder[0].GetComponent<Character>().GetCharacterStats();
        CharacterStats targetStats = CurrentSelection.GetComponent<Character>().GetCharacterStats();

        // Reduce attacker's health by 10
        targetStats.Attack(10);

        Debug.Log("Attacker Has Delt 10 Damage!!");

        // Trigger the attack animation
        TurnOrder[0].GetComponent<Animator>().SetTrigger("Attack");
        if (CurrentSelection.GetComponent<Character>().GetCharacterStats().GetCurrentHealth() == 0)
        {
            CurrentSelection.GetComponent<Animator>().SetTrigger("IsDead");
            if (CurrentSelection.GetComponent<Character>().GetUnitType() == EUnitType.Player)
            {
                PlayerDead++;
            }
            else if (CurrentSelection.GetComponent<Character>().GetUnitType() == EUnitType.Enemy)
            {
                Enemydead++;
            }
        }

        Debug.Log("Turn Ending!!");
        EndTurn();
    }

    public void Heal()
    {
        Debug.Log("Playing Heal Animation...");

        // Get the healer and the target
        CharacterStats healerStats = TurnOrder[0].GetComponent<Character>().GetCharacterStats();
        CharacterStats targetStats = CurrentSelection.GetComponent<Character>().GetCharacterStats();


        // Heal the target by 10
        healerStats.Heal(targetStats, 10);

        // Trigger the heal animation
        TurnOrder[0].GetComponent<Animator>().SetTrigger("Heal");
        if (CurrentSelection.GetComponent<Character>().GetCharacterStats().GetCurrentHealth() > 0)
        {
            CurrentSelection.GetComponent<Animator>().SetTrigger("IsAlive");
            if (CurrentSelection.GetComponent<Character>().GetUnitType() == EUnitType.Player)
            {
                PlayerDead--;
            }
            else if (CurrentSelection.GetComponent<Character>().GetUnitType() == EUnitType.Enemy)
            {
                Enemydead--;
            }
        }

        Debug.Log("Turn Ending!!");
        EndTurn();
    }
    public IEnumerator BattleWon()
    {
        //Time Slow
        Time.timeScale = 0.05f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        yield return new WaitForSeconds(0.075f);

        //Battle Transition
        //BattleUI.PlayTransition();
        Time.timeScale = 1f;
        Time.fixedDeltaTime = Time.timeScale;
        yield return new WaitForSeconds(TransitionTime);

        //BattleUI.SetActivePlayerUI(false);
        //BattleUI.SetBattleWinUI(true);

        //BattleUI.EndTransition();


    }
    public IEnumerator BattleLost()
    {
        //Time Slow
        Time.timeScale = 0.05f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        yield return new WaitForSeconds(0.075f);

        //Battle Transition
        //BattleUI.PlayTransition();
        Time.timeScale = 1f;
        Time.fixedDeltaTime = Time.timeScale;
        yield return new WaitForSeconds(TransitionTime);

        //BattleUI.SetActivePlayerUI(false);
        //BattleUI.SetBattleLostUI(true);

       // BattleUI.EndTransition();

    }
}
public enum EBattleState { Wait, StartBattle, ChooseTurn, PlayerTurn, EnemyTurn, BattleWon, BattleLost, COUNT }
public enum EActionType { None, Attack, Heal, Gaurd }
