using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

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
        if (other.CompareTag("Pizza") && !carriedObject.activeSelf)
        {
            // Activa prefab que representa la comida
            carriedObject.SetActive(true);
            Rigidbody rb = other.attachedRigidbody;
            Inventary.OnEnableCashCharge?.Invoke();

            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }

            carriedObject.transform.SetParent(point.transform);
            carriedObject.transform.localPosition = Vector3.zero;

            Destroy(other.gameObject);

            // Buscar el primer cliente de la lista
            if (GameManager.instance.clients.Count > 0)
            {
                targetClient = GameManager.instance.clients[0]; // toma el primero
                agent.SetDestination(targetClient.transform.position);
            }
        }

        if (other.CompareTag("Client") && carriedObject.activeSelf)
        {
            // Entrega la pizza
            carriedObject.SetActive(false);
            carriedObject.transform.parent = null;

            targetClient = null;
            agent.SetDestination(reception.position);
            transform.DOJump(transform.position, 1, 1, 1);
        }
    }

    void Update()
    {
        if (targetClient != null)
        {
            float distance = Vector3.Distance(transform.position, targetClient.transform.position);
            if (distance < 1.5f)
            {
                // Entrega pizza
                carriedObject.SetActive(false);
                carriedObject.transform.parent = null;
                targetClient = null;

                Inventary.OnDisableCashCharge.Invoke();

                // Vuelve al mostrador
                agent.SetDestination(reception.position);
                transform.DOJump(transform.position, 1, 1, 1);
            }
        }
    }
}