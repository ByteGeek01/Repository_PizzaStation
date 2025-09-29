using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClickerManager : MonoBehaviour
{
    public static ClickerManager instance;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI minionsText;

    public System.Action minionClicked;

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
        minionClicked?.Invoke();
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
