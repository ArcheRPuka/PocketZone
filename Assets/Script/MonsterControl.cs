using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Windows;

public class MonsterControl : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField]
    private float speed = 1f;
    [SerializeField]
    private float visionRadius = 1f;
    [SerializeField]
    private float damage = 10f;
    [SerializeField]
    private float radiusAttack = 1f;
    [SerializeField]
    private float maxhp = 100f;

    [Header("Other")]
    [SerializeField]
    private new Rigidbody2D rigidbody;
    [SerializeField]
    private CircleCollider2D visionCollider;   
    [SerializeField]
    private GameObject hpGO;   

    private float hp = -1;
    public float Hp
    {
        get { return hp; }
        set { hp = value; }
    }
    private bool targetAttack = false;
    private Animator animator;
    private float timeMoove;
    private bool flipLeft = false;
    private Vector2 lastPos = Vector2.zero;
    private GameObject targetPlayer;

    private bool isDead = false;
    public bool IsDead
    {
        get { return isDead; }
        set { isDead = value; }
    }

    private void Die(bool spawnItem = true)
    {
        animator.SetBool("Die", true);
        isDead = true;
        if (spawnItem)
            GameObject.FindGameObjectWithTag("MainCamera").gameObject.GetComponent<Data>().AddItemGround(new Vector2(transform.position.x, transform.position.y - 1f), 1, 10);
    }
    /// <summary>
    /// damage this monster
    /// </summary>
    /// <param name="damage">value damage</param>
    /// <param name="spawnItem">at the death</param>
    /// <returns></returns>
    public bool Damage(float damage, bool spawnItem = true)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Die(spawnItem);
            return true;
        }
        EditHealthPointGameObject();
        return false;
    }

    private void EditHealthPointGameObject()
    {
        hpGO.transform.GetChild(0).GetComponent<RectTransform>().offsetMax = new Vector2(hp / maxhp, 0);
    }

    private void DamagePlayer()
    {
        targetPlayer.GetComponent<PlayerControl>().Damage(damage);
    }

    private void Moove()
    {
        if (isDead)
        {
            rigidbody.velocity = Vector3.zero;
        }
        else
        {
            if (targetAttack)
            {
                animator.SetBool("Run", true);
                float posX = transform.position.x;
                transform.position = Vector3.MoveTowards(transform.position, targetPlayer.transform.position, speed * Time.deltaTime);
                if ((posX > transform.position.x && !flipLeft) || (posX < transform.position.x && flipLeft))
                    Flip();
                Attack();
            }
            else
            {
                if (MooveTime())
                {
                    animator.SetBool("Run", true);
                    rigidbody.velocity = new Vector2(Random.Range(-1f, 1f) * speed, Random.Range(-1f, 1f) * speed);
                    if ((rigidbody.velocity.x > 0 && flipLeft) || (rigidbody.velocity.x < 0 && !flipLeft))
                        Flip();
                }
            }
        }
    }
    /// <summary>
    /// if radiusAttack true then attack player
    /// </summary>
    private void Attack()
    {
        if (Vector3.Distance(transform.position, targetPlayer.transform.position) <= radiusAttack && !isDead)
        {
            animator.SetBool("Attack", true);
            targetPlayer.GetComponent<PlayerControl>().CamAlig();
        }
        else
        {
            animator.SetBool("Attack", false);
        }
    }
    /// <summary>
    /// flip monster
    /// </summary>
    private void Flip()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        hpGO.transform.localScale = new Vector3(hpGO.transform.localScale.x * -1, hpGO.transform.localScale.y, hpGO.transform.localScale.z);
        flipLeft = !flipLeft;
    }

    private bool MooveTime()
    {
        if (Time.time - timeMoove >= 1f)
        {
            timeMoove = Time.time;
            return true;
        }
        return false;
    }
    /// <summary>
    /// if player then set target player
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            targetPlayer = collision.gameObject;
            targetAttack = true;
            rigidbody.velocity = Vector3.zero;
        }
    }
    /// <summary>
    /// lose a player
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            targetAttack = false;
            collision.GetComponent<PlayerControl>().StopVelocity();
            timeMoove = Time.time;
        }
    }
    /// <summary>
    /// attack player || free
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (collision.GetComponent<PlayerControl>().IsDead)
            {
                targetAttack = false;
                animator.SetBool("Attack", false);
            }
        }
    }

    private void Update()
    {
        Moove();
    }
    /// <summary>
    /// set animation Run
    /// </summary>
    private void FixedUpdate()
    {
        if (!targetAttack)
        {
            if (Vector2.Distance(lastPos, (Vector2)transform.position) == 0)
                animator.SetBool("Run", false);
            else
                animator.SetBool("Run", true);
            lastPos = transform.position;
        }
    }
    /// <summary>
    /// if no load set hp, set visionRadius 
    /// </summary>
    private void Awake()
    {
        if (hp == -1)
        {
            hp = maxhp;
            InitStartPos();
        }
        timeMoove = Time.time;
        visionCollider.radius = visionRadius;
        animator = GetComponent<Animator>();
    }
    /// <summary>
    /// search empty cell on the map and set position monster
    /// </summary>
    private void InitStartPos()
    {
        Vector3Int v3i;
        Tilemap[] tilemaps = FindObjectsOfType<Tilemap>();
        Tilemap tilemap, tilemapCollideable;
        if (tilemaps[0].tag == "Ground")
        {
            tilemap = tilemaps[0];
            tilemapCollideable = tilemaps[1];
        }
        else
        {
            tilemap = tilemaps[1];
            tilemapCollideable = tilemaps[0];
        }
        BoundsInt boundsInt = tilemap.cellBounds;
        do
        {
            v3i = new Vector3Int(Random.Range(boundsInt.min.x, boundsInt.max.x), Random.Range(boundsInt.min.y, boundsInt.max.y), 0);
            if (tilemapCollideable.GetTile(v3i) != null)
                continue;
        } while (tilemap.GetTile(v3i) == null);
        transform.position = tilemap.CellToWorld(v3i);
    }
}
