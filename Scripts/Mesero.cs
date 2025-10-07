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

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        carriedObject.SetActive(false);

        pizzaCollider = carriedObject.GetComponent<PizzaCollider>();
        if (pizzaCollider != null)
            pizzaCollider.mesero = this;

        OnReachReception = GoToReception;
        OnReceivePizza = GoToClient;
        OnReturnToReception = GoToReception;

        OnReachReception?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
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
            OnReceivePizza?.Invoke();
        }
        else
        {
            OnReturnToReception?.Invoke();
        }
    }

    private void GoToClient()
    {
        if (targetClient != null)
        {
            agent.SetDestination(targetClient.transform.position);
        }
    }

    public void DeliverPizzaToClient()
    {
        carriedObject.SetActive(false);
        if (pizzaCollider != null)
            pizzaCollider.enabled = false;

        if (targetClient != null)
        {
            assignedClients.Remove(targetClient);
            targetClient = null;
        }

        OnReturnToReception?.Invoke();
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