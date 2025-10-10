using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Inventary : MonoBehaviour
{
    public static Inventary instance;

    public static Action OnEnableCashCharge;
    public static Action OnDisableCashCharge;

    [Header("Ingresos")]
    [SerializeField] private int cash = 100;
    [SerializeField] private TextMeshProUGUI cashText;

    [Header("Ingredientes")]
    public Dictionary<string, int> ingredients = new Dictionary<string, int>();
    [SerializeField] private TextMeshProUGUI ingredientsText;

    [Header("Costos")]
    public int waiterCost = 10;
    public int billIncome = 80;

    public int pizzasEntregadas = 0;
    public int clientesMolestos = 0;

    private void Awake()
    {
        instance = this;

        OnEnableCashCharge += ChargeWaiter;
        OnDisableCashCharge += GiveBillIncome;
    }

    private void Start()
    {
        cash = 100;
        UpdateCashUI();

        string[] ingredientNames = { "Bread", "Sauce", "Cheese", "Meat", "Pizza", "Waiter" };
        foreach (string name in ingredientNames)
        {
            ingredients[name] = 0;
        }

        UpdateIngredientsUI();
    }

    private void ChargeWaiter()
    {
        SubtractCash(waiterCost);
    }

    private void GiveBillIncome()
    {
        AddCash(billIncome);
        pizzasEntregadas++;
    }

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

    public void AddIngredient(string ingredient, int cost = 0)
    {
        if (!ingredients.ContainsKey(ingredient))
            ingredients[ingredient] = 0;

        if (cost > 0 && cash >= cost)
            SubtractCash(cost);

        ingredients[ingredient]++;
        UpdateIngredientsUI();
    }

    private void UpdateCashUI()
    {
        if (cashText != null)
            cashText.text = $"Cash: ${cash}";
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