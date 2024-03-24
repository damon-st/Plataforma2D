using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkeleton : MonoBehaviour
{

    private PlayerController player;
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    private Animator animator;
    private CinemachineVirtualCamera cm;
    private bool applyForce;
    private bool agitando;

    [Header("Config")]
    [SerializeField] private float distanceDectectionPlayer=17;
    [SerializeField] private float distanceDetectionArrow = 11;
    [SerializeField] private GameObject arrow;
    [SerializeField] private float forceLaunch = 5;
    [SerializeField] private float velocityMove;
    [SerializeField] private int lives=3;
    [SerializeField] private bool launchingArrow;


    private int hashWalinkgA = Animator.StringToHash("walking");
    private int hashShootingA = Animator.StringToHash("shooting");

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        cm = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = "Skeleton";
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized * distanceDetectionArrow;
        Debug.DrawRay(transform.position,direction,Color.red);

        float distanceCurrent = Vector2.Distance(transform.position, player.transform.position);
        if(distanceCurrent <= distanceDetectionArrow)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool(hashWalinkgA, false);

            Vector2 directionNormalized = direction.normalized;
            ChangeView(directionNormalized.x);
            if (!launchingArrow)
            {
                StartCoroutine(LaunchingArrow(direction, distanceCurrent));
            }
        }
        else
        {
            if(distanceCurrent <= distanceDectectionPlayer)
            {
                Vector2 move = new Vector2(direction.x,0).normalized;
                rb.velocity = new Vector2(move.x * velocityMove,rb.velocity.y);
                animator.SetBool(hashWalinkgA, true);
                ChangeView(move.x);
            }else
            {
                animator.SetBool(hashWalinkgA, false);
            }
        }
    }

    private void ChangeView(float directionX)
    {
        if (directionX < 0 && transform.localScale.x >0) 
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if(directionX > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanceDectectionPlayer);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanceDetectionArrow);
    }

    private IEnumerator LaunchingArrow(Vector2 directionArrow,float distance)
    {
        launchingArrow = true;
        animator.SetBool(hashShootingA, true);
        yield return new WaitForSeconds(1.42f);
        animator.SetBool(hashShootingA, false);
        directionArrow =  ((player.transform.position - transform.position).normalized * distanceDetectionArrow).normalized;
        GameObject arrowG = Instantiate(arrow, transform.position, Quaternion.identity);
        Arrow arrowCtrl = arrowG.transform.GetComponent<Arrow>();
        arrowCtrl.directionArrow = directionArrow;
        arrowCtrl.skeleton = this.gameObject;
        arrowG.GetComponent<Rigidbody2D>().velocity = directionArrow * forceLaunch;
        
        launchingArrow = false;
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
            velocityMove = 0;
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
            // Dead();
            agitando = false;
        }

    }

    private IEnumerator UltimoAgitarCamera(float time)
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.ReciveDamange((transform.position- player.transform.position).normalized);
        }
    }

    private void FixedUpdate()
    {
        if (applyForce)
        {
            rb.AddForce((transform.position - player.transform.position).normalized * 100, ForceMode2D.Impulse);
            applyForce = false;
        }
    }
}
