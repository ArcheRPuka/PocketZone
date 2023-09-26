using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CellInventory : MonoBehaviour
{
    [Header("Collection sprite items")]
    [SerializeField]
    private Sprite[] spritesItem;

    [Header("Other link")]
    [SerializeField]
    private GameObject Btn;
    [SerializeField]
    private Image iconItem;
    [SerializeField]
    private TMP_Text textCount;

    private int cellId;
    private GameObject contrObj;
    public void Init(int _cellId, GameObject _obj)
    {
        cellId = _cellId;
        contrObj = _obj;
    }

    public void ClickCell()
    {
        if (Btn.activeSelf)
            Btn.SetActive(false);
        else
            Btn.SetActive(true);
    }

    public void ClickDel()
    {
        Btn.SetActive(false);
        iconItem.sprite = null;
        ShoweText(0);
        contrObj.GetComponent<BackpackControl>().DelItem(cellId);
    }

    public void Add(int _id, int count)
    {
        if(count == 0)
        {
            iconItem.sprite = null;
            return;
        }
        iconItem.sprite = spritesItem[_id];
        ShoweText(count);
    }

    private void ShoweText(int count)
    {
        if(count <= 1)
        {
            textCount.text = "";
            return;
        }
        textCount.text = count.ToString();
    }
}
