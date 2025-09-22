using UnityEngine;
using TMPro;

public class ClickerManager : MonoBehaviour
{
    public static ClickerManager instance;
    public TextMeshProUGUI pointsText;

    public void Awake()
    {
        instance = this;
    }

    public void AddPoints(int pointsToAdd)
    {
        ClickerDataManager.points += pointsToAdd;
        refreshUI();
    }

    public void refreshUI()
    {
        pointsText.text = "x" + ClickerDataManager.points;
    }
}
