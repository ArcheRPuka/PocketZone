using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    [SerializeField]
    private float speed = 100f;

    private Vector2 targetPos;
    public Vector2 TargetPos
    {
        get { return targetPos; }
    }

    private float damage;
    public float Damage
    {
        get { return damage; }
    }

    GameObject player;


    public void Init(Vector2 startPos, Vector2 endPos, float _damage, GameObject _player)
    {
        transform.position = startPos;
        targetPos = endPos;
        damage = _damage;
        player = _player;
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        if ((Vector2)transform.position == targetPos)
            Destroy(gameObject);
    }
    /// <summary>
    /// damage if not a dead monster
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Monster" && !collision.GetComponent<MonsterControl>().IsDead)
        {
            if (collision.GetComponent<MonsterControl>().Damage(damage))
                player.GetComponent<PlayerControl>().MonsterDie();
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// damage if not a dead monster
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Monster" && !collision.GetComponent<MonsterControl>().IsDead)
        {
            if (collision.GetComponent<MonsterControl>().Damage(damage))
                player.GetComponent<PlayerControl>().MonsterDie();
            Destroy(gameObject);
        }
    }
}
