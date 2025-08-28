using UnityEngine;

public class Inventary : MonoBehaviour
{
    public int cash;

    public void Start()
    {
        cash = 100;
    }

    public void BillPayed()
    {
        cash = cash + 50;
    }

    public void CostIngredients()
    {
        cash = cash - 10;
    }
}
