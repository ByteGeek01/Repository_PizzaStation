using UnityEngine;
using UnityEngine.UI;

public class BlockedIfResourcesIsntEnough : MonoBehaviour
{
    private Button button;

    public int cost = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button = GetComponent<Button>();
        ClickerManager.instance.minionClicked += UpdateButtonState;
        UpdateButtonState();
    }

    public void UpdateButtonState()
    {
        button.interactable = ClickerDataManager.points >= cost;
    }
}
