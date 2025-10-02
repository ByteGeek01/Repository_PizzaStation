using TMPro;
using UnityEngine;

public class ClientUIItem : MonoBehaviour
{
    public TextMeshProUGUI orderText;
    public TextMeshProUGUI timerText;

    private Client linkedClient;

    public void Setup(Client client)
    {
        linkedClient = client;
        UpdateOrder(client.noOrder);
        UpdateTimer(client.CountDown);
    }

    public void UpdateOrder(float orderAmount)
    {
        orderText.text = $"X{orderAmount}";
    }

    public void UpdateTimer(float time)
    {
        timerText.text = $"{Mathf.Ceil(time)}s";
    }
}
