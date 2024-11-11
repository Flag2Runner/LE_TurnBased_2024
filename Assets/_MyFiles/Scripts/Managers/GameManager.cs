using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Wave 
{
    [SerializeField] private GameObject[] enemys;
}

public class GameManager : MonoBehaviour
{
    public static GameManager m_Instance;

    public static GameManager Instance
    {
        get;
        private set;
    }
    
    [Header("Player Info")]
    [SerializeField] GameObject PlayerPrefab;
    [SerializeField] Transform PlayerSpawn;
    private GameObject Player;
    [Header("Wave Data")]
    [SerializeField] Wave[] Waves;
    [Header("UI")]
    [SerializeField] private UIManager UIManager;
    [SerializeField] GameObject InventorySlotUI;

    [Header("Managers [READ ONLY]")]
    [SerializeField] private BattleManager CurrentBattle;

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
            //Spawning Player
            GameObject playerGrp = Instantiate(PlayerPrefab, PlayerSpawn.transform.position, PlayerSpawn.transform.rotation);
            Player = playerGrp.GetComponentInChildren<Character>().gameObject;
            LoadData();

        }
        else
        {
            Debug.LogWarning("Player Prefab or PlayerSpawn not Referanced");
        }
    }
    public GameObject GetPlayer() { return Player; }

    public void DestroyInventoryUIManager()
    {
        Destroy(UIManager);
        UIManager = null;
    }
    public UIManager GetUIManager() { return UIManager; }
    public void CreateBattleManager(List<GameObject> enemyBattleList)
    {
        if (CurrentBattle) { return; }


        Debug.Log("Creating BattleManager...");
        CurrentBattle = gameObject.AddComponent<BattleManager>();
        CurrentBattle.InitializeBattle(enemyBattleList);

    }
    public void DestroyBattleManager()
    {
        Destroy(CurrentBattle);
        CurrentBattle = null;
    } 
    public void EndTurnManager()
    {
        CurrentBattle.EndTurn();
    }
    public BattleManager GetBattleManager() { return CurrentBattle; }

    public void SaveData()
    {
        Debug.Log("Saving Data...");
        string playerData = JsonUtility.ToJson(Player.GetComponent<Character>().GetCharacterStats());
        string filePath = Application.persistentDataPath + "/PlayerData.Json";
        Debug.Log(filePath);
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
