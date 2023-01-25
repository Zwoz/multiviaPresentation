using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] const int MAXSLOTS = 6;
    bool inventoryOpen;
    public static InventoryManager instance;
    public Item[] startItems;
    public int maxStackedItems = 4;
    public InventorySlot[] inventorySlots;
    public GameObject invenotryItemPrefab;

    [SerializeField] GameObject inventory;
    [SerializeField] GameObject inventoryExit;



    int selectedSlot = 0;

    private void Start()
    {
        ChangeSelectedSlot(0);
        foreach (var item in startItems)
        {
            AddItem(item);
        }
        instance = this;
    }

    public void OnInventory()
    {
        if (inventoryOpen == false)
        {
            inventory.SetActive(true);
            inventoryExit.SetActive(true);
            inventoryOpen = true;
        }
        else 
        {
            inventory.SetActive(false);
            inventoryExit.SetActive(false);
            inventoryOpen = false;
        }
    }

    public void OnHotBar(InputValue value)
    {
        Debug.Log(value.Get<float>());
        ChangeSelectedSlot((int) value.Get<float>() - 1);
    }
    
    public void OnScrollWheel(InputValue value)
    {       
            float scrollValue = value.Get<Vector2>().y;
            if (scrollValue != 0){
                float scrollSigned = Mathf.Sign(scrollValue);
                float number = (selectedSlot + scrollSigned) % MAXSLOTS;
                if (number < 0) number = MAXSLOTS - 1;
                if(number != selectedSlot){
                ChangeSelectedSlot((int) number);
            }
        }
    }
    
    void ChangeSelectedSlot(int newValue)
    {
        inventorySlots[selectedSlot].Deselect();   
        inventorySlots[newValue].Select();
        selectedSlot = newValue;
    }

    public bool AddItem(Item item) // add item to inventory in slot
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItems itemInSlot = slot.GetComponentInChildren<InventoryItems>();
            if (itemInSlot != null && 
            itemInSlot.item == item && 
            itemInSlot.count < maxStackedItems &&
            itemInSlot.item.stackable == true)
            {
                itemInSlot.count++;
                itemInSlot.RefreshCount();
                return true;
            }
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItems itemInSlot = slot.GetComponentInChildren<InventoryItems>();
            if (itemInSlot == null)
            {
                SpawnNewItem(item, slot);
                return true;
            }
        }

        return false;
    }

    void SpawnNewItem(Item item, InventorySlot slot)
    {
        GameObject newItemGo = Instantiate(invenotryItemPrefab, slot.transform);
        InventoryItems inventoryItems = newItemGo.GetComponent<InventoryItems>();
        inventoryItems.InitialiseItem(item);
    }

    public Item GetSelectedItem(bool use) // Item being used/consumed
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItems itemInSlot = slot.GetComponentInChildren<InventoryItems>();
        if (itemInSlot != null)
        {
            Item item = itemInSlot.item;
            if(use == true)
            {
                itemInSlot.count--;
                if (itemInSlot.count <= 0)
                {
                    Destroy(itemInSlot.gameObject);
                } else 
                {
                    itemInSlot.RefreshCount();
                }
            }
            return item;
        }
        return null;
    }

}
