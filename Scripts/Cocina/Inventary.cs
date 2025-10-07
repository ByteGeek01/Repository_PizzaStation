using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Inventary : MonoBehaviour
{
    [Header("Ingresos")]
    [SerializeField] private int cash = 100;
    [SerializeField] private TextMeshProUGUI cashText;

    [Header("Ingredientes")]
    public Dictionary<string, int> ingredients = new Dictionary<string, int>();
    [SerializeField] private TextMeshProUGUI ingredientsText;

    [Header("Costos")]
    [SerializeField] private int waiterCost = 10; // costo del mesero
    [SerializeField] private int billIncome = 80;
    public int GetWaiterCost() => waiterCost;
    public int BillIncome => billIncome;

    private void Start()
    {
        cash = 100;
        // Inicializa dinero e ingredientes
        UpdateCashUI();

        string[] ingredientNames = { "Bread", "Sauce", "Cheese", "Meat", "Pizza", "Waiter" };
        foreach (string name in ingredientNames)
        {
            ingredients[name] = 0;
        }

        UpdateIngredientsUI();
    }

    // Métodos de dinero
    public void AddCash(int amount)
    {
        cash += amount;
        UpdateCashUI();
    }

    public void SubtractCash(int amount)
    {
        cash = Mathf.Max(0, cash - amount);
        UpdateCashUI();
    }

    public int GetCash() => cash;
    public int WaiterCostAmount => waiterCost;


    // Métodos de ingredientes
    public void AddIngredient(string ingredient, int cost = 0)
    {
        if (!ingredients.ContainsKey(ingredient))
            ingredients[ingredient] = 0;

        if (cost > 0 && cash >= cost)
            SubtractCash(cost);

        ingredients[ingredient]++;
        UpdateIngredientsUI();
    }

    public bool HasIngredientsForRecipe()
    {
        return ingredients["Bread"] >= 1 &&
               ingredients["Sauce"] >= 1 &&
               ingredients["Cheese"] >= 1 &&
               ingredients["Meat"] >= 1;
    }

    private void UpdateCashUI()
    {
        if (cashText != null)
            cashText.text = $"Cash: ${cash}";
    }

    public void BillPayed()
    {
        AddCash(billIncome);
    }

    private void UpdateIngredientsUI()
    {
        if (ingredientsText != null)
        {
            ingredientsText.text = $"Ingredients:\n";
            foreach (var kvp in ingredients)
            {
                ingredientsText.text += $"{kvp.Key}: {kvp.Value}\n";
            }
        }
    }
}