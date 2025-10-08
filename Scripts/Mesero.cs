using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using System;
using System.Collections.Generic;

public class Mesero : MonoBehaviour
{
    [Header("Referencias")]
    public NavMeshAgent agent;
    public Transform reception;
    public GameObject point;
    public GameObject carriedObject; // Pizza visual
    public Inventary inventary;

    private Client targetClient;
    public Action OnReachReception;
    public Action OnReceivePizza;
    public Action OnReturnToReception;

    private static HashSet<Client> assignedClients = new HashSet<Client>();
    private PizzaCollider pizzaCollider;

    private bool returning = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        carriedObject.SetActive(false);

        pizzaCollider = carriedObject.GetComponent<PizzaCollider>();
        if (pizzaCollider != null)
            pizzaCollider.mesero = this;

        OnReachReception = GoToReception;
        OnReceivePizza = GoToClient;
        OnReturnToReception = ReturnToReception;  // GoToReception

        // Inicia yendo a recepción
        OnReachReception?.Invoke();
    }

    private void Update()
    {
        // Cuando está regresando
        if (returning && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            returning = false;
            OnReachReception?.Invoke(); // reinicia el ciclo
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Recoger pizza
        if (other.CompareTag("Pizza") && !carriedObject.activeSelf)
        {
            TakePizza(other.gameObject);
        }
    }

    private void GoToReception()
    {
        agent.SetDestination(reception.position);
    }

    private void TakePizza(GameObject pizza)
    {
        carriedObject.SetActive(true);
        if (pizzaCollider != null)
            pizzaCollider.enabled = true;

        carriedObject.transform.SetParent(point.transform);
        carriedObject.transform.localPosition = Vector3.zero;

        Destroy(pizza);

        targetClient = GetNextFreeClient();

        if (targetClient != null)
        {
            assignedClients.Add(targetClient);
            OnReceivePizza?.Invoke(); // Va al cliente
        }
        else
        {
            OnReturnToReception?.Invoke(); // Si no hay clientes, vuelve
        }
    }

    private void GoToClient()
    {
        if (targetClient != null)
        {
            agent.SetDestination(targetClient.transform.position);
        }
    }

    // llamado desde PizzaCollider al tocar al cliente
    public void DeliverPizzaToClient()
    {
        carriedObject.SetActive(false);

        GameManager.instance.RegisterPizzaEntregada();

        if (targetClient != null)
        {
            assignedClients.Remove(targetClient);
            targetClient = null;
        }

        OnReturnToReception?.Invoke();
    }

    // Regresa a recepción
    private void ReturnToReception()
    {
        returning = true;
        agent.SetDestination(reception.position);

        // Animación para dar feedback visual
        transform.DOJump(reception.position, 0.5f, 1, 1f);
    }

    private Client GetNextFreeClient()
    {
        foreach (Client client in GameManager.instance.clients)
        {
            if (client.client.state == ClientStates.ORDERING && !assignedClients.Contains(client))
                return client;
        }
        return null;
    }
}