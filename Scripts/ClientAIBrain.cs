using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ClientAIBrain : MonoBehaviour
{
    private NavMeshAgent agent;
    public List<Transform> target;

    public int current;

    private Transform currentTarget;

    // Inicia el conteo de los puntos de recorrido
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if(target.Count > 0)
        {
            currentTarget = target[0];
            MoveToTarget();
        }
    }

    // Se mueve al destino
    private void MoveToTarget()
    {
        if (currentTarget != null)
        { 
            agent.SetDestination(currentTarget.position);
        }
    }

    // Cambio de destino
    private void ChangeTarget(Transform newTarget)
    {
        currentTarget = newTarget;
        MoveToTarget();
    }

    // Cuando llega a un destino de la lista, ira al que sigue
    private void Update()
    {
        if (agent.remainingDistance < agent.stoppingDistance && !agent.pathPending)
        {
            current++;
            if (current >= target.Count)
            {
                current = 0;
            }
            ChangeTarget(target[current]);
        }
    }
}
