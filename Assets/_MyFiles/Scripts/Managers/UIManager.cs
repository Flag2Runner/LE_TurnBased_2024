using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject iventoryPrefabUI;
    [SerializeField] private GameObject shopPrefabUI;
    [SerializeField] private GameObject playerStatsPrefabUI;
    [SerializeField] private GameObject choicePrefabUI;

    private GameObject _inventoryUI;
    private GameObject _shopUI;
    private GameObject _playerStatsUI;
    private GameObject _choiceUI;

    internal void UpdateUI(List<Item> items)
    {
        //throw new NotImplementedException();
    }

    private void Start()
    {
        _inventoryUI = Instantiate(iventoryPrefabUI, this.transform, false);
        _shopUI = Instantiate(shopPrefabUI, this.transform, false);
        _playerStatsUI = Instantiate(playerStatsPrefabUI, this.transform, false);
        _choiceUI = Instantiate(choicePrefabUI, this.transform, false);
    }
}
