using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Backpack : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField, Tooltip("UI")]
    private GameObject bagpack;

    [SerializeField]
    private int countCell = 10;
    public int CountCell
    {
        get { return countCell; }
    }

    [SerializeField, Header("Prefabs")]
    private GameObject prefabItemGround;

    private InventoryItem[] inventoryItems;
    public InventoryItem[] InventoryItems
    {
        get { return inventoryItems; }
        set { inventoryItems = value; }
    }

    /// <summary>
    /// search cell with id or empty
    /// </summary>
    /// <param name="id">id item</param>
    /// <returns>cell number invrentoryItems</returns>
    private int FindInventory(int id)
    {
        for (int i = 0; i < inventoryItems.Length; i++) 
        {
            if (inventoryItems[i].Id == id)
                return i;
        }
        for (int i = 0; i < inventoryItems.Length; i++) 
        {
            if (inventoryItems[i].Count == 0)
                return i;
        }
        return -1;
    }
    /// <summary>
    /// search bullet if find sub 1
    /// </summary>
    /// <param name="_id">id ammo</param>
    /// <returns>true if successful</returns>
    public bool IsAmmoShoot(int _id)
    {
        int cell = FindInventory(_id);
        if (inventoryItems[cell].Count > 0)
        {
            inventoryItems[cell].Count--;
            bagpack.GetComponent<BackpackControl>().AddItem(inventoryItems[cell].Id, cell, inventoryItems[cell].Count);
            return true;
        }
        return false;
    }

    /// <summary>
    /// UI backpack active
    /// </summary>
    public void OnClick()
    {
        if (bagpack.activeSelf)
            bagpack.SetActive(false);
        else
            bagpack.SetActive(true);
    }

    /// <summary>
    /// add in item to inventoryItems
    /// </summary>
    /// <param name="id">id item</param>
    /// <param name="count">count item</param>
    /// <returns>true if successful</returns>
    public bool AddItem(int id, int count)
    {
        int cell = FindInventory(id);
        if (cell == -1)
            return false;
        //+gun+
        if (id == 0)
            GetComponent<PlayerControl>().AddGunID(id);
        //-gun-
        inventoryItems[cell].Id = id;
        inventoryItems[cell].Count += count;
        bagpack.GetComponent<BackpackControl>().AddItem(inventoryItems[cell].Id, cell, inventoryItems[cell].Count);
        return true;
    }

    /// <summary>
    /// deleting by cell number array inventiryItems
    /// </summary>
    /// <param name="cell"> cell number </param>
    public void DelItem(int cell)
    {     
        //+gun+
        if (inventoryItems[cell].Id == 0)
            GetComponent<PlayerControl>().AddGunID(-1);
        //-gun-   
        GameObject go = Instantiate(prefabItemGround);
        go.GetComponent<ItemGround>().Init(inventoryItems[cell].Id, inventoryItems[cell].Count);
        go.transform.position = new Vector3(transform.position.x + 1f, transform.position.y, transform.position.z);
        inventoryItems[cell].Count = 0;
        inventoryItems[cell].Id = -1;
    }    

    /// <summary>
    /// creating and filling empty values
    /// </summary>
    public void InitInventoruItem()
    {
        if (inventoryItems == null)
        {
            inventoryItems = new InventoryItem[countCell];
            for (int i = 0; i < countCell; i++)
                inventoryItems[i].Id = -1;
            bagpack.GetComponent<BackpackControl>().Init(gameObject);
        }
    }

    private void Awake()
    {
        InitInventoruItem();
    }
}

/// <summary>
/// id - item id, count - item count
/// </summary>
public struct InventoryItem
{
    private int id;
    public int Id
    {
        get { return id; }
        set { id = value; }
    }

    private int count;
    public int Count
    {
        get { return count; }
        set { count = value; }
    }
}
