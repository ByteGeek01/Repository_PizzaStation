using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FoodPanel : MonoBehaviour
{
    public Image itemIcon;
    public TextMeshProUGUI countText;

    public void SetInfo(FoodSO foodSO, Inventory inventory)
    {
        itemIcon.sprite = foodSO.icon;
        countText.text = "x" + inventory.inventory[foodSO.foodName];
    }
}
