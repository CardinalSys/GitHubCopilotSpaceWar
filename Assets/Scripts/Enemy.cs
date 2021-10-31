using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //RigidBody IA enemy controller
    public Rigidbody2D rb;
    //Speed of the enemy
    public float speed = 8f;

    //Enemy life
    public float life = 10;

    //bullet prefab
    public GameObject bulletPref;
    //Player position
    public Transform player;

    public bool IA = false, canShoot = true;
    //Damage of the enemy
    public int damage = 1;

    public AudioSource audioS;

    public AudioClip shoot, hit;

    void Start()
    {
        //Get the rigidbody component
        rb = GetComponent<Rigidbody2D>();
        //Player position
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if(!IA)
        {
            horizontalMovement();
        }
        Shoot();
        audioS.volume = PlayerPrefs.GetFloat("Audio");
    }

    void Update()
    {
        if(IA)
        {
            MoveToPlayer();
        }
    }
    //Function called horizontalMovement
    void horizontalMovement()
    {
        if(!IA)
        {
            StopCoroutine(horizontalMovementCoroutine());
            //if the rigidbody speed is 0, randomly horizontal move the enemy, else call a coroutine to stop the enemy
            if (rb.velocity.x == 0)
            {
                StartCoroutine(horizontalMovementCoroutine());
            }
            else
            {
                StopCoroutine(horizontalMovementCoroutine());
            }
        }


    }

    //Function called horizontalMovementCoroutine
    IEnumerator horizontalMovementCoroutine()
    {
        //Randomly move the enemy
        if(rb.constraints != RigidbodyConstraints2D.FreezeAll)
        {
            while (true)
            {

                //Randomly move the enemy
                int random = Random.Range(0, 2);
                if (random == 0)
                {
                    rb.velocity = new Vector2(-speed, 0);
                }
                else
                {
                    rb.velocity = new Vector2(speed, 0);
                }
                //Wait random time between 1 and 3 seconds
                yield return new WaitForSeconds(Random.Range(1, 3));
                horizontalMovement();

            }
        }
    }

    //if enemy collide with the scenario, move to the opposite position of the scenario collision
    void OnCollisionEnter2D(Collision2D collision)
    {
        StopCoroutine(horizontalMovementCoroutine());
        if (collision.gameObject.tag == "Scenario")
        {
            rb.velocity = new Vector2(0, 0);
            if (collision.gameObject.transform.position.x > transform.position.x)
            {
                rb.velocity = new Vector2(-speed, 0);
                Debug.Log(rb.velocity);
            }
            else
            {
                rb.velocity = new Vector2(speed, 0);
                Debug.Log(rb.velocity);
            }
            //Call stopEnemyCoroutine
            StartCoroutine(stopEnemyCoroutine());
        }
    }

    //Function called stopEnemyCoroutine
    IEnumerator stopEnemyCoroutine()
    {
        //Wait 0.5 second
        yield return new WaitForSeconds(0.7f);
        //Stop the enemy
        rb.velocity = new Vector2(0, 0);
        horizontalMovement();
    }

    //Take damage function
    public void TakeDamage(float damage)
    {
        audioS.clip = hit;
        audioS.Play();
        //Reduce the life of the enemy
        life -= damage;
        //If the life is 0, destroy the enemy
        if (life <= 0)
        {
            Destroy(gameObject);
        }
    }

    //Make a function that randomly shoot a bullet
    public void Shoot()
    {
        //Wait a random time between 1 and 3 seconds
        if(canShoot)
        {
            StartCoroutine(ShootCoroutine());
        }
    }

    //Function called ShootCoroutine
    IEnumerator ShootCoroutine()
    {
        //Wait a random time between 1 and 3 seconds
        if(canShoot)
        {
            audioS.clip = shoot;
            audioS.Play();
            yield return new WaitForSeconds(Random.Range(1, 3));
            //Instantiate a bullet
            GameObject bullet = Instantiate(bulletPref, transform.position, new Quaternion(0,0,-90,90));
            //Set the bullet speed
            bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -10);
            //Get targetTag of the bullet script at bullet gameobject and set it to "Player"
            bullet.GetComponent<Bullet>().targetTag = "Player";
            //Set the damage of the bullet
            bullet.GetComponent<Bullet>().damage = damage;
            //Call the function Shoot
        }

        Shoot();
    }

    //Function that move the rigidbody to the x player position

    void MoveToPlayer()
    {
        //If the player position is greater than the enemy position, move the enemy to the right
        if(rb.constraints != RigidbodyConstraints2D.FreezeAll)
        {
            if (player.position.x > transform.position.x)
            {
                rb.velocity = new Vector2(speed, 0);
            }
            //If the player position is less than the enemy position, move the enemy to the left
            else if (player.position.x < transform.position.x)
            {
                rb.velocity = new Vector2(-speed, 0);
            }
            //If the player position is equal to the enemy position, stop the enemy
            else
            {
                rb.velocity = new Vector2(0, 0);
            }
        }

    }
    
}
