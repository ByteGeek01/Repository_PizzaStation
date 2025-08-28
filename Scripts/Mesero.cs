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
    }

    private void OnTriggerEnter(Collider other)
    {
        // Si colisiona con Pizza
        if (other.CompareTag("Pizza"))
        {
            //Destroy(other.gameObject);
            carriedObject.SetActive(true);
        }
    }
}
