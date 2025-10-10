using System.Collections.Generic;
using UnityEngine;

public class TakeObject : MonoBehaviour
{
    public Dictionary<string, int> inventaryList = new Dictionary<string, int>();

    public GameObject[] point;

    public GameObject[] pickedObject;

    public GameObject[] InventaryUI;

    private HashSet<string> pickableTags = new HashSet<string> { "Pizza", "Cheese", "Bread", "Meat", "Sauce", "Waiter" };

    private Inventary inventary;

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            inventary = player.GetComponent<Inventary>();
        }

        pickedObject = new GameObject[point.Length];

        // Inicializa los ingredientes en el diccionario
        string[] ingredients = { "Bread", "Sauce", "Cheese", "Meat", "Pizza" };
        foreach (string item in ingredients)
        {
            if (!inventaryList.ContainsKey(item))
                inventaryList[item] = 0;
        }
    }


    private void OnTriggerStay(Collider other)
    {
        // Si se presiona la tecla E y el objeto tiene un tag de la lista
        if (Input.GetKey(KeyCode.E) && pickableTags.Contains(other.tag))
        {
            if (IsAlreadyPicked(other.gameObject)) return;

            // Busca la primera posicion libre en el array
            int index = GetFirstFreeSlot();

            if (index == -1) return;

            pickedObject[index] = other.gameObject;
            if (inventaryList.ContainsKey(other.tag))
            {
                inventaryList[other.tag]++;
            }
            else
            {
                inventaryList[other.tag] = 1;
            }

            // Si no es pizza, se cobra
            if (other.tag != "Pizza")
            {
                inventary.SubtractCash(5);

                if (inventary.GetCash() <= 0)
                {
                    inventary.AddCash(0); // no cambia nada, solo mantiene consistencia
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

        if (other.CompareTag("Bread"))
        {
            InventaryUI[0].SetActive(true);
        }
        if (other.CompareTag("Sauce"))
        {
            InventaryUI[1].SetActive(true);
        }
        if (other.CompareTag("Cheese"))
        {
            InventaryUI[2].SetActive(true);
        }
        if (other.CompareTag("Meat"))
        {
            InventaryUI[3].SetActive(true);
        }
    }

    public bool HasIngredientsForRecipe()
    {
        return inventaryList.ContainsKey("Bread") && inventaryList["Bread"] >= 1 &&
           inventaryList.ContainsKey("Sauce") && inventaryList["Sauce"] >= 1 &&
           inventaryList.ContainsKey("Cheese") && inventaryList["Cheese"] >= 1 &&
           inventaryList.ContainsKey("Meat") && inventaryList["Meat"] >= 1;
    }

    // Devuelve el primer index del array
    private int GetFirstFreeSlot()
    {
        for (int i = 0; i < pickedObject.Length; i++)
        {
            if (pickedObject[i] == null)
                return i;
        }
        return -1;
    }

    // Revisa si ya ha recogido el objeto
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