using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 40;
    public Sprite EnemyBullet, PlayerBullet,impactEffect;

    public bool hitted = false;
    //tag
    public string targetTag;

    //start
    void Start()
    {
        //if targetTag is "Player" the image will be red
        if (targetTag == "Player")
        {
            GetComponent<SpriteRenderer>().sprite = EnemyBullet;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = PlayerBullet;
        }
    }
    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if(hitInfo.tag == targetTag && !hitted)
        {
            hitted = true;
            //if targetTag is enemy
            if(targetTag == "Enemy")
            {
                hitInfo.GetComponent<Enemy>().TakeDamage(damage);
            }
            else
            {
                if(!hitInfo.GetComponent<Player>().shieldEquippedBool)
                {
                    hitInfo.GetComponent<Player>().TakeDamage(damage);
                }
            }
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            GetComponent<SpriteRenderer>().sprite = impactEffect;
            StartCoroutine(DestroyBullet());
        }
        //Instantiate(impactEffect, transform.position, transform.rotation);
        if(hitInfo.tag == "Scenario")
        {
            Destroy(gameObject);
        }

    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}

