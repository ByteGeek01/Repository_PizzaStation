using System.Collections;
using UnityEngine;

public class Oven : MonoBehaviour
{
    public TakeObject itemCollect;

    public Transform spawnPizza;
    public GameObject pizza;

    public bool[] ingredients = new bool[4];
    private bool isBaking = false;

    private enum IngredientType { Bread, Sauce, Cheese, Meat }

    public void OnTriggerEnter(Collider other)
    {
        // Lista de ingredientes de pizza
        switch (other.tag)
        {
            case "Bread":
                ingredients[(int)IngredientType.Bread] = true;
                Destroy(other.gameObject);
                itemCollect.InventaryUI[0].SetActive(false);
                break;

            case "Sauce":
                ingredients[(int)IngredientType.Sauce] = true;
                Destroy(other.gameObject);
                itemCollect.InventaryUI[1].SetActive(false);
                break;

            case "Cheese":
                ingredients[(int)IngredientType.Cheese] = true;
                Destroy(other.gameObject);
                itemCollect.InventaryUI[2].SetActive(false);
                break;

            case "Meat":
                ingredients[(int)IngredientType.Meat] = true;
                Destroy(other.gameObject);
                itemCollect.InventaryUI[3].SetActive(false);
                break;
        }

        // Solo comienza a hornear si no está horneando y tiene todos los ingredientes
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
        Instantiate(pizza, spawnPizza.position, Quaternion.identity);

        // Espera un segundo y reinicia ingredientes
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < ingredients.Length; i++)
        {
            ingredients[i] = false;
        }
        isBaking = false;
    }
}