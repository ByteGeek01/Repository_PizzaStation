using System.Collections.Generic;
using UnityEngine;

public class TakeObject : MonoBehaviour
{
    public GameObject point;

    public GameObject pickedObject = null;

    private HashSet<string> pickableTags = new HashSet<string> { "Pizza", "Cheese", "Bread", "Meat", "Sauce", "Waiter" };

    private Inventary inventary;

    public void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            inventary = player.GetComponent<Inventary>();
        }
    }

    // Objetos con tag
    private void OnTriggerStay(Collider other)
    {
        if (pickedObject != null) return;

        // Toma los objetos con los tags especificos y cobra por ingredientes
        if (Input.GetKey(KeyCode.E) && pickableTags.Contains(other.tag))
        {
            if(other.tag != "Pizza")
            {
                inventary.CostIngredients();

                if(inventary.cash <= 0)
                {
                    inventary.cash = 0;
                    return;
                }
            }
            Rigidbody rb = other.attachedRigidbody;

            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }

            other.transform.SetParent(point.transform);
            other.transform.localPosition = Vector3.zero;
            pickedObject = other.gameObject;
        }
    }
}