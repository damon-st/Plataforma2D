using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D bc;


    public LayerMask layerFloor;
    public GameObject skeleton;
    public Vector2 directionArrow;
    public float radioCollition = 0.25f;
    public bool touchedFloor;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
           collision.GetComponent<PlayerController>().ReciveDamange(-(collision.transform.position - skeleton.transform.position).normalized);
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        touchedFloor = Physics2D.OverlapCircle((Vector2)transform.position, radioCollition, layerFloor);
        if(touchedFloor)
        {
            rb.bodyType = RigidbodyType2D.Static;
            bc.enabled = false;
            enabled = false;
        }

        float angle = Mathf.Atan2(directionArrow.y, directionArrow.x)*Mathf.Rad2Deg;

        transform.localEulerAngles = new Vector3(transform.localEulerAngles.y, transform.localEulerAngles.x, angle);
    }
}
