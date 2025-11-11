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
        if (!canPick) return;
        if (!pickableTags.Contains(other.tag)) return;

        // ENTREGAR PIZZA
        if (other.CompareTag("Waiter") && HasPizzaInHands())
        {
            ClearAllHands();
            Debug.Log("🍕 Pizza entregada al mesero.");
            return;
        }

        // TOMAR OBJETO
        if (Input.GetKeyDown(KeyCode.E))
        {
            string tag = other.tag;

            // Evita duplicar ingredientes del mismo tipo
            if (!CanPickType(tag))
            {
                Debug.Log($"⚠ Ya tienes este ingrediente en la mano: {tag}");
                return;
            }

            CleanupPickedArray();

            int freeIndex = GetFirstFreeSlot();
            if (freeIndex == -1)
            {
                Debug.Log("No hay espacios disponibles.");
                return;
            }

            canPick = false;

            // Cobro solo si no es pizza
            if (tag != "Pizza")
            {
                if (inventary.GetCash() < 5)
                {
                    Debug.Log("No hay suficiente dinero.");
                    StartCoroutine(ResetPickDelay());
                    return;
                }
                inventary.SubtractCash(5);
            }

            // Inventario lógico (solo una vez)
            if (inventaryList[tag] < 1)
                inventaryList[tag] = 1;

            // Físicas y posición
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }

            // Parentado al slot libre
            other.transform.SetParent(point[freeIndex].transform);
            other.transform.localPosition = Vector3.zero;

            pickedObject[freeIndex] = other.gameObject;

            UpdateUI(tag);
            StartCoroutine(ResetPickDelay());
        }
    }

    private System.Collections.IEnumerator ResetPickDelay()
    {
        yield return new WaitForSeconds(0.1f);
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

    // Evita duplicar ingredientes del mismo tipo
    private bool CanPickType(string tag)
    {
        foreach (var obj in pickedObject)
        {
            if (obj != null && obj.CompareTag(tag))
                return false;
        }
        return true;
    }

    // Elimina referencias rotas
    private void CleanupPickedArray()
    {
        for (int i = 0; i < pickedObject.Length; i++)
        {
            if (pickedObject[i] != null && pickedObject[i].transform.parent == null)
                pickedObject[i] = null;
        }
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