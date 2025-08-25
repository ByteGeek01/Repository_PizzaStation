using System.Collections.Generic;
using UnityEngine;

public class TakeObject : MonoBehaviour
{
    public GameObject point;

    public GameObject pickedObject = null;

    private HashSet<string> pickableTags = new HashSet<string> { "Pizza", "Cheese", "Bread", "Meat", "Sauce" };

    // Agarra objetos con tag
    private void OnTriggerStay(Collider other)
    {
        if (pickedObject != null) return;

        if (Input.GetKey(KeyCode.E) && pickableTags.Contains(other.tag))
        {
            Rigidbody rb = other.attachedRigidbody;

            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }

            other.transform.SetParent(point.transform);
            other.transform.localPosition = Vector3.zero; // Alinea exactamente al punto
            pickedObject = other.gameObject;
        }
    }
}