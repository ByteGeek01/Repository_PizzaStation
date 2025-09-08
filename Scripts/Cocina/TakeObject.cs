using System.Collections.Generic;
using UnityEngine;

public class TakeObject : MonoBehaviour
{
    public GameObject[] point;

    public GameObject[] pickedObject;

    private HashSet<string> pickableTags = new HashSet<string> { "Pizza", "Cheese", "Bread", "Meat", "Sauce", "Waiter" };

    private Inventary inventary;

    public void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            inventary = player.GetComponent<Inventary>();
        }

        // Array de objetos recogidos al mismo tamaño que los puntos
        pickedObject = new GameObject[point.Length];
    }

    private void OnTriggerStay(Collider other)
    {
        // Si se presiona la tecla E y el objeto tiene un tag de la lista
        if (Input.GetKey(KeyCode.E) && pickableTags.Contains(other.tag))
        {
            if (IsAlreadyPicked(other.gameObject)) return;

            // Busca la primera posición libre en el array
            int index = GetFirstFreeSlot();

            if (index == -1) return;

            // Si no es pizza, se cobra
            if (other.tag != "Pizza")
            {
                inventary.CostIngredients();

                if (inventary.cash <= 0)
                {
                    inventary.cash = 0;
                    inventary.use = false;
                    return;
                }
            }

            Rigidbody rb = other.attachedRigidbody;

            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }

            // Poner el objeto dentro de los puntos de agarre
            other.transform.SetParent(point[index].transform);
            other.transform.localPosition = Vector3.zero;

            // Registra el objeto en la lista
            pickedObject[index] = other.gameObject;
        }
    }

    // Devuelve el primer index libre del array
    private int GetFirstFreeSlot()
    {
        for (int i = 0; i < pickedObject.Length; i++)
        {
            if (pickedObject[i] == null)
                return i;
        }
        return -1;
    }

    // Revisa si ya se ha recogido ese objeto
    private bool IsAlreadyPicked(GameObject obj)
    {
        foreach (var picked in pickedObject)
        {
            if (picked == obj)
                return true;
        }
        return false;
    }
}