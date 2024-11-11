using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> TurnOrder = new List<GameObject>();
    [SerializeField] private bool BattleStarted = false;
    [SerializeField] private EBattleState BattleState = EBattleState.Wait;
    [SerializeField] private float TransitionTime = 1.5f;

    [SerializeField] private Transform[] rowTransforms;
    [SerializeField] private GameObject selectionIndicatorPrefab;

    private GameObject player;
    private List<GameObject> EnemyList = new List<GameObject>();
    private int EnemyDead = 0;
    private int PlayerDead = 0;
    private Character selectedEnemy;
    private GameObject selectionIndicator;
    private int selectedRow = 0;
    private int selectedIndex = 0;

    private ChoiceUI choiceUI;

    private void Start()
    {
        choiceUI = GameManager.m_Instance.GetUIManager().GetComponentInChildren<ChoiceUI>();
        if (choiceUI != null) choiceUI.DisableAllButtons(); // Start with buttons disabled

        // Instantiate the selection indicator and hide it initially
        if (selectionIndicatorPrefab != null)
        {
            selectionIndicator = Instantiate(selectionIndicatorPrefab, transform);
            selectionIndicator.SetActive(false);
        }
    }

    private void Update()
    {
        if (BattleState == EBattleState.PlayerTurn)
        {
            HandleKeyboardInput();
        }
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveSelection(1); // Move selection right
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveSelection(-1); // Move selection left
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && selectedEnemy != null)
        {
            SelectEnemy(selectedEnemy); // Confirm selection
            
        }
    }

    private void MoveSelection(int direction)
    {
        int rowCount = rowTransforms.Length;
        Transform currentRow = rowTransforms[selectedRow];
        int enemyCount = currentRow.childCount;

        // Update the selected index within bounds, wrapping around if necessary
        selectedIndex = (selectedIndex + direction + enemyCount) % enemyCount;
        Transform selectedTransform = currentRow.GetChild(selectedIndex);

        // Update the selected enemy and move the selection indicator
        selectedEnemy = selectedTransform.GetComponent<Character>();
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(true);
            StartCoroutine(LerpIndicatorPosition(selectedTransform.position));
        }
    }

    public void InitializeBattle(GameObject playerCharacter, Wave wave)
    {
        if (!BattleStarted)
        {
            Debug.Log("Initializing Battle...");
            BattleStarted = true;

            player = playerCharacter;

            SpawnWave(wave);
            GatherUnits();
            OrderByDiceRoll();

            SetBattleState(EBattleState.StartBattle);
        }
    }

    public void SelectEnemy(Character enemy)
    {
        if (BattleState != EBattleState.PlayerTurn)
        {
            Debug.LogWarning("Cannot select enemy during the enemy's turn.");
            return;
        }

        if (enemy == null || enemy.GetUnitType() == EUnitType.Player)
        {
            Debug.LogWarning("No enemy selected.");
            return;
        }

        selectedEnemy = enemy;
        Debug.Log($"Selected Enemy: {enemy.GetUnitType()}");
        HighlightEnemy(enemy);
    }

    private void HighlightEnemy(Character enemy)
    {
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(true);
            StartCoroutine(LerpIndicatorPosition(enemy.transform.position));
        }
    }

    private IEnumerator LerpIndicatorPosition(Vector3 targetPosition)
    {
        float duration = 0.3f;
        Vector3 startPosition = selectionIndicator.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            selectionIndicator.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        selectionIndicator.transform.position = targetPosition;
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

            Character enemyComponent = enemyObject.GetComponent<Character>();
            if (enemyComponent != null)
            {
                EnemyList.Add(enemyObject);
            }
        }
    }

    private void GatherUnits()
    {
        if (player != null)
        {
            TurnOrder.Add(player);
        }

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
                choiceUI.EnableAllButtons();
                break;
            case EBattleState.EnemyTurn:
                choiceUI.DisableAllButtons();
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

    private IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(1f); // Wait a bit for clarity between actions

        // Retrieve the enemy's character stats
        CharacterStats enemyStats = TurnOrder[0].GetComponent<Character>().GetCharacterStats();

        // Choose a random action for the enemy
        int actionChoice = Random.Range(0, 3); // 0 = Attack, 1 = Guard, 2 = Pass

        switch (actionChoice)
        {
            case 0: // Attack
                Debug.Log("Enemy chooses to attack!");

                // Select a random target (in this case, it will be the player character)
                CharacterStats playerStats = TurnOrder.First(unit => unit.GetComponent<Character>().GetUnitType() == EUnitType.Player).GetComponent<Character>().GetCharacterStats();

                // Deal damage to the player, considering armor first
                int damage = enemyStats.GetAttackDamage();
                int leftoverDamage = Mathf.Max(0, damage - playerStats.GetCurrentArmor());

                playerStats.RegenArmor(-damage); // Deduct armor first
                if (leftoverDamage > 0)
                {
                    GameManager.m_Instance.GetPlayer().GetComponent<Character>().TakeDamage(leftoverDamage); // Apply leftover damage to health
                }
                Debug.Log($"Enemy dealt {damage} damage. Leftover health damage: {leftoverDamage}");
                break;

            case 1: // Guard
                Debug.Log("Enemy chooses to guard.");
                enemyStats.RegenArmor(5); // Guard restores armor by a fixed amount (customize as needed)
                break;

            case 2: // Pass
                Debug.Log("Enemy chooses to pass their turn.");
                break;
        }

        // End the enemy's turn after the chosen action
        yield return new WaitForSeconds(1f); // Add a delay for readability in turn transitions
        EndTurn();
    }

    public void Attack(bool endTurnAfterAttack = true)
    {
        if (BattleState != EBattleState.PlayerTurn || selectedEnemy == null) return;

        Debug.Log("Player Attack!");

        selectedEnemy.TakeDamage(TurnOrder[0].GetComponent<Character>().GetCharacterStats().GetAttackDamage());

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

        ShopUI shopUI = GameManager.m_Instance.GetUIManager().GetShopUI().GetComponent<ShopUI>();
        int totalAttackModifier = 0;

        foreach (DraggableItem item in shopUI.GetAllShopItems())
        {
            totalAttackModifier += item.GetModifierValue();
            Destroy(item.gameObject);
        }

        CharacterStats playerStats = TurnOrder[0].GetComponent<Character>().GetCharacterStats();
        playerStats.AddTemporaryStatsModifier(totalAttackModifier, EStatType.Attack);

        Debug.Log($"Total Attack Bonus this turn: {totalAttackModifier}");

        Attack(false);

        // Remove the temporary modifier after the attack
        playerStats.RemoveTemporaryStatsModifier(EStatType.Attack);

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
