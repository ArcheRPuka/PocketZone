using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackpackControl : MonoBehaviour
{
    [Header("Other")]
    [SerializeField]
    private GameObject prefabCell;
    [SerializeField]
    private GameObject content;

    private int countCell;
    private GameObject player;
    private GameObject[] cellGO;
    /// <summary>
    /// visul add item
    /// </summary>
    /// <param name="id">id item</param>
    /// <param name="cell">cell array</param>
    /// <param name="count">count items</param>
    public void AddItem(int id, int cell, int count)
    {
        cellGO[cell].GetComponent<CellInventory>().Add(id, count);
    }
    /// <summary>
    /// visual remove item
    /// </summary>
    /// <param name="cell">cell array</param>
    public void DelItem(int cell)
    {
        player.GetComponent<Backpack>().DelItem(cell);
    }
    /// <summary>
    /// visual performance inventory
    /// </summary>
    /// <param name="objPlayer"></param>
    public void Init(GameObject objPlayer)
    {
        player = objPlayer;
        countCell = player.GetComponent<Backpack>().CountCell;
        cellGO = new GameObject[countCell];
        for (int i = 0; i < countCell; i++)
        {
            cellGO[i] = Instantiate(prefabCell);
            cellGO[i].transform.parent = content.transform;
            cellGO[i].transform.localPosition = Vector3.zero;
            cellGO[i].transform.localScale = Vector3.one;
            cellGO[i].GetComponent<CellInventory>().Init(i, gameObject);
        }
    }
}
