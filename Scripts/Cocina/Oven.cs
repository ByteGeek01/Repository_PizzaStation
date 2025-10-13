using System.Collections;
using UnityEngine;

public class Oven : MonoBehaviour
{
    public TakeObject itemCollect;

    public Transform[] spawnPizza;
    public GameObject pizza;

    public bool[] ingredients = new bool[4];
    public bool isBaking = false;

    private enum IngredientType { Bread, Sauce, Cheese, Meat }

    public void OnTriggerEnter(Collider other)
    {
        // Lista de ingredientes de pizza
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

        // Solo comienza a hornear si tiene todos los ingredientes
        if (!isBaking && AllIngredientsPresent())
        {
            StartCoroutine(Bake());
        }
    }

    // Ingredientes presentes
    private bool AllIngredientsPresent()
    {
        foreach (bool hasIngredient in ingredients)
        {
            if (!hasIngredient) return false;
        }
        return true;
    }

    // Con todos los ingredientes, se hace la pizza
    public IEnumerator Bake()
    {
        isBaking = true;
        yield return new WaitForSeconds(5f);
        Instantiate(pizza, spawnPizza[0].position, Quaternion.identity);
        Instantiate(pizza, spawnPizza[1].position, Quaternion.identity);
        Instantiate(pizza, spawnPizza[2].position, Quaternion.identity);

        // Limita a 6 clientes maximo
        if (GameManager.instance != null && GameManager.instance.clients.Count < GameManager.instance.maxClients && GameManager.instance.numberWave < 6)
        {
            GameManager.instance.numberWave++;

            Client newClient = GameManager.instance.CreateClient();
            GameManager.instance.SetTableForClient(newClient);

            // Si llegamos al límite de olas, detener spawneo
            if (GameManager.instance.numberWave >= 6)
            {
                GameManager.instance.isSpawning = false;
            }
        }

        itemCollect.InventaryUI[0].SetActive(false);
        itemCollect.InventaryUI[1].SetActive(false);
        itemCollect.InventaryUI[2].SetActive(false);
        itemCollect.InventaryUI[3].SetActive(false);

        // Espera un tiempo, reinicia ingredientes y el horno se apaga
        yield return new WaitForSeconds(30f);
        for (int i = 0; i < ingredients.Length; i++)
        {
            ingredients[i] = false;
        }
        isBaking = false;
    }
}