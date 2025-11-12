using System.Collections;
using UnityEngine;

public class Oven : MonoBehaviour
{
    public TakeObject itemCollect;

    [Header("Horno y Pizza")]
    public Transform[] spawnPizza;
    public GameObject pizza;

    [Header("Ingredientes requeridos")]
    public bool[] ingredients = new bool[4];
    private enum IngredientType { Bread, Sauce, Cheese, Meat }

    [Header("Estado del horno")]
    public bool isBaking = false;
    public int ovenLevel = 1; // Nivel 1 = 1 pizza, Nivel 2 = 3 pizzas

    [Header("Mejoras")]
    public GameObject upgradeNotice; // Panel o ícono de mejora
    public int upgradeCost = 1000;

    private void Start()
    {
        // Recuperar nivel del horno guardado (por si ya fue mejorado antes)
        ovenLevel = PlayerPrefs.GetInt("OvenLevel", 1);

        // Comprobar si el jugador tiene suficiente dinero para mostrar la mejora
        int playerCash = PlayerPrefs.GetInt("PlayerCash", 0);

        if (upgradeNotice != null)
            upgradeNotice.SetActive(playerCash >= upgradeCost && ovenLevel == 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Bread":
                ingredients[(int)IngredientType.Bread] = true;
                Destroy(other.gameObject);
                break;
            case "Sauce":
                ingredients[(int)IngredientType.Sauce] = true;
                Destroy(other.gameObject);
                break;
            case "Cheese":
                ingredients[(int)IngredientType.Cheese] = true;
                Destroy(other.gameObject);
                break;
            case "Meat":
                ingredients[(int)IngredientType.Meat] = true;
                Destroy(other.gameObject);
                break;
        }

        if (!isBaking && AllIngredientsPresent())
        {
            StartCoroutine(Bake());
        }
    }

    private bool AllIngredientsPresent()
    {
        foreach (bool hasIngredient in ingredients)
        {
            if (!hasIngredient) return false;
        }
        return true;
    }

    private IEnumerator Bake()
    {
        isBaking = true;
        Debug.Log($"🔥 Horneando {ovenLevel} pizza(s)...");

        yield return new WaitForSeconds(5f);

        int pizzasToSpawn = ovenLevel == 1 ? 1 : 3;

        for (int i = 0; i < pizzasToSpawn && i < spawnPizza.Length; i++)
        {
            Instantiate(pizza, spawnPizza[i].position, Quaternion.identity);
        }

        // Generar clientes (si hay espacio)
        if (GameManager.instance != null &&
            GameManager.instance.clients.Count < GameManager.instance.maxClients &&
            GameManager.instance.numberWave < 6)
        {
            GameManager.instance.numberWave++;

            Client newClient = GameManager.instance.CreateClient();
            GameManager.instance.SetTableForClient(newClient);

            if (GameManager.instance.numberWave >= 6)
                GameManager.instance.isSpawning = false;
        }

        // Apagar los iconos del inventario de ingredientes
        foreach (var ui in itemCollect.InventaryUI)
        {
            ui.SetActive(false);
        }

        // Esperar antes de volver a usar el horno
        yield return new WaitForSeconds(30f);
        for (int i = 0; i < ingredients.Length; i++)
            ingredients[i] = false;

        isBaking = false;
    }

    // --- 💡 MEJORA DEL HORNO ---
    public void UpgradeOven()
    {
        if (Inventary.instance == null) return;

        if (ovenLevel >= 2)
        {
            Debug.Log("🔥 El horno ya está al máximo nivel.");
            return;
        }

        if (Inventary.instance.GetCash() < upgradeCost)
        {
            Debug.Log("💸 No tienes suficiente dinero para mejorar el horno.");
            return;
        }

        // Restar dinero y mejorar el horno
        Inventary.instance.SubtractCash(upgradeCost);
        ovenLevel = 2;

        PlayerPrefs.SetInt("OvenLevel", ovenLevel);
        PlayerPrefs.Save();

        Debug.Log("🔥 ¡Horno mejorado! Ahora puede hornear 3 pizzas a la vez.");

        if (upgradeNotice != null)
            upgradeNotice.SetActive(false);
    }
}