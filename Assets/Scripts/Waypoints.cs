using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{

    private Vector3 direction;
    private PlayerController player;
    private CinemachineVirtualCamera cm;
    private Rigidbody2D rb;
    private int indexCurrent = 0;
    private SpriteRenderer sp;
    private bool applyForce;
    private bool agitando;


    [SerializeField] private int lives = 3;
    [SerializeField] private Vector2 pocitionHead;
    [SerializeField] private float velocityDisplacement;
    [SerializeField] private List<Transform> points = new();
    [SerializeField] private bool waiting;
    [SerializeField] private float timeWaiting;
    [SerializeField] private float forceImpact;


    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        cm = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        if (gameObject.CompareTag("Enemy"))
        {
            gameObject.name = "Spider";
        }
    }

    private void FixedUpdate()
    {
        MoveWaypoints();
        if (gameObject.CompareTag("Enemy"))
        {
            ChangedScaleEnemy();
        }
        if (applyForce)
        {
            rb.AddForce((transform.position - player.transform.position).normalized * forceImpact, ForceMode2D.Impulse);
            applyForce = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Enemy"))
        {
            if(player.transform.position.y - 0.7f > transform.position.y + pocitionHead.y)
            {
                player.GetComponent<Rigidbody2D>().velocity = Vector2.up*player.ForceJumping;
                Destroy(gameObject, 0.2f);
            }
            else
            {
                player.ReciveDamange(-(player.transform.position-transform.position).normalized);
            }
        }
        else if(collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Plataforma"))
        {
            if(player.transform.position.y - 0.7f > transform.position.y)
            {
                player.transform.parent = transform;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Plataforma"))
        {
            if (player.transform.position.y - 0.7f > transform.position.y)
            {
                player.transform.parent = null;
            }
        }
    }

    private void ChangedScaleEnemy()
    {
        if (direction.x < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (direction.x > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void MoveWaypoints()
    {
        Vector3 tempP = points[indexCurrent].position;
        if (!waiting)
        {
            direction = (tempP - transform.position).normalized;
            transform.position = (Vector2.MoveTowards(transform.position, tempP, velocityDisplacement * Time.deltaTime));
            if (Vector2.Distance(transform.position, tempP) <= 0.7f)
            {
                StartCoroutine(Waiting());
            }
        }
    }

    private IEnumerator Waiting()
    {
        waiting = true;
        yield return new WaitForSeconds(timeWaiting);
        indexCurrent++;
        if(indexCurrent> points.Count-1)
        {
            indexCurrent = 0;
        }
        waiting = false;
    }


    public void ReceiveDamange()
    {
        if (lives > 0)
        {
            StartCoroutine(EfectDamange());
            applyForce = true;
            lives--;
        }
        else
        {
            StartCoroutine(UltimoAgitarCamera(0.1f));
        }
    }

    private void Dead()
    {
        if (lives <= 0)
        {
            velocityDisplacement = 0;
            rb.velocity = Vector2.zero;
            Destroy(gameObject, 0.2f);
        }
    }

    private IEnumerator AgitarCamera(float time)
    {
        if (!agitando)
        {
            agitando = true;
            CinemachineBasicMultiChannelPerlin cinemachineBaseicMultiChannelPerlin = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineBaseicMultiChannelPerlin.m_AmplitudeGain = 5;
            yield return new WaitForSeconds(time);
            cinemachineBaseicMultiChannelPerlin.m_AmplitudeGain = 0;
            agitando = false;
        }
    }

    private IEnumerator UltimoAgitarCamera(float time)
    {
        if (!agitando)
        {
            transform.localScale = Vector3.zero;
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
