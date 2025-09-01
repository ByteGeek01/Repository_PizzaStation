using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Dictionary<string, int> inventory = new Dictionary<string, int>();

    public void AddObject(string itemName, int count)
    {
        inventory[itemName] += count;
    }
}
