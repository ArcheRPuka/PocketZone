using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGround : MonoBehaviour
{
    [Header("Collection items")]
    [SerializeField]
    private Sprite[] sprites;

    [Header("Only debug")]
    [SerializeField]
    private int id;
    public int Id
    {
        get { return id; }
    }
    [SerializeField]
    private int count = 1;
    public int Count
    {
        get { return count; }
    }
    private SpriteRenderer spriteRenderer;

    

    public void Init(int _id, int _count = 1)
    {
        id = _id;
        count = _count;
        spriteRenderer.sprite = sprites[id];
    }
    /// <summary>
    /// add item in backpack player
    /// </summary>
    /// <param name="obj">player object</param>
    public void Pick(GameObject obj)
    {
        if (obj.GetComponent<Backpack>().AddItem(id, count))
            Destroy(gameObject);
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (spriteRenderer.sprite == null)
            Init(id, count);
    }
    /// <summary>
    /// if player pick up item
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            Pick(collision.gameObject);
    }
}
