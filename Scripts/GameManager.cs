using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<Table> tables;
    public List<Client> clients;
    public List<FoodSO> menu;

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

    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        StartCoroutine(ClientSpawner());
    }

    public Client CreateClient()
    {
        /*
        Client clt = Instantiate(clientObject, spawnPoint.position, Quaternion.identity).GetComponent<Client>();
        clients.Add(clt);
        clt.pedido = RandomChooseFromMenu();
        return clt;
        */
        // Crea al cliente en su punto de spawn
        GameObject go = Instantiate(clientObject, spawnPoint.position, Quaternion.identity);
        Client clt = go.GetComponent<Client>();
        clients.Add(clt);
        //clt.pedido = RandomChooseFromMenu();
        return clt;
    }

    public Table GetFreeTable()
    {
        // Lleva a la mesa desocupada mas desocupada
        foreach (var table in tables)
        {
            if (!table.isOccupied)
            {
                table.isOccupied = true;
                return table;
            }
        }
        Debug.Log("No hay mesas");
        return null;
        /*
        for (int i = 0; i < tables.Count; i++)
        {
            if (!tables[i].isOccupied)
            {
                tables[i].isOccupied = true;
                return tables[i];
            }
        }
        Debug.Log ("No hay");
        return null;
        */
    }

    public void SetTableForClient(Client client)
    {
        //client.ChooseTable(GetFreeTable());
        // Asigna la mesa
        Table table = GetFreeTable();
        if (table != null)
        {
            client.ChooseTable(table);
        }
        else
        {
            Debug.Log("Cliente no pudo sentarse");
        }
    }

    
    public void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SetTableForClient(CreateClient());
        }
        */
        // Ingredientes spawneados
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

        // Si ya llega a llenarse, debe vaciarse para volver a spawnear
        if (!isSpawning && clients.Count == 0)
        {
            numberWave = 0;
            isSpawning = true;
            StartCoroutine(ClientSpawner());
        }
    }
    
    // Comida del menu
    /*
    public FoodSO RandomChooseFromMenu()
    {
        return menu[Random.Range(0, menu.Count)];
    }
    */

    // Borra al cliente una vez termina de comer y camina a la entrada
    public void RemoveClient(Client client)
    {
        if (clients.Contains(client))
        {
            clients.Remove(client);
        }
    }

    // Corrutina para el intervalo de spawn
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
}
