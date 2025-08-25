using UnityEngine;
using UnityEngine.UI;

public class FoodItem : MonoBehaviour
{
    public FoodSO foodSO;

    public Image icon;

    void Start()
    {
        Instantiate(foodSO.model, transform.position, Quaternion.identity);
        icon.sprite = foodSO.icon;
    }
}
