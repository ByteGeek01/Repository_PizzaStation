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
    private int cash;
    [SerializeField] private TextMeshProUGUI cashText;

    [Header("Ingredientes")]
    public Dictionary<string, int> ingredients = new Dictionary<string, int>();
    [SerializeField] private TextMeshProUGUI ingredientsText;

    [Header("Costos")]
    public int waiterCost = 10;
    public int billIncome = 80;
    public int moreWaiter = 200;

    public int pizzasEntregadas = 0;
    public int clientesMolestos = 0;

    [Header("Personal y Waiters")]
    public GameObject personalUpdate; // Panel o botón para contratar
    public GameObject[] waiter;       // Array de meseros

    private int activeWaiters = 0; // cantidad de meseros activos
    private bool hiredThisRound = false; // Evita mostrar el panel tras contratar

    private void Awake()
    {
        instance = this;

        OnEnableCashCharge += ChargeWaiter;
        OnDisableCashCharge += GiveBillIncome;

        // Recuperar dinero y cantidad de meseros activos guardados
        cash = PlayerPrefs.GetInt("PlayerCash", 100);
        activeWaiters = PlayerPrefs.GetInt("ActiveWaiters", 0); // <-- Cambiado a 0
        hiredThisRound = PlayerPrefs.GetInt("HiredThisRound", 0) == 1;

        UpdateCashUI();
    }

    private void Start()
    {
        // Inicializar inventario
        string[] ingredientNames = { "Bread", "Sauce", "Cheese", "Meat", "Pizza", "Waiter" };
        foreach (string name in ingredientNames)
        {
            ingredients[name] = 0;
        }

        // Reactivar los meseros guardados
        for (int i = 0; i < waiter.Length; i++)
        {
            waiter[i].SetActive(i < activeWaiters);
        }

        // Mostrar el panel de contratación si hay dinero suficiente y todavía faltan meseros
        CheckPersonalUpdateAvailability();

        UpdateIngredientsUI();
    }

    private void OnDestroy()
    {
        // Guarda el dinero y cantidad de meseros antes de cambiar de escena
        PlayerPrefs.SetInt("PlayerCash", cash);
        PlayerPrefs.SetInt("ActiveWaiters", activeWaiters);
        PlayerPrefs.SetInt("HiredThisRound", hiredThisRound ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Mostrar/ocultar el panel según dinero, meseros activos y si ya contrató en la ronda
    private void CheckPersonalUpdateAvailability()
    {
        if (personalUpdate != null)
        {
            bool canHire = (cash >= 500 && activeWaiters < waiter.Length && !hiredThisRound);
            personalUpdate.SetActive(canHire);
        }
    }

    // Cobro de mesero
    private void ChargeWaiter()
    {
        SubtractCash(waiterCost);
    }

    // Cuenta pagada
    private void GiveBillIncome()
    {
        AddCash(billIncome);
        pizzasEntregadas++;
    }

    // Suma dinero
    public void AddCash(int amount)
    {
        cash += amount;
        UpdateCashUI();
        PlayerPrefs.SetInt("PlayerCash", cash);
        CheckPersonalUpdateAvailability();
    }

    // Quita dinero
    public void SubtractCash(int amount)
    {
        cash = Mathf.Max(0, cash - amount);
        UpdateCashUI();
        PlayerPrefs.SetInt("PlayerCash", cash);
        CheckPersonalUpdateAvailability();
    }

    public int GetCash() => cash;
    public int WaiterCostAmount => waiterCost;

    // Añade ingrediente a cambio de dinero
    public void AddIngredient(string ingredient, int cost = 0)
    {
        if (!ingredients.ContainsKey(ingredient))
            ingredients[ingredient] = 0;

        if (cost > 0 && cash >= cost)
            SubtractCash(cost);

        ingredients[ingredient]++;
        UpdateIngredientsUI();
    }

    // Contratar más meseros
    public void MoreWaiter()
    {
        if (cash < moreWaiter)
        {
            Debug.Log("💸 No tienes suficiente dinero para contratar otro mesero.");
            return;
        }

        if (activeWaiters >= waiter.Length)
        {
            Debug.Log("✅ Ya has contratado todos los meseros disponibles.");
            personalUpdate.SetActive(false);
            return;
        }

        // Cobrar
        SubtractCash(moreWaiter);

        // Activar el siguiente mesero
        waiter[activeWaiters].SetActive(true);
        activeWaiters++;

        // Marcar que ya se contrató un mesero en esta ronda
        hiredThisRound = true;
        PlayerPrefs.SetInt("HiredThisRound", 1);

        // Guardar progreso
        PlayerPrefs.SetInt("ActiveWaiters", activeWaiters);
        PlayerPrefs.Save();

        Debug.Log($"👨‍🍳 Contrataste un nuevo mesero. Total activos: {activeWaiters}");

        // Ocultar el panel hasta la próxima ronda
        personalUpdate.SetActive(false);
    }

    // Método para reiniciar el estado de contratación al iniciar nueva ronda
    public void ResetHireFlagForNewRound()
    {
        hiredThisRound = false;
        PlayerPrefs.SetInt("HiredThisRound", 0);
        PlayerPrefs.Save();
        CheckPersonalUpdateAvailability();
    }

    // Actualiza UI de dinero
    private void UpdateCashUI()
    {
        if (cashText != null)
            cashText.text = $"Cash: ${cash}";
    }

    // Actualiza UI de ingredientes
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