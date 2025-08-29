using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

// Estados de cliente
public enum ClientStates
{
    WAITING,
    GOING_TO_TABLE,
    THINKING,
    ORDERING,
    EATING,
    PAYING,
    LEAVING
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

    public Animator anim;

    public Transform target;

    public Inventary inventary;
    public FoodSO pedido;
    public float noOrder;

    public Table targetTable;

    private float thingking = 0f;
    public float timeToThing = 6f;

    public Mesero mesero;

    public GameObject floatingUIPrefab;
    private TMP_Text floatingText;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

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
        switch (client.state)
        {
            // Piensa
            case ClientStates.THINKING:
                if (thingking < timeToThing)
                {
                    thingking += 1 * Time.deltaTime;
                    if(thingking >= timeToThing)
                    {
                        client.state = ClientStates.ORDERING;
                        Debug.Log("Order");
                        thingking = 0;
                        anim.SetBool("order", true);
                    }
                }
                break;

            // Ordena
            case ClientStates.ORDERING:
                anim.SetBool("order", true);
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
                        client.state = ClientStates.PAYING;
                        target = GameManager.instance.spawnPoint;
                        anim.SetBool("order", false);
                    }
                }
                /*
                client.state = ClientStates.LEAVING;
                target = GameManager.instance.spawnPoint;
                agent.SetDestination(target.position);
                prefab.SetActive(false);
                anim.SetBool("order", false);
                */
                break;
            case ClientStates.PAYING:
                StartCoroutine(Bye());
                Debug.Log("Eat and Pay");
                break;

            // Se va
            case ClientStates.LEAVING:
                agent.SetDestination(target.position);
                Debug.Log("Bye");
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
    }

    public void OrderComing()
    {
        noOrder--; // Resta hasta 0 para completar la orden

        UpdateFloatingText(); // Actualiza el estado de la orden

        mesero.carriedObject.SetActive(false);
        mesero.carriedObject.transform.parent = null;

        // Pago de cuenta
        inventary.BillPayed();

        // Si ya recibió toda la orden, come y luego se va
        if (noOrder <= 0)
        {
            client.state = ClientStates.EATING;
            thingking = 0f;
            anim.SetBool("order", true);
        }
    }

    // Se elimina al cliente luego de unos segundos y se desocupa la mesa
    public IEnumerator Bye()
    {
        yield return new WaitForSeconds(2f);
        client.state = ClientStates.LEAVING;
        //agent.SetDestination(target.position);
        yield return new WaitForSeconds(4f);
        GameManager.instance.RemoveClient(this);
        Destroy(gameObject);
        targetTable.isOccupied = false;
    }
}