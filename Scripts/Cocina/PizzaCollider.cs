using UnityEngine;

public class PizzaCollider : MonoBehaviour
{
    public Mesero mesero;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Client"))
        {
            gameObject.SetActive(false);
        }
    }
}
