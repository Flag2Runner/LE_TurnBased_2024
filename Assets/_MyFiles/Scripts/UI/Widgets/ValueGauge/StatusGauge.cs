using UnityEngine;

public class StatusGauage : Widget
{
    [SerializeField] private ValueGauge healthBar;
    [SerializeField] private ValueGauge manaBar;

    public override void SetOwner(GameObject newOwner)
    {
        base.SetOwner(newOwner);
        HealthComponent ownerHealthComp = newOwner.GetComponent<HealthComponent>();
        if (ownerHealthComp)
        {

        }
    }
}