using System.Drawing;
using UnityEngine;
using UnityEngine.AI;

public class Mesero : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform reception;

    public GameObject point;
    public GameObject carriedObject;

    private Client targetClient;

    public Inventary inventary;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.SetDestination(reception.position);

        carriedObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Si colisiona con Pizza
        if (other.CompareTag("Pizza") && other.gameObject != carriedObject)
        {
            // Activar objeto visual que representa la comida
            carriedObject.SetActive(true);
            Rigidbody rb = other.attachedRigidbody;
            inventary.WaiterCost();

            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }

            carriedObject.transform.SetParent(point.transform);
            carriedObject.transform.localPosition = Vector3.zero;
            //carriedObject = other.gameObject;

            Destroy(other.gameObject);

            // Busca cliente (ORDERING)
            foreach (Client client in GameManager.instance.clients)
            {
                if (client.client.state == ClientStates.ORDERING)
                {
                    targetClient = client;
                    agent.SetDestination(targetClient.transform.position);
                    break;
                }
            }
        }

        if (other.CompareTag("Client"))
        {
            carriedObject.transform.parent = null;
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