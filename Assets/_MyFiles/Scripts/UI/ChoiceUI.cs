using UnityEngine;
using UnityEngine.UI;

public class ChoiceUI : MonoBehaviour
{
    [SerializeField] private Button attackButton;
    [SerializeField] private Button guardButton;
    [SerializeField] private Button passTurnButton;
    [SerializeField] private Button convertShopItemsButton;

    private void Start()
    {
        if (attackButton != null) attackButton.onClick.AddListener(Attack);
        if (guardButton != null) guardButton.onClick.AddListener(Guard);
        if (passTurnButton != null) passTurnButton.onClick.AddListener(PassTurn);
        if (convertShopItemsButton != null) convertShopItemsButton.onClick.AddListener(ConvertShopItemsToAttack);
    }

    private void Attack()
    {
        GameManager.m_Instance.GetBattleManager()?.Attack(true);
    }

    private void Guard()
    {
        GameManager.m_Instance.GetBattleManager()?.Guard();
    }

    private void PassTurn()
    {
        GameManager.m_Instance.GetBattleManager()?.Pass();
    }

    private void ConvertShopItemsToAttack()
    {
        GameManager.m_Instance.GetBattleManager()?.ConvertShopItemsToAttack();
    }

    public void EnableAllButtons()
    {
        attackButton.interactable = true;
        guardButton.interactable = true;
        passTurnButton.interactable = true;
        convertShopItemsButton.interactable = true;
    }

    public void DisableAllButtons()
    {
        attackButton.interactable = false;
        guardButton.interactable = false;
        passTurnButton.interactable = false;
        convertShopItemsButton.interactable = false;
    }
}
