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
    private int cash; // 🔹 ahora NO es static ni serializado
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

        // 🔹 Recuperar dinero guardado (si existe)
        cash = PlayerPrefs.GetInt("PlayerCash", 100);
        UpdateCashUI();
    }

    private void Start()
    {
        string[] ingredientNames = { "Bread", "Sauce", "Cheese", "Meat", "Pizza", "Waiter" };
        foreach (string name in ingredientNames)
        {
            ingredients[name] = 0;
        }

        UpdateIngredientsUI();
    }

    private void OnDestroy()
    {
        // 🔹 Guardar dinero antes de cambiar de escena
        PlayerPrefs.SetInt("PlayerCash", cash);
        PlayerPrefs.Save();
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
        PlayerPrefs.SetInt("PlayerCash", cash);
    }

    public void SubtractCash(int amount)
    {
        cash = Mathf.Max(0, cash - amount);
        UpdateCashUI();
        PlayerPrefs.SetInt("PlayerCash", cash);
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