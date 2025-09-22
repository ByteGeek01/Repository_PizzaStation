using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ClickerManager : MonoBehaviour
{
    public static ClickerManager instance;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI minionsText;

    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        refreshUI();
    }

    public void AddPoints(int pointsToAdd)
    {
        ClickerDataManager.points += pointsToAdd;
        refreshUI();
    }

    public void AddMinions(int minionToAdd)
    {
        ClickerDataManager.minions += minionToAdd;
        refreshUI();
    }

    public void BuyMinion(int cost)
    {
        if(ClickerDataManager.points >= cost)
        {
            AddPoints(-cost);
            AddMinions(1);
        }
    }

    public void refreshUI()
    {
        pointsText.text = "x" + ClickerDataManager.points;
        minionsText.text = "x" + ClickerDataManager.minions;
    }

    public void GoToGame(string miniGameScene)
    {
        SceneManager.LoadScene(miniGameScene);
    }
}
