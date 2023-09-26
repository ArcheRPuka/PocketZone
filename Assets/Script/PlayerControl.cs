using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    [Header("Setting Player")]
    [SerializeField]
    private float speed = 1.0f;
    [SerializeField]
    private float maxhp = 100f;

    [Header("Setting gun")]
    [SerializeField]
    private float damageAk47 = 25f;
    [SerializeField]
    private float radiusAttack = 10f;

    [Header("Settinh other")]
    [SerializeField, Tooltip("Camera on the player")]
    private Transform targetCamera;
    [SerializeField]
    private GameObject gunGO;
    [SerializeField]
    private GameObject hpGO;
    [SerializeField, Tooltip("UI")]
    private GameObject btnShoot;
    [SerializeField]
    private GameObject prefabBullet;
    [SerializeField]
    private Animator animator;

    /// <summary>
    /// player look left
    /// </summary>
    private bool leftPlayer = false;
    /// <summary>
    /// id item gun, -1 = empty
    /// </summary>
    private int idGun = -1;
    public int IdGun
    {
        get { return idGun; }
    }
    private float hp = -1;
    public float Hp
    {
        get { return hp; }
        set { hp = value; }
    }
    /// <summary>
    /// link monster near
    /// </summary>
    private GameObject monster;

    private bool isDead = false;
    public bool IsDead
    {
        get { return isDead; }
        set { isDead = value; }
    }

    /// <summary>
    /// create instantiate bullet if has ammo
    /// </summary>
    public void BtnShoot()
    {
        if (GetComponent<Backpack>().IsAmmoShoot(1))
        {
            GameObject bullet = Instantiate(prefabBullet);
            bullet.GetComponent<BulletControl>().Init(transform.position, monster.transform.position, damageAk47, gameObject);
        }
    }

    /// <summary>
    /// reset link on monster, btn shoot deactive
    /// </summary>
    public void MonsterDie()
    {
        monster = null;
        btnShoot.GetComponent<Button>().interactable = false;
    }

    /// <summary>
    /// start animation die, btn off
    /// </summary>
    private void Die()
    {
        StopVelocity();
        animator.SetBool("Die", true);
        isDead = true;
        Transform obj = GameObject.FindGameObjectWithTag("BtnGame").transform;
        int childCount = obj.childCount;
        for (int i = 0; i < childCount; i++) 
            obj.GetChild(i).gameObject.SetActive(false);
    }

    private void EditHealthPointGameObject()
    {
        hpGO.transform.GetChild(0).GetComponent<RectTransform>().offsetMax = new Vector2(hp / maxhp, 0); 
    }

    /// <summary>
    /// damage player, edit healt point
    /// </summary>
    /// <param name="damage"></param>
    public void Damage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
            Die();
        EditHealthPointGameObject();
    }

    /// <summary>
    /// start animation with gun
    /// </summary>
    /// <param name="_id">id item gun</param>
    public void AddGunID(int _id)
    {
        idGun = _id;
        gunGO.SetActive(true);
        animator.SetInteger("TypeGun", 1);
        if (idGun == -1)
        {
            animator.SetInteger("TypeGun", 0);
            gunGO.SetActive(false);
        }
    }

    /// <summary>
    /// stop velocity rigidBody2D
    /// </summary>
    public void StopVelocity()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
    }

    /// <summary>
    /// animation stand
    /// </summary>
    public void Stand()
    {
        animator.SetBool("Stand", true);
    }

    /// <summary>
    /// mooving player on vector, start animation run
    /// </summary>
    /// <param name="vector">vector direction movement</param>
    public void Moove(Vector3 vector)
    {
        animator.SetBool("Stand", false);
        vector = vector.normalized;
        transform.position = new Vector3(transform.position.x + vector.x * speed * Time.deltaTime,
            transform.position.y + vector.y * speed * Time.deltaTime,
            transform.position.z);
        CamAlig();
        if ((vector.x > 0 && leftPlayer) || (vector.x < 0 && !leftPlayer))
            Flip();        
    }

    /// <summary>
    /// point camera the player
    /// </summary>
    public void CamAlig()
    {
        targetCamera.position = new Vector3(transform.position.x,
            transform.position.y,
            targetCamera.position.z);
    }

    /// <summary>
    /// flip player
    /// </summary>
    private void Flip()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        hpGO.transform.localScale = new Vector3(hpGO.transform.localScale.x * -1, hpGO.transform.localScale.y, hpGO.transform.localScale.z);
        leftPlayer = !leftPlayer;
    }

    /// <summary>
    /// deactive btn shoot
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Monster" && idGun != -1)
            btnShoot.GetComponent<Button>().interactable = false;
    }

    /// <summary>
    /// active btn shoot
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Monster" && idGun != -1 && !collision.GetComponent<MonsterControl>().IsDead)
        {
            monster = collision.gameObject;
            btnShoot.GetComponent<Button>().interactable = true;
        }
    }

    /// <summary>
    /// set hp if no load, asign radiusAttack
    /// </summary>
    private void Awake()
    {
        if (hp == -1)
            hp = maxhp;
        gunGO.GetComponent<CircleCollider2D>().radius = radiusAttack;
    }

}
