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
    private bool canPick = true; // evita spam si mantiene presionado E

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
        // 🔹 Entregar pizza al mesero (libera todos los puntos)
        if (other.CompareTag("Waiter") && HasPizzaInHands())
        {
            ClearAllHands();
            Debug.Log("🍕 Pizza entregada al mesero.");
            return;
        }

        // 🔹 Recolectar ingredientes
        if (Input.GetKeyDown(KeyCode.E) && canPick && pickableTags.Contains(other.tag))
        {
            // Evita recoger el mismo objeto 2 veces
            if (IsAlreadyPicked(other.gameObject))
                return;

            int freeIndex = GetFirstFreeSlot();
            if (freeIndex == -1)
            {
                Debug.Log("No hay más espacio para sostener ingredientes.");
                return;
            }

            canPick = false; // evita spam de tecla

            string tag = other.tag;

            // Cobro si no es pizza
            if (tag != "Pizza")
            {
                if (inventary.GetCash() < 5)
                {
                    Debug.Log("No hay suficiente dinero para comprar este ingrediente.");
                    return;
                }
                inventary.SubtractCash(5);
            }

            // Agrega al inventario una sola vez
            if (inventaryList.ContainsKey(tag))
                inventaryList[tag]++;
            else
                inventaryList[tag] = 1;

            // Configura físicas y posición
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }

            // Coloca en el punto libre
            other.transform.SetParent(point[freeIndex].transform);
            other.transform.localPosition = Vector3.zero;
            pickedObject[freeIndex] = other.gameObject;

            UpdateUI(tag);

            StartCoroutine(ResetPickDelay());
        }
    }

    private System.Collections.IEnumerator ResetPickDelay()
    {
        yield return new WaitForSeconds(0.3f);
        canPick = true;
    }

    private bool HasPizzaInHands()
    {
        foreach (var obj in pickedObject)
        {
            if (obj != null && obj.CompareTag("Pizza"))
                return true;
        }
        return false;
    }

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

    private bool IsAlreadyPicked(GameObject obj)
    {
        foreach (var picked in pickedObject)
        {
            if (picked == obj)
                return true;
        }
        return false;
    }

    private int GetFirstFreeSlot()
    {
        for (int i = 0; i < pickedObject.Length; i++)
        {
            if (pickedObject[i] == null)
                return i;
        }
        return -1;
    }

    private void UpdateUI(string tag)
    {
        if (tag == "Bread") InventaryUI[0].SetActive(true);
        if (tag == "Sauce") InventaryUI[1].SetActive(true);
        if (tag == "Cheese") InventaryUI[2].SetActive(true);
        if (tag == "Meat") InventaryUI[3].SetActive(true);
    }
}