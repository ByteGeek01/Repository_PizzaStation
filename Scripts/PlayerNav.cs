using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerNav : MonoBehaviour
{
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if(input.magnitude <= 0)
        {
            return;
        }

        if(Mathf.Abs(input.y) > 0.01f)
        {
            Move(input);
        }
        else
        {
            Rotate(input);
        }
    }

    public void Move(Vector2 input)
    {
        Vector3 destination = transform.position + transform.right * input.x + transform.forward * input.y;
        agent.destination = destination;
    }
    public void Rotate(Vector2 input)
    {
        agent.destination = transform.position;
        transform.Rotate(0, input.x * agent.angularSpeed * Time.deltaTime, 0);
    }
}
