using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject iventoryPrefabUI;
    [SerializeField] private GameObject shopPrefabUI;
    [SerializeField] private GameObject playerStatsPrefabUI;
    [SerializeField] private GameObject choicePrefabUI;

    [Header("[READ ONLY]")]
    [SerializeField] private GameObject _inventoryUI;
    [SerializeField] private GameObject _shopUI;
    [SerializeField] private GameObject _playerstatsBG;
    [SerializeField] private GameObject _choiceUI;

    private PlayerStatsUI playerStatsUIComponent;

    internal void UpdateUI(List<Item> items)
    {
        //throw new NotImplementedException();
    }

    private void Awake()
    {
        _inventoryUI = Instantiate(iventoryPrefabUI, this.transform, false);
        _shopUI = Instantiate(shopPrefabUI, this.transform, false);
        _playerstatsBG = Instantiate(playerStatsPrefabUI, this.transform, false);
        playerStatsUIComponent = _playerstatsBG.GetComponent<PlayerStatsUI>();
        _choiceUI = Instantiate(choicePrefabUI, this.transform, false);

        _shopUI.GetComponent<ShopUI>().SetInventoryList(GetInventoryUI().GetInventoryList());
    }


    public InventoryComponent GetInventoryUI() { return _inventoryUI.GetComponent<InventoryComponent>(); }

    public GameObject GetShopUI() { return _shopUI; }

    public GameObject GetChoiceUI() { return _choiceUI; }

    public PlayerStatsUI GetPlayerStatsUI() { return playerStatsUIComponent; }


}
