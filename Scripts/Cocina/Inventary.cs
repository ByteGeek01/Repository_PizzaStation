using UnityEngine;

public class Inventary : MonoBehaviour
{
    // Dinero
    public int cash;

    public void Start()
    {
        cash = 100;
    }

    // Cuenta de cliente pagada
    public void BillPayed()
    {
        cash = cash + 50;
    }

    // Costo de ingredientes
    public void CostIngredients()
    {
        cash = cash - 10;
    }
}
