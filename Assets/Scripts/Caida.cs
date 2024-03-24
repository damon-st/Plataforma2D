using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caida : MonoBehaviour
{
    [SerializeField] private GameObject checkPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.position = checkPoint.transform.position;
            collision.GetComponent<PlayerController>().ReciveDamange();
        }
    }
}
