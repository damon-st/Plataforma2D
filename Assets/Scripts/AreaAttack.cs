using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if(collision.name == "Bat")
            {
                collision.GetComponent<EnemyBat>().ReceiveDamange();
            }
            else if(collision.name == "Skeleton")
            {
                collision.GetComponent<EnemySkeleton>().ReceiveDamange();
            }
            else if(collision.name == "Spider")
            {
                collision.GetComponent<Waypoints>().ReceiveDamange();
            }
        }
        else if(collision.CompareTag("Destruible"))
        {
            collision.GetComponent<Animator>().SetBool("destruir", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }
}
