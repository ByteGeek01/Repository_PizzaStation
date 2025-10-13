using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

// Estados de cliente
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

    //public Animator anim;

    public Transform target;

    public Inventary inventary;

    public float noOrder;

    public Table targetTable;

    private float thingking = 0f;
    public float timeToThing = 6f;

    public Mesero mesero;

    public GameObject floatingUIPrefab;
    private TMP_Text floatingText;

    public GameObject clientUIPrefab;
    private ClientUIItem uiItem;

    private bool timer = false;
    public float CountDown = 200f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //anim = GetComponent<Animator>();

        GameObject player = GameObject.FindWithTag("Player");
        if(player != null)
        {
            inventary = player.GetComponent<Inventary>();
        }

        GameObject waiter = GameObject.FindWithTag("Waiter");
        if (waiter != null)
        {
            mesero = waiter.GetComponent<Mesero>();
        }

        noOrder = Random.Range(1, 4);

        GameObject uiInstance = Instantiate(floatingUIPrefab, transform.position + Vector3.up * 2f, Quaternion.identity, transform);
        floatingText = uiInstance.GetComponentInChildren<TMP_Text>();

        UpdateFloatingText();

        // 🔹 Instanciar item en el panel global
        if (clientUIPrefab != null && UIManager.instance != null)
        {
            GameObject item = Instantiate(clientUIPrefab, UIManager.instance.clientsPanel);
            uiItem = item.GetComponent<ClientUIItem>();
            uiItem.Setup(this); // le pasamos el cliente
        }
    }

    // Elige la mesa
    public void ChooseTable(Table table)
    {
        if(table != null)
        {
            targetTable = table;
            target = table.chair;
            client.state = ClientStates.GOING_TO_TABLE;
            agent.SetDestination(target.position);
            timer = true;
            CountDown = 200;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        // Condiciones para cambiar de estados
        if (other.GetComponent<Table>() == targetTable && client.state == ClientStates.GOING_TO_TABLE)
        {
            client.state = ClientStates.THINKING;
        }
        // Si recibió su pedido
        if (other.CompareTag("Pizza") && client.state == ClientStates.ORDERING)
        {
            OrderComing();
        }
    }

    void Update()
    {
        if (timer)
        {
            CountDown -= Time.deltaTime;

            // 🔹 actualizar el UI del cliente
            if (uiItem != null)
                uiItem.UpdateTimer(CountDown);

            if (CountDown <= 0)
            {
                client.state = ClientStates.NO_EATEN;
                RemoveUI();
            }
        }

        switch (client.state)
        {
            // Piensa
            case ClientStates.THINKING:
                if (thingking < timeToThing)
                {
                    thingking += 1 * Time.deltaTime;
                    if(thingking >= timeToThing)
                    {
                        transform.DOJump(transform.position, 1.5f, 1, 1);
                        client.state = ClientStates.ORDERING;
                        Debug.Log("Order");
                        thingking = 0;
                        //anim.SetBool("order", true);
                    }
                }
                break;

            // Ordena
            case ClientStates.ORDERING:
                //anim.SetBool("order", true);
                //client.state = ClientStates.EATING;
                /*
                if (thingking < timeToThing)
                {
                    thingking += 1 * Time.deltaTime;
                    if (thingking >= timeToThing)
                    {
                        anim.SetBool("order", true);
                        client.state = ClientStates.EATING;
                        thingking = 0;
                    }
                }
                */
                Debug.Log("Waiting");
                break;

            // Come y paga
            case ClientStates.EATING:
                if (thingking < timeToThing)
                {
                    thingking += 1 * Time.deltaTime;
                    if (thingking >= timeToThing)
                    {
                        client.state = ClientStates.LEAVING;
                        target = GameManager.instance.spawnPoint;
                        agent.SetDestination(target.position);
                        //anim.SetBool("order", false);

                        StartCoroutine(Bye());
                    }
                }
                break;

            case ClientStates.PAYING:
                Debug.Log("Eat and Pay");
                break;

            // Se va
            case ClientStates.LEAVING:
                agent.SetDestination(target.position);
                Debug.Log("Bye");
                break;

            case ClientStates.NO_EATEN:
                CountDown = 0;
                timer = false;

                GameManager.instance.unhappyClients++;

                Debug.Log("Mal restaurant");

                agent.isStopped = true;

                DG.Tweening.Sequence sadSeq = DOTween.Sequence();
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
                break;
        }
    }

    // Tamaño de la orden del cliente
    private void UpdateFloatingText()
    {
        if (floatingText != null)
        {
            floatingText.text = $"x{noOrder}";
        }

        // Actualiza en el panel global
        if (uiItem != null)
            uiItem.UpdateOrder(noOrder);
    }

    public void OrderComing()
    {
        noOrder--;
        UpdateFloatingText();

        mesero.carriedObject.SetActive(false);
        mesero.carriedObject.transform.parent = null;

        Inventary.OnDisableCashCharge?.Invoke();

        if (noOrder <= 0)
        {
            timer = false;
            client.state = ClientStates.EATING;
            thingking = 0f;
            //anim.SetBool("order", true);
            transform.DOJump(transform.position, 1.5f, 1, 1);

            RemoveUI();
        }
    }


    private void RemoveUI()
    {
        if (uiItem != null)
        {
            Destroy(uiItem.gameObject);
            uiItem = null;
        }
    }

    // Se elimina al cliente luego de unos segundos y se desocupa la mesa
    public IEnumerator Bye()
    {
        yield return new WaitForSeconds(2f);
        client.state = ClientStates.LEAVING;
        yield return new WaitForSeconds(3f);
        GameManager.instance.RemoveClient(this);
        Destroy(gameObject);
        targetTable.isOccupied = false;
    }
}