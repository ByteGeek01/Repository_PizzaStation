using System.Collections.Generic;
using UnityEngine;

public class TakeObject : MonoBehaviour
{
    [Header("Inventario interno del jugador")]
    public Dictionary<string, int> inventaryList = new Dictionary<string, int>();

    [Header("Puntos donde se colocan los objetos recogidos")]
    public GameObject[] point;

    [Header("Referencia visual del inventario (íconos, UI)")]
    public GameObject[] InventaryUI;

    private GameObject[] pickedObject;
    private Inventary inventary;

    // Solo estos tags pueden ser recogidos
    private readonly HashSet<string> pickableTags = new HashSet<string>
    {
        "Pizza", "Cheese", "Bread", "Meat", "Sauce", "Waiter"
    };

    // Costos por ingrediente (editable fácilmente)
    private readonly Dictionary<string, int> ingredientCosts = new Dictionary<string, int>
    {
        { "Bread", 5 },
        { "Sauce", 5 },
        { "Cheese", 5 },
        { "Meat", 5 },
        { "Pizza", 0 }, // no se cobra
        { "Waiter", 0 }
    };

    private void Start()
    {
        // Referencia al componente Inventary del jugador
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            inventary = player.GetComponent<Inventary>();

        pickedObject = new GameObject[point.Length];

        // Inicializa el diccionario de ingredientes
        foreach (string item in pickableTags)
        {
            if (!inventaryList.ContainsKey(item))
                inventaryList[item] = 0;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Si no es un objeto válido o no se presiona E, salimos
        if (!Input.GetKey(KeyCode.E) || !pickableTags.Contains(other.tag))
            return;

        if (IsAlreadyPicked(other.gameObject)) return;

        int index = GetFirstFreeSlot();
        if (index == -1) return;

        string tag = other.tag;
        int cost = ingredientCosts.ContainsKey(tag) ? ingredientCosts[tag] : 0;

        // Cobro si no es Pizza
        if (cost > 0)
        {
            if (inventary.GetCash() >= cost)
                inventary.SubtractCash(cost);
            else
                return; // sin dinero suficiente
        }

        // Registra en el diccionario local
        inventaryList[tag]++;
        Debug.Log($"Obtuviste: {tag} (Total: {inventaryList[tag]})");

        // También se podría actualizar el inventario global si lo necesitás
        inventary.AddIngredient(tag);

        // Ajusta físicas del objeto
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        // Lo coloca en la mano o posición de recogida
        other.transform.SetParent(point[index].transform);
        other.transform.localPosition = Vector3.zero;
        pickedObject[index] = other.gameObject;

        // Muestra en la UI si existe
        UpdateIngredientUI(tag);
    }

    private void UpdateIngredientUI(string tag)
    {
        switch (tag)
        {
            case "Bread": InventaryUI[0].SetActive(true); break;
            case "Sauce": InventaryUI[1].SetActive(true); break;
            case "Cheese": InventaryUI[2].SetActive(true); break;
            case "Meat": InventaryUI[3].SetActive(true); break;
        }
    }

    // === UTILIDADES ===

    private int GetFirstFreeSlot()
    {
        for (int i = 0; i < pickedObject.Length; i++)
        {
            if (pickedObject[i] == null)
                return i;
        }
        return -1;
    }

    private bool IsAlreadyPicked(GameObject obj)
    {
        foreach (var picked in pickedObject)
        {
            if (picked == obj)
                return true;
        }
        return false;
    }

    // ✅ Verifica si se tienen los ingredientes necesarios
    public bool HasIngredientsForRecipe()
    {
        return inventaryList["Bread"] >= 1 &&
               inventaryList["Sauce"] >= 1 &&
               inventaryList["Cheese"] >= 1 &&
               inventaryList["Meat"] >= 1;
    }

    // ✅ Método auxiliar si querés vaciar inventario (por ejemplo, tras cocinar)
    public void ClearIngredients()
    {
        List<string> keys = new List<string>(inventaryList.Keys);
        foreach (var key in keys)
            inventaryList[key] = 0;
    }
}