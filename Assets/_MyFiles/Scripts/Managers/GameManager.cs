using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Wave
{
    public GameObject[] row1Prefabs;
    public GameObject[] row2Prefabs;
    public GameObject[] row3Prefabs;
}

public class GameManager : MonoBehaviour
{
    public static GameManager m_Instance;

    [Header("Player Info")]
    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private Transform PlayerSpawn;
    private GameObject Player;

    [Header("Wave Data")]
    [SerializeField] private Wave[] Waves;

    [Header("UI")]
    [SerializeField] private GameObject UIManager;
    [SerializeField] private GameObject InventorySlotUI;

    [Header("Battle Manager")]
    [SerializeField] private BattleManager battleManagerPrefab; // Reference to the BattleManager prefab
    private BattleManager CurrentBattle;

    private int currentWaveIndex = 0;

    private void Awake()
    {
        if (m_Instance != null && m_Instance != this)
        {
            Debug.LogError("Multiple GameManagers found. Deleting copy...");
            Destroy(this);
        }
        else
        {
            m_Instance = this;
        }

        if (PlayerPrefab && PlayerSpawn)
        {
            GameObject playerGrp = Instantiate(PlayerPrefab, PlayerSpawn.position, PlayerSpawn.rotation);
            Player = playerGrp.GetComponentInChildren<Character>().gameObject;
            LoadData();
        }
        else
        {
            Debug.LogWarning("Player Prefab or PlayerSpawn not Referenced");
        }
        StartBattle();
    }

    public GameObject GetPlayer() => Player;

    public void DestroyInventoryUIManager()
    {
        Destroy(UIManager);
        UIManager = null;
    }

    public UIManager GetUIManager() => UIManager.GetComponent<UIManager>();

    public void StartBattle()
    {
        if (CurrentBattle == null)
        {
            CreateBattleManager();
            currentWaveIndex = 1;
            m_Instance.GetUIManager().GetPlayerStatsUI().UpdateWaveText(currentWaveIndex);
        }
    }

    public void CreateBattleManager()
    {
        if (battleManagerPrefab == null)
        {
            Debug.LogError("BattleManager prefab is not assigned in the GameManager.");
            return;
        }

        Debug.Log("Instantiating BattleManager...");
        CurrentBattle = Instantiate(battleManagerPrefab, transform.position, Quaternion.identity);

        // Pass the player and the wave to InitializeBattle
        CurrentBattle.InitializeBattle(Player, GetNextWave());

        UpdateWaveText();
    }


    public void DestroyBattleManager()
    {
        if (CurrentBattle != null)
        {
            Destroy(CurrentBattle.gameObject);
            CurrentBattle = null;
        }

        // After destroying, start the next wave after a delay
        Invoke(nameof(StartBattle), 2f); // Delay before starting the next wave
    }

    public BattleManager GetBattleManager() => CurrentBattle;

    public Wave GetNextWave()
    {
        if (currentWaveIndex < Waves.Length)
        {
            return Waves[currentWaveIndex++];
        }
        else
        {
            return GenerateRandomWave();
        }
    }

    private Wave GenerateRandomWave()
    {
        Wave randomWave = new Wave();
        randomWave.row1Prefabs = GenerateRandomEnemyRow(3);
        randomWave.row2Prefabs = GenerateRandomEnemyRow(3);
        randomWave.row3Prefabs = GenerateRandomEnemyRow(3);
        return randomWave;
    }

    private GameObject[] GenerateRandomEnemyRow(int count)
    {
        GameObject[] enemyRow = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, Waves[currentWaveIndex].row1Prefabs.Length);
            enemyRow[i] = Waves[currentWaveIndex].row1Prefabs[randomIndex];
        }
        return enemyRow;
    }

    public void UpdateWaveText()
    {
        if (UIManager == null)
        {
            Debug.LogWarning("UIManager is not assigned.");
            return;
        }

        var playerStatsUI = m_Instance.GetUIManager().GetPlayerStatsUI();
        if (playerStatsUI == null)
        {
            Debug.LogWarning("PlayerStatsUI is not found in UIManager.");
            return;
        }

        playerStatsUI.UpdateWaveText(currentWaveIndex + 1);
    }


    public void SaveData()
    {
        Debug.Log("Saving Data...");
        string playerData = JsonUtility.ToJson(Player.GetComponent<Character>().GetCharacterStats());
        string filePath = Application.persistentDataPath + "/PlayerData.Json";
        System.IO.File.WriteAllText(filePath, playerData);
        Debug.Log("Save Complete!");
    }

    public void LoadData()
    {
        Debug.Log("Loading Data...");
        string filePath = Application.persistentDataPath + "/PlayerData.Json";
        if (System.IO.File.Exists(filePath))
        {
            string playerData = System.IO.File.ReadAllText(filePath);
            JsonUtility.FromJsonOverwrite(playerData, Player.GetComponent<Character>().GetCharacterStats());
            Debug.Log("Load Complete!");
        }
    }
}
