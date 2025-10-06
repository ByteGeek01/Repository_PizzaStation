using UnityEngine;
using TMPro;

/// <summary>
/// Gestiona el dinero del jugador y las transacciones del restaurante.
/// </summary>
public class Inventary : MonoBehaviour
{
    [Header("Dinero")]
    [SerializeField] private int cash = 100;
    [SerializeField] private TextMeshProUGUI cashText;

    [Header("Configuración de valores")]
    [SerializeField] private int billIncome = 80;
    [SerializeField] private int ingredientCost = 5;
    [SerializeField] private int waiterCost = 10;
    [SerializeField] private int chefCost = 20;

    public bool Use { get; private set; }

    private void Start()
    {
        cash = 100;
        UpdateCashUI();
    }

    // Suma dinero cuando el cliente paga su cuenta.
    public void BillPayed()
    {
        AddCash(billIncome);
    }

    // Resta dinero al comprar ingredientes.
    public void CostIngredients()
    {
        SubtractCash(ingredientCost);
        Use = true;
    }

    // Resta dinero al pagar al mesero.
    public void WaiterCost()
    {
        SubtractCash(waiterCost);
    }

    // Resta el sueldo del chef
    public void ChefCost()
    {
        SubtractCash(chefCost);
    }

    // Suma dinero al inventario.
    public void AddCash(int amount)
    {
        cash += Mathf.Max(0, amount); // evita agregar valores negativos
        UpdateCashUI();
    }

    // Resta dinero del inventario, sin permitir valores negativos.
    public void SubtractCash(int amount)
    {
        cash = Mathf.Max(0, cash - Mathf.Abs(amount));
        UpdateCashUI();
    }

    // Actualiza el texto del dinero en pantalla.
    private void UpdateCashUI()
    {
        if (cashText != null)
        {
            cashText.text = $"Cash: ${cash}";
        }
    }

    // Retorna el dinero actual.
    public int GetCash() => cash;

    public void ResetUse()
    {
        Use = false;
    }
}