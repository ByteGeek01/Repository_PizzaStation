using UnityEngine;
using UnityEngine.AI;

public class Mesero : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform reception;

    public GameObject carriedObject;
    private Client targetClient;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.SetDestination(reception.position);

        carriedObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Si colisiona con Pizza
        if (other.CompareTag("Pizza"))
        {
            // Activar objeto visual que representa la comida
            carriedObject.SetActive(true);

            // Buscar cliente que está esperando (estado ORDERING)
            foreach (Client client in GameManager.instance.clients)
            {
                if (client.client.state == ClientStates.ORDERING)
                {
                    targetClient = client;
                    agent.SetDestination(targetClient.transform.position);
                    Debug.Log("Mesero va hacia el cliente: " + client.name);
                    break;
                }
            }
        }
    }

    void Update()
    {
        // Si tiene un cliente objetivo
        if (targetClient != null)
        {
            float distance = Vector3.Distance(transform.position, targetClient.transform.position);
            if (distance < 1.5f)
            {
                // Entregar la comida
                carriedObject.SetActive(false); // Ocultar plato visual
                targetClient = null;

                // Volver a recepción
                agent.SetDestination(reception.position);
            }
        }
    }
}
