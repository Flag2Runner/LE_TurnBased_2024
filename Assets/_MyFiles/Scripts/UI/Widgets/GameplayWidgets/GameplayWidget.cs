using UnityEngine;

public class GameplayWidget : Widget
{
    [SerializeField] private CanvasGroup gameplayControlCanvasGroup;


    public void SetGameplayControlEnabled(bool bIsEnabled)
    {
        gameplayControlCanvasGroup.blocksRaycasts = bIsEnabled;
        gameplayControlCanvasGroup.interactable = bIsEnabled;
    }

    public override void SetOwner(GameObject newOwner)
    {
        base.SetOwner(newOwner);
        Widget[] allWidgets = GetComponentsInChildren<Widget>();
        foreach (Widget childWidget in allWidgets)
        {
            if (childWidget != this)
                childWidget.SetOwner(newOwner);
        }
    }

}
