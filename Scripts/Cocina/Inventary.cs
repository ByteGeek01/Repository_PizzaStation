using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Inventary : MonoBehaviour
{
    // Dinero
    public int cash;
    public bool use;

    public TextMeshProUGUI cashText;

    public void Start()
    {
        cash = 100;
        UpdateCashUI();
    }

    // Cuenta de cliente pagada
    public void BillPayed()
    {
        cash = cash + 80;
        UpdateCashUI();
    }

    // Costo de ingredientes
    public void CostIngredients()
    {
        cash = cash - 10;
        use = true;
        UpdateCashUI();
    }

    // Pago de empleado
    public void WaiterCost()
    {
        cash = cash - 20;
        UpdateCashUI();
    }

    private void UpdateCashUI()
    {
        cashText.text = "Cash: $" + cash.ToString();
    }
}
