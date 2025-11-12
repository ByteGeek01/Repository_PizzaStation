using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public enum ClientStates
{
    WAITING,
    GOING_TO_TABLE,
    THINKING,
    ORDERING,
    EATING,
    PAYING,
    LEAVING,
    NO_EATEN
}

[System.Serializable]
public class ClientClass
{
    public string nameClient;
    public ClientStates state = ClientStates.WAITING;
}

public class Client : MonoBehaviour
{
    public ClientClass client;
    public NavMeshAgent agent;
    public Transform target;
    public Inventary inventary;
    public Table targetTable;
    public Mesero mesero;

    [Header("Orden")]
    public float noOrder;
    private float thingking = 0f;
    public float timeToThing = 6f;

    [Header("UI")]
    public GameObject floatingUIPrefab;
    private TMP_Text floatingText;
    public GameObject clientUIPrefab;
    private ClientUIItem uiItem;

    [Header("Tiempo de espera")]
    public float CountDown = 180f;
    private float initialCountdown;
    private bool timer = false;
    private bool hasJumpedHalf = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            inventary = player.GetComponent<Inventary>();

        GameObject waiter = GameObject.FindWithTag("Waiter");
        if (waiter != null)
            mesero = waiter.GetComponent<Mesero>();

        noOrder = Random.Range(1, 4);
        CountDown = Random.Range(120f, 240f);
        initialCountdown = CountDown;

        // UI flotante
        GameObject uiInstance = Instantiate(floatingUIPrefab, transform.position + Vector3.up * 2f, Quaternion.identity, transform);
        floatingText = uiInstance.GetComponentInChildren<TMP_Text>();
        UpdateFloatingText();

        // UI global
        if (clientUIPrefab != null && UIManager.instance != null)
        {
            GameObject item = Instantiate(clientUIPrefab, UIManager.instance.clientsPanel);
            uiItem = item.GetComponent<ClientUIItem>();
            uiItem.Setup(this);
        }
    }

    public void ChooseTable(Table table)
    {
        if (table != null)
        {
            targetTable = table;
            target = table.chair;
            client.state = ClientStates.GOING_TO_TABLE;
            agent.SetDestination(target.position);
            hasJumpedHalf = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Llega a su mesa
        if (other.GetComponent<Table>() == targetTable && client.state == ClientStates.GOING_TO_TABLE)
        {
            client.state = ClientStates.THINKING;
            StartCoroutine(StartCountdown());
        }

        // Recibe pizza
        if (other.CompareTag("Pizza") && client.state == ClientStates.ORDERING)
        {
            OrderComing();
        }
    }

    void Update()
    {
        switch (client.state)
        {
            case ClientStates.THINKING:
                thingking += Time.deltaTime;
                if (thingking >= timeToThing)
                {
                    thingking = 0;
                    transform.DOJump(transform.position, 1.5f, 1, 1);
                    client.state = ClientStates.ORDERING;
                    Debug.Log($"{client.nameClient} está listo para ordenar.");
                }
                break;

            case ClientStates.ORDERING:
                Debug.Log($"{client.nameClient} espera su pedido.");
                break;

            case ClientStates.EATING:
                thingking += Time.deltaTime;
                if (thingking >= timeToThing)
                {
                    client.state = ClientStates.LEAVING;
                    target = GameManager.instance.spawnPoint;
                    agent.SetDestination(target.position);
                    StartCoroutine(Bye());
                }
                break;

            case ClientStates.LEAVING:
                agent.SetDestination(target.position);
                break;

            case ClientStates.NO_EATEN:
                HandleUnhappyClient();
                break;
        }
    }

    private IEnumerator StartCountdown()
    {
        timer = true;
        Debug.Log($"{client.nameClient} comenzó su cuenta regresiva ({CountDown} segundos).");

        while (timer && CountDown > 0)
        {
            yield return new WaitForSeconds(1f);
            CountDown--;

            // Actualiza UI global
            if (uiItem != null)
                uiItem.UpdateTimer(CountDown);

            // Impaciente
            if (!hasJumpedHalf && CountDown <= initialCountdown / 2f)
            {
                hasJumpedHalf = true;
                transform.DOJump(transform.position, 1.5f, 1, 1);
                Debug.Log($"{client.nameClient} está impaciente!");
            }

            if (CountDown <= 0)
            {
                client.state = ClientStates.NO_EATEN;
                timer = false;
                yield break;
            }
        }
    }

    private void HandleUnhappyClient()
    {
        Debug.Log($"{client.nameClient} se fue sin comer 😡");
        GameManager.instance.unhappyClients++;
        agent.isStopped = true;

        Sequence sadSeq = DOTween.Sequence();
        sadSeq.Append(transform.DOLocalRotate(new Vector3(0, 10, 0), 0.5f).SetRelative(true))
              .Append(transform.DOLocalRotate(new Vector3(0, -20, 0), 1f).SetRelative(true))
              .Append(transform.DOLocalRotate(new Vector3(0, 10, 0), 0.5f).SetRelative(true))
              .OnComplete(() =>
              {
                  agent.isStopped = false;
                  client.state = ClientStates.LEAVING;
                  target = GameManager.instance.spawnPoint;
                  agent.SetDestination(target.position);
                  StartCoroutine(Bye());
              });

        RemoveUI();
    }

    public void OrderComing()
    {
        noOrder--;
        UpdateFloatingText();

        if (mesero != null && mesero.carriedObject != null)
        {
            mesero.carriedObject.SetActive(false);
            mesero.carriedObject.transform.parent = null;
        }

        Inventary.OnDisableCashCharge?.Invoke();

        if (noOrder <= 0)
        {
            timer = false;
            client.state = ClientStates.EATING;
            thingking = 0f;
            transform.DOJump(transform.position, 1.5f, 1, 1);
            RemoveUI();
        }
    }

    private void UpdateFloatingText()
    {
        if (floatingText != null)
            floatingText.text = $"x{noOrder}";

        if (uiItem != null)
            uiItem.UpdateOrder(noOrder);
    }

    private void RemoveUI()
    {
        if (uiItem != null)
        {
            Destroy(uiItem.gameObject);
            uiItem = null;
        }
    }

    private IEnumerator Bye()
    {
        yield return new WaitForSeconds(2f);
        client.state = ClientStates.LEAVING;
        yield return new WaitForSeconds(3f);
        GameManager.instance.RemoveClient(this);
        Destroy(gameObject);
        targetTable.isOccupied = false;
    }
}