using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<Table> tables;
    public List<Client> clients;

    public GameObject[] element;
    public Transform[] spawnElement;
    public Oven oven;
    public bool[] Spawned = new bool[4];

    public GameObject clientObject;
    public Transform spawnPoint;

    public int numberWave;
    public float spawnInterval = 10f;
    public int maxClients = 6;
    private bool isSpawning = true;

    public int unhappyClients = 0;
    public GameObject lose;

    public Inventary inventary;
    private bool dayEnded = false;

    // Contador de pizzas entregadas
    public int pizzasEntregadas = 0;

    private HashSet<Client> unhappyClientSet = new HashSet<Client>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartCoroutine(ClientSpawner());
    }

    private void Update()
    {
        // Limpiar referencias nulas
        clients.RemoveAll(c => c == null);

        // Spawnear ingredientes
        for (int i = 0; i < 4; i++)
        {
            if (oven.ingredients[i] && !Spawned[i])
            {
                Instantiate(element[i], spawnElement[i].position, Quaternion.identity);
                Spawned[i] = true;
            }
            if (!oven.ingredients[i])
            {
                Spawned[i] = false;
            }
        }

        // 🔹 Si ya no se spawnean más clientes y todos se fueron
        if (!isSpawning && clients.Count == 0 && !dayEnded)
        {
            dayEnded = true;
            EndOfDay();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndOfDay();
        }
    }

    private IEnumerator ClientSpawner()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (numberWave >= 6)
            {
                isSpawning = false;
                yield break;
            }

            if (clients.Count < maxClients)
            {
                numberWave++;
                Client newClient = CreateClient();
                SetTableForClient(newClient);
            }
        }
    }

    public Client CreateClient()
    {
        GameObject go = Instantiate(clientObject, spawnPoint.position, Quaternion.identity);
        Client clt = go.GetComponent<Client>();
        clients.Add(clt);
        return clt;
    }

    public void RemoveClient(Client client)
    {
        if (clients.Contains(client))
            clients.Remove(client);
    }

    public void SetTableForClient(Client client)
    {
        Table table = GetFreeTable();
        if (table != null)
            client.ChooseTable(table);
    }

    public Table GetFreeTable()
    {
        foreach (var table in tables)
        {
            if (!table.isOccupied)
            {
                table.isOccupied = true;
                return table;
            }
        }
        return null;
    }

    // 🔹 Llamar desde el mesero cuando entrega una pizza
    public void RegisterPizzaEntregada()
    {
        Inventary.OnDisableCashCharge?.Invoke();
    }

    public void RegisterUnhappyClient(Client client)
    {
        if (!unhappyClientSet.Contains(client))
        {
            unhappyClients++;
            unhappyClientSet.Add(client);

            Debug.Log($"Cliente molesto agregado. Total: {unhappyClients}");

            if (unhappyClients >= 6)
            {
                GameOver();
            }
        }
    }

    private void GameOver()
    {
        if (lose != null)
        {
            lose.SetActive(true);
            Debug.Log("GAME OVER - Demasiados clientes molestos!");
        }
    }

    public void EndOfDay()
    {
        Debug.Log("Fin del día: todos los clientes fueron atendidos.");

        // Guardar estadísticas del día en PlayerPrefs
        if (inventary != null)
        {
            PlayerPrefs.SetInt("Cash", inventary.GetCash());
            PlayerPrefs.SetInt("PizzasDelivered", inventary.pizzasEntregadas);
            PlayerPrefs.SetInt("UnhappyClients", inventary.clientesMolestos);
            PlayerPrefs.Save();
        }

        // Cambiar de escena a Ganancias
        StartCoroutine(LoadResultsScene());
    }

    private IEnumerator LoadResultsScene()
    {
        yield return new WaitForSeconds(1f); // Pequeño delay opcional
        SceneManager.LoadScene("Ganancia");
    }
}