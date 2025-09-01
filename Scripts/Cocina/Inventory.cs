using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Dictionary<string, int> inventory = new Dictionary<string, int>();
    public List<FoodSO> foodSOs = new List<FoodSO>();

    public Transform scrollInventory;
    public GameObject panelProduct;

    public FoodSO GetFoodSO(string foodName)
    {
        foreach(var food in foodSOs)
        {
            if(food.foodName == foodName) return food;
        }
        return null;
    }

    public void AddObject(string itemName, int count)
    {
        if (inventory.ContainsKey(itemName))
        {
            inventory[itemName] += count;
        }
        else
        {
            inventory.TryAdd(itemName, count);
        }
        Debug.Log("obtenido " + count + " " + itemName);
        RefreshInventoryUI();
    }

    public int RemoveObject(string itemName, int count)
    {
        if (!inventory.ContainsKey(itemName))
        {
            RefreshInventoryUI();
            return 0;
        }
        else
        {
            if(inventory [itemName] >= count)
            {
                inventory[itemName] -= count;
                RefreshInventoryUI();
                return inventory[itemName];
                
            }
            else
            {
                inventory[itemName] = 0;
                RefreshInventoryUI();
                return inventory[itemName] - count;
            }
        }
    }

    public void RefreshInventoryUI()
    {
        if(inventory.Count <= 0)
        {
            return;
        }
        foreach(Transform child in scrollInventory)
        {
            Destroy(child.gameObject);
        }
        foreach (var item in inventory)
        {
            GameObject p = Instantiate(panelProduct, scrollInventory);
            p.GetComponent<FoodPanel>().SetInfo(GetFoodSO(item.Key), this);
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            AddObject("Pizza", 10);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RemoveObject("Pizza", 10);
        }
    }
}
