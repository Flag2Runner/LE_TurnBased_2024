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
        // Assign the button listeners to the GameManager's methods
        if (attackButton != null) attackButton.onClick.AddListener(Attack);
        if (guardButton != null) guardButton.onClick.AddListener(Guard);
        if (passTurnButton != null) passTurnButton.onClick.AddListener(PassTurn);
        if (convertShopItemsButton != null) convertShopItemsButton.onClick.AddListener(ConvertShopItemsToAttack);
    }

    private void Attack()
    {
        GameManager.Instance.GetBattleManager()?.Attack();
    }

    private void Guard()
    {
        GameManager.Instance.GetBattleManager()?.Guard();
    }

    private void PassTurn()
    {
        GameManager.Instance.EndTurnManager();
    }

    private void ConvertShopItemsToAttack()
    {
        // Ensure that the BattleManager exists before calling this
        GameManager.Instance.GetBattleManager()?.ConvertShopItemsToAttack();
    }
}
