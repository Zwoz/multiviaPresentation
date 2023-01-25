using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoScript : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public Item[] itemsToPickup;

    public void PickupItem(int id)
    {
       bool result = inventoryManager.AddItem(itemsToPickup[id]);
        if (result == true)
        {
            Debug.Log("Item Added");
        }
        else
        {
            Debug.Log("Item NOT Added");
        }
    }

    public void GetSelectedItem()
    {
        Item recivedItem = inventoryManager.GetSelectedItem(false);
        if (recivedItem != null)
        {
            Debug.Log("Received Item: " + recivedItem);
        } else
        {
            Debug.Log("No item received");
        }
    }
    
    public void UseSelectedItem()
    {
        Item recivedItem = inventoryManager.GetSelectedItem(true);
        if (recivedItem != null)
        {
            Debug.Log("Used Item: " + recivedItem);
        } else
        {
            Debug.Log("No item used");
        }
    }
}
