using UnityEngine;

public class PlayerBattleActions : MonoBehaviour, IBattleActions
{
    public void Attack(Character TargetToAttack)
    {
        Debug.Log($"{this.name} Attacks {TargetToAttack.name}");
    }

    public void Gaurd()
    {
        throw new System.NotImplementedException();
    }

    public void Heal()
    {
        Debug.Log($"{this.name} Heals {this.name}");
    }

    public void Pass()
    {
        Debug.Log($"{this.name} passes their turn.");
    }
}
