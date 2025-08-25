using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public enum ClientStates
{
    WAITING,
    GOING_TO_TABLE,
    THINKING,
    ORDERING,
    EATING,
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
    public Transform exit;

    public FoodSO pedido;
    public float noOrder;

    public Table targetTable;

    private float thingking = 0f;
    public float timeToThing = 6f;

    public GameObject prefab;

    public GameObject floatingUIPrefab;
    private TMP_Text floatingText;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        noOrder = Random.Range(1, 4);

        GameObject uiInstance = Instantiate(floatingUIPrefab, transform.position + Vector3.up * 2f, Quaternion.identity, transform);
        floatingText = uiInstance.GetComponentInChildren<TMP_Text>();

        UpdateFloatingText();
    }

    public void ChooseTable(Table table)
    {
        if(table != null)
        {
            targetTable = table;
            target = table.chair;
            client.state = ClientStates.GOING_TO_TABLE;
            agent.SetDestination(target.position);
        }
        else
        {
            // esperar
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
            noOrder--; // Resta una pizza entregada

            Destroy(other.gameObject);

            UpdateFloatingText();

            if (noOrder <= 0)
            {
                client.state = ClientStates.EATING;
                thingking = 0f;
                anim.SetBool("order", true);
                Debug.Log($"{client.nameClient} recibió todas las pizzas y empieza a comer.");
            }
            else
            {
                Debug.Log($"{client.nameClient} aún espera {noOrder} pizza(s).");
            }
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
            // Come y luego se va
            case ClientStates.EATING:
                if (thingking < timeToThing)
                {
                    thingking += 1 * Time.deltaTime;
                    if (thingking >= timeToThing)
                    {
                        client.state = ClientStates.LEAVING;
                        target = GameManager.instance.spawnPoint;
                        agent.SetDestination(target.position);
                        StartCoroutine(Bye());
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
                Debug.Log("Eat and Bye");
                break;
        }
    }

    private void UpdateFloatingText()
    {
        if (floatingText != null)
        {
            floatingText.text = $"x{noOrder}";
        }
    }

    // Se elimina al cliente luego de unos segundos y se desocupa la mesa
    public IEnumerator Bye()
    {
        yield return new WaitForSeconds(5f);
        GameManager.instance.RemoveClient(this);
        Destroy(gameObject);
        targetTable.isOccupied = false;
    }
}