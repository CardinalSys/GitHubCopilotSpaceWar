using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Debug.Log("Enemy hit");
            //Enemy Take Damage
            other.gameObject.GetComponent<Enemy>().TakeDamage(0.2f);
        }
        else if(other.gameObject.tag == "Bullet")
        {
            Debug.Log("Bullet hit");
            //Bullet Destroy
            Destroy(other.gameObject);
        }
    }

}
