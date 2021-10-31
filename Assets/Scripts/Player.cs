using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //References
    public Rigidbody2D rb;
    public GameObject bulletPref;
    public GameObject rayEquipped, shieldEquipped;
    public GameObject laserBeam, startLaserBeam;

    public GameObject RestartButton, ExitButton;

    public AudioSource audioS;

    public AudioClip shoot, hit, pickUp;

    //Variables
    public float speed;
    public float fireRate = 0.5f;
    public int health = 100;
    public bool rayEquippedBool = false, shieldEquippedBool = false;
    public bool colliding = false, dead = false;


    void Start()
    {
        //reference to the rigidbody
        rb = GetComponent<Rigidbody2D>();
        audioS.volume = PlayerPrefs.GetFloat("Audio");
    }


    void Update()
    {
        //horizontal movement

        float moveHorizontal = Input.GetAxis("Horizontal");
        Vector2 movement = new Vector2(moveHorizontal, 0);
        rb.velocity = movement * speed;

        //Call Shoot function when left mouse button is clicked and fireRate float is 0.5f
        if (Input.GetButtonDown("Fire1") && fireRate >= 0.5f && !rayEquippedBool && !dead)
        {
            fireRate = 0;
            Shoot();
        }

        //if fireRate is 0, increase him with Time.deltaTime
        if (fireRate < 0.5f)
        {
            fireRate += Time.deltaTime;
        }

        Ray();
    }

    //Make a Shoot function
    void Shoot()
    {
        //make a bullet
        audioS.clip = shoot;
        audioS.Play();
        GameObject bullet = Instantiate(bulletPref, transform.position, bulletPref.transform.rotation);
        //make a bullet move
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 10);
        //Get targetTag of the bullet script at bullet gameobject and set it to "Enemy"
        bullet.GetComponent<Bullet>().targetTag = "Enemy";
    }

    //Take Damage function
    public void TakeDamage(int damage)
    {
        //if health is less than 0, destroy the game object
        audioS.clip = hit;
        audioS.Play();
        if (health <= 0)
        {
            health = 0;
            RestartButton.SetActive(true);
            ExitButton.SetActive(true);
            Rigidbody2D Enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Rigidbody2D>();
            if(Enemy != null)
            {
                Enemy.constraints = RigidbodyConstraints2D.FreezeAll;
                Enemy.gameObject.GetComponent<Enemy>().canShoot = false;
                Enemy.gameObject.GetComponent<Enemy>().StopAllCoroutines();
                StartCoroutine(Unfreeze());
            }
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            dead = true;
        }
        else
        {
            //decrease health by damage
            health -= damage;
        }
    }

    //OnTriggerEnter2D function
    void OnTriggerEnter2D(Collider2D other)
    {
        //if the other game object name is "Ray"
        if (other.name.Contains("Ray"))
        {
            audioS.clip = pickUp;
            audioS.Play();
            Debug.Log("Ray");
            //set rayEquippedBool to true
            rayEquippedBool = true;
            rayEquipped.SetActive(true);
            Destroy(other.gameObject);
            //make a coroutine for set rayEquippedBool to false after 5 seconds
            StartCoroutine(Unequipped(rayEquipped));
        }
        //else if other contains "Shield"
        else if (other.name.Contains("Shield"))
        {
            audioS.clip = pickUp;
            audioS.Play();
            Debug.Log("Shield");
            shieldEquippedBool = true;
            shieldEquipped.SetActive(true);
            Destroy(other.gameObject);
            StartCoroutine(Unequipped(shieldEquipped));
        }
        else if(other.name.Contains("Ice"))
        {
            audioS.clip = pickUp;
            audioS.Play();
            Debug.Log("Ice");
            Destroy(other.gameObject);
            Rigidbody2D Enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Rigidbody2D>();
            if(Enemy != null)
            {
                Enemy.constraints = RigidbodyConstraints2D.FreezeAll;
                Enemy.gameObject.GetComponent<Enemy>().canShoot = false;
                Enemy.gameObject.GetComponent<Enemy>().StopAllCoroutines();
                StartCoroutine(Unfreeze());
            }

        }
    }
    //Ray function
    public void Ray()
    {
        //if rayEquippedBool is true and mouse left button is pressed
        if (rayEquippedBool == true && Input.GetButton("Fire1"))
        {
            startLaserBeam.SetActive(true);
            laserBeam.SetActive(true);
        }
        if(!Input.GetButton("Fire1"))
        {
            laserBeam.SetActive(false);
            startLaserBeam.SetActive(false);
        }
    }

    //RayEquipped function
    IEnumerator Unequipped(GameObject obj)
    {
        //wait for 5 seconds
        yield return new WaitForSeconds(5);
        obj.SetActive(false);
        //set rayEquippedBool to false
        rayEquippedBool = false;
        shieldEquippedBool = false;
    }

    IEnumerator Unfreeze()
    {
        yield return new WaitForSeconds(5);
        Rigidbody2D Enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Rigidbody2D>();
        if(Enemy != null)
        {
            Enemy.constraints = RigidbodyConstraints2D.None;
            Enemy.constraints = RigidbodyConstraints2D.FreezePositionY;
            Enemy.constraints = RigidbodyConstraints2D.FreezeRotation;
            Enemy.gameObject.GetComponent<Enemy>().canShoot = true;
        }
    }

}
