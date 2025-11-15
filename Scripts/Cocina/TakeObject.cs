using System.Collections.Generic;
using UnityEngine;

public class TakeObject : MonoBehaviour
{
    public Dictionary<string, int> inventaryList = new Dictionary<string, int>();
    public GameObject[] point;
    public GameObject[] pickedObject;
    public GameObject[] InventaryUI;

    private HashSet<string> pickableTags = new HashSet<string>
    {
        "Pizza", "Cheese", "Bread", "Meat", "Sauce", "Waiter"
    };

    private Inventary inventary;

    private Collider currentObject;  // 🔥 El objeto dentro del trigger
    private bool canPick = true;

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            inventary = player.GetComponent<Inventary>();

        pickedObject = new GameObject[point.Length];

        string[] ingredients = { "Bread", "Sauce", "Cheese", "Meat", "Pizza" };
        foreach (string item in ingredients)
            inventaryList[item] = 0;
    }

    private void Update()
    {
        // Solo toma si hay un objeto cerca y presiona E
        if (currentObject != null && Input.GetKeyDown(KeyCode.E))
        {
            TryPickObject(currentObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (pickableTags.Contains(other.tag))
            currentObject = other;
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentObject == other)
            currentObject = null;
    }

    private void TryPickObject(Collider other)
    {
        if (!canPick) return;

        string tag = other.tag;

        // ENTREGAR PIZZA AL MESERO
        if (other.CompareTag("Waiter") && HasPizzaInHands())
        {
            ClearAllHands();
            Debug.Log("Pizza entregada al mesero.");
            return;
        }

        // Evita duplicar ingredientes
        if (!CanPickType(tag))
        {
            Debug.Log($"⚠ Ya tienes un ingrediente del tipo {tag}");
            return;
        }

        CleanupPickedArray();

        int slot = GetFirstFreeSlot();
        if (slot == -1)
        {
            Debug.Log("❌ No hay espacio en las manos.");
            return;
        }

        canPick = false;

        // 🔥 YA NO HAY COSTO
        // inventary.SubtractCash(5);

        // Lógica inventario
        inventaryList[tag] = 1;

        // Física
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        // Parent
        other.transform.SetParent(point[slot].transform);
        other.transform.localPosition = Vector3.zero;

        pickedObject[slot] = other.gameObject;

        UpdateUI(tag);
        StartCoroutine(ResetPickDelay());
    }

    private System.Collections.IEnumerator ResetPickDelay()
    {
        yield return new WaitForSeconds(0.1f);
        canPick = true;
    }

    private bool HasPizzaInHands()
    {
        foreach (var o in pickedObject)
            if (o != null && o.CompareTag("Pizza"))
                return true;

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

    private bool CanPickType(string tag)
    {
        foreach (var o in pickedObject)
            if (o != null && o.CompareTag(tag))
                return false;

        return true;
    }

    private void CleanupPickedArray()
    {
        for (int i = 0; i < pickedObject.Length; i++)
            if (pickedObject[i] != null && pickedObject[i].transform.parent == null)
                pickedObject[i] = null;
    }

    private int GetFirstFreeSlot()
    {
        for (int i = 0; i < pickedObject.Length; i++)
            if (pickedObject[i] == null)
                return i;

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