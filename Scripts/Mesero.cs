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
        // Si colisiona con Pizza y no lleva nada
        if (other.CompareTag("Pizza") && !carriedObject.activeSelf)
        {
            // Activa el objeto que siempre lleva
            carriedObject.SetActive(true);
            carriedObject.transform.SetParent(point.transform);
            carriedObject.transform.localPosition = Vector3.zero;

            Rigidbody rb = other.attachedRigidbody;
            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }

            // Desactiva la pizza de la escena (no destruirla)
            other.gameObject.SetActive(false);

            Inventary.OnEnableCashCharge?.Invoke();

            // Buscar el primer cliente de la lista
            if (GameManager.instance.clients.Count > 0)
            {
                targetClient = GameManager.instance.clients[0];
                agent.SetDestination(targetClient.transform.position);
            }
        }

        // Si llega al cliente y lleva la pizza
        if (other.CompareTag("Client") && carriedObject.activeSelf)
        {
            // Entrega la pizza: solo desactiva carriedObject
            carriedObject.SetActive(false);
            carriedObject.transform.parent = null;

            targetClient = null;
            agent.SetDestination(reception.position);
            transform.DOJump(transform.position, 1, 1, 1);

            Inventary.OnDisableCashCharge?.Invoke();
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