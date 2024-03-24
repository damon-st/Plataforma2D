using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBat : MonoBehaviour
{

    private CinemachineVirtualCamera cm;
    private SpriteRenderer sp;
    private PlayerController player;
    private Rigidbody2D rb;
    private bool applyForce;
    private bool agitando;

    [SerializeField] private float velocityMove = 3;
    [SerializeField] private float radioDetection = 15;
    [SerializeField] private LayerMask layerPlayer;
    [SerializeField] private Vector2 pocitionHead;
    [SerializeField] private int lives;
    [SerializeField] private string name;

    private void Awake()
    {
        cm = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        sp = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }


    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = name;

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioDetection);

        Gizmos.color = Color.green;
        Gizmos.DrawCube((Vector2)transform.position+pocitionHead, new Vector2(1, 0.5f) * 0.7f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = player.transform.position - transform.position;
        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance <= radioDetection)
        {
            rb.velocity = direction.normalized * velocityMove;
            ChangeVista(direction.normalized.x);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        
    }

    private void ChangeVista(float directionX)
    {
        if (directionX <0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (directionX>0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if ((transform.position.y + pocitionHead.y) < player.transform.position.y -0.7f )
            {
                player.GetComponent<Rigidbody2D>().velocity = Vector2.up*player.ForceJumping;
                StartCoroutine(AgitarCamara(0.1f));
                Destroy(gameObject,0.2f);
            }
            else
            {
             player.ReciveDamange((transform.position- player.transform.position).normalized);
                
            }
        }
    }

    private void FixedUpdate()
    {
        if (applyForce)
        {
            rb.AddForce((transform.position - player.transform.position).normalized*100,ForceMode2D.Impulse);
            applyForce = false;
        }   
    }

    public void ReceiveDamange()
    {
        StartCoroutine(AgitarCamara(0.1f));
        if (lives > 0)
        {
            StartCoroutine(EfectDamange());
            applyForce = true;
            lives--;
        }
    }
    private void Dead()
    {
        if (lives <= 0)
        {
            Destroy(gameObject, 0.2f);

        }
    }

    private IEnumerator AgitarCamara(float time)
    {
        if (!agitando)
        {
            agitando = true;
            CinemachineBasicMultiChannelPerlin cinemachineBaseicMultiChannelPerlin = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineBaseicMultiChannelPerlin.m_AmplitudeGain = 5;
            yield return new WaitForSeconds(time);
            cinemachineBaseicMultiChannelPerlin.m_AmplitudeGain = 0;
            agitando = false;
            Dead();
        }
     
    }

    private IEnumerator EfectDamange()
    {
        sp.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        sp.color = Color.white;
    }
}
