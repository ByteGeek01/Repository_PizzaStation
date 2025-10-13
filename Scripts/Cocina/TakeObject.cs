using System.Collections.Generic;
using UnityEngine;

public class TakeObject : MonoBehaviour
{
    public Dictionary<string, int> inventaryList = new Dictionary<string, int>();
    public GameObject[] point;             // puntos donde se sujetan los objetos
    public GameObject[] pickedObject;      // objetos actualmente tomados
    public GameObject[] InventaryUI;       // referencias a UI

    private HashSet<string> pickableTags = new HashSet<string>
    {
        "Pizza", "Cheese", "Bread", "Meat", "Sauce", "Waiter"
    };

    private Inventary inventary;
    private bool canPick = true;

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            inventary = player.GetComponent<Inventary>();

        pickedObject = new GameObject[point.Length];

        string[] ingredients = { "Bread", "Sauce", "Cheese", "Meat", "Pizza" };
        foreach (string item in ingredients)
        {
            if (!inventaryList.ContainsKey(item))
                inventaryList[item] = 0;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Entrega la pizza al mesero y evita que se queden objetos en el array
        if (other.CompareTag("Waiter") && HasPizzaInHands())
        {
            ClearAllHands();
            Debug.Log("🍕 Pizza entregada al mesero.");
            return;
        }

        // Recolecta ingredientes
        if (Input.GetKeyDown(KeyCode.E) && canPick && pickableTags.Contains(other.tag))
        {
            // Evita recoger el mismo ingrediente multiples veces
            if (IsAlreadyPicked(other.gameObject))
                return;

            int freeIndex = GetFirstFreeSlot();
            if (freeIndex == -1)
            {
                Debug.Log("No hay más espacio para sostener ingredientes.");
                return;
            }

            canPick = false; // evita spam de array

            string tag = other.tag;

            // Solo cobra los ingredientes y el mesero
            if (tag != "Pizza")
            {
                if (inventary.GetCash() < 5)
                {
                    Debug.Log("No hay suficiente dinero para comprar este ingrediente.");
                    return;
                }
                // Llama a action de quitar dinero
                inventary.SubtractCash(5);
            }

            // Agrega al inventario una sola vez
            if (inventaryList.ContainsKey(tag))
                inventaryList[tag]++;
            else
                inventaryList[tag] = 1;

            // Físicas y posición de los ingredientes tomados
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }

            // Coloca en el primer punto desocupado
            other.transform.SetParent(point[freeIndex].transform);
            other.transform.localPosition = Vector3.zero;
            pickedObject[freeIndex] = other.gameObject;

            UpdateUI(tag);

            StartCoroutine(ResetPickDelay());
        }
    }

    // Permite volver a tomar los ingredientes
    private System.Collections.IEnumerator ResetPickDelay()
    {
        yield return new WaitForSeconds(0.1f);
        canPick = true;
    }

    // Pizza en sus manos
    private bool HasPizzaInHands()
    {
        foreach (var obj in pickedObject)
        {
            if (obj != null && obj.CompareTag("Pizza"))
                return true;
        }
        return false;
    }

    // Limpia de referencias en array si hace falta
    private void ClearAllHands()
    {
        for (int i = 0; i < pickedObject.Length; i++)
        {
            if (pickedObject[i] != null)
            {
                Destroy(pickedObject[i]);
                pickedObject[i] = null;
            }
        }
    }

    // Cuando levanta un ingrediente
    private bool IsAlreadyPicked(GameObject obj)
    {
        foreach (var picked in pickedObject)
        {
            if (picked == obj)
                return true;
        }
        return false;
    }

    // Se posiciona en el primer punto disponible
    private int GetFirstFreeSlot()
    {
        for (int i = 0; i < pickedObject.Length; i++)
        {
            if (pickedObject[i] == null)
                return i;
        }
        return -1;
    }

    // Actualiza el UI de los ingredientes
    private void UpdateUI(string tag)
    {
        if (tag == "Bread") InventaryUI[0].SetActive(true);
        if (tag == "Sauce") InventaryUI[1].SetActive(true);
        if (tag == "Cheese") InventaryUI[2].SetActive(true);
        if (tag == "Meat") InventaryUI[3].SetActive(true);
    }
}