using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plataforma : MonoBehaviour
{
    private bool applyForce;
    private bool detectingPlayer;
    private PlayerController player;

    [SerializeField] private BoxCollider2D plataformCollider;
    [SerializeField] private BoxCollider2D plataformTrigger;
    [SerializeField] private bool daSalto;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void Start()
    {
        if (!daSalto)
        {
            Physics2D.IgnoreCollision(plataformCollider, plataformTrigger, true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!daSalto)
            {
                Physics2D.IgnoreCollision(plataformCollider, player.GetComponent<CapsuleCollider2D>(), true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!daSalto)
            {
                Physics2D.IgnoreCollision(plataformCollider, player.GetComponent<CapsuleCollider2D>(), false);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            detectingPlayer = true;
            if (daSalto)
            {
                applyForce = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            detectingPlayer = false;
        }
    }

    private void Update()
    {
        if (daSalto)
        {
            if (player.transform.position.y -0.8f > transform.position.y)
            {
                plataformCollider.isTrigger = false;
            }
            else
            {
                plataformCollider.isTrigger = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (applyForce)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * 25, ForceMode2D.Impulse);
            applyForce = false;
        }
    }
}
