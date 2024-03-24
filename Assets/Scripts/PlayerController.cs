using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using static Cinemachine.CinemachineImpulseManager.ImpulseEvent;

public class PlayerController : MonoBehaviour
{

    [Header("Estadisticas")]
    [SerializeField] private float velocityMove = 10f;
    [SerializeField] private float forceJumping = 5f;
    [SerializeField] private float velocityDash = 20f;
    [SerializeField] private float velocityDeslizar = 5f;
    [SerializeField] public int lives = 3;
    [SerializeField] private float timeInmortalidad = 3;

    private Rigidbody2D rb;
    private Vector2 direction;
    private Animator animator;
    private Vector2 directionMove;
    private Vector2 directionDamange;
    private bool isBloking;
    private GrayCamera gc;
    private SpriteRenderer sp;
    private int directionX;
    private CapsuleCollider2D collider;
    private GameObject lastEnemy;

    private float velocityMoveAxuliar;

    [SerializeField] private CinemachineVirtualCamera cm;

    [Header("Colisiones")]
    [SerializeField] private LayerMask layerFloor;
    [SerializeField] private Vector2 down, right, left;
    [SerializeField] private float radioCollision;

    [Header("Booleas")]
    [SerializeField] private bool canBeMove = true;
    [SerializeField] private bool inFloor = true;
    [SerializeField] private bool canBeDash = false;
    [SerializeField] private bool doingDash = false;
    [SerializeField] private bool touchedFloor = false;
    [SerializeField] private bool doingShake = false;
    [SerializeField] private bool doingAttacking = false;
    [SerializeField] private bool enMuro = false;
    [SerializeField] private bool muroRight = false;
    [SerializeField] private bool muroLeft = false;
    [SerializeField] private bool agarrarse = false;
    [SerializeField] private bool jumpingMuro = false;
    [SerializeField] private bool isInmortal = false;
    [SerializeField] private bool applyForce = false;
    [SerializeField] public bool finishMap = false;
    [SerializeField] private bool agachandose = false;


    private int hashJumpingA = Animator.StringToHash("Jumping");
    private int hashWalkinggA = Animator.StringToHash("Walking");
    private int hashVelocityVA = Animator.StringToHash("VelocityVertical");
    private int hashDashA = Animator.StringToHash("Dash");
    private int hashAttackX = Animator.StringToHash("AtaqueX");
    private int hashAttackY = Animator.StringToHash("AtaqueY");
    private int hashAttack = Animator.StringToHash("Atacar");
    private int hashEscalar = Animator.StringToHash("Escalar");
    private int hashVelocity = Animator.StringToHash("Velocidad");
    private int hashAgachadoA = Animator.StringToHash("agachado");


    public float ForceJumping => forceJumping;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        cm = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        gc = Camera.main.GetComponent<GrayCamera>();
        sp = GetComponent<SpriteRenderer>();

        velocityMoveAxuliar = velocityMove;
        collider= GetComponent<CapsuleCollider2D>();
    }

    public void ChangeAnimWalking(bool value)
    {
        animator.SetBool(hashWalkinggA, value);
    }

    public void MoveFinalMap(int directionX)
    {
        this.directionX = directionX;
        finishMap = true;
        ChangeAnimWalking(true);
        if (directionX < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (directionX > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!finishMap)
        {
            Move();
            Agagress();
        }else
        {
            rb.velocity = new Vector2(directionX * velocityMove, rb.velocity.y);
        }


        if (!isInmortal && lastEnemy !=null)
        {
            Physics2D.IgnoreCollision(lastEnemy.GetComponent<Collider2D>(), collider, false);
            lastEnemy = null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (isInmortal)
            {
                lastEnemy=collision.gameObject;
                Physics2D.IgnoreCollision(lastEnemy.GetComponent<Collider2D>(),collider,true);
            }
        }
    }

    public void SetBlockingTrue()
    {
        isBloking = true;
    }

    public void Dead()
    {
        if (lives > 0) return;
        GameManager.Instance.GameOver();
        enabled = false;
    }
    public void ReciveDamange()
    {
        StartCoroutine(ImpactDamange(Vector2.zero));
    }

    public void ReciveDamange(Vector2 direction)
    {
        StartCoroutine(ImpactDamange(direction));
    }

    private IEnumerator ImpactDamange(Vector2 directionDamange)
    {
        if (!isInmortal)
        {
            //  StartCoroutine(Inmortalidad());
            lives--;
            gc.enabled = true;
            float velicityAuxl = velocityMove;
            this.directionDamange = directionDamange;
            applyForce = true;
            Time.timeScale = 0.4f;
            FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));
            StartCoroutine(AgitarCamara());
            yield return new WaitForSeconds(0.2f);
            Time.timeScale = 1f;
            gc.enabled = false;
            UpdateLivesUI(1);
            velocityMove = velicityAuxl;
            Dead();
        }
    }

    public void ShowLivesUI()
    {
        for (int i = 0; i < GameManager.Instance.vidasUI.transform.childCount; i++)
        {
            GameManager.Instance.vidasUI.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void UpdateLivesUI(int livesDiscount)
    {
        int livesD = livesDiscount;
        for (int i = GameManager.Instance.vidasUI.transform.childCount - 1; i >= 0; i--)
        {
            if (GameManager.Instance.vidasUI.transform.GetChild(i).gameObject.activeInHierarchy && livesD != 0)
            {
                GameManager.Instance.vidasUI.transform.GetChild(i).gameObject.SetActive(false);
                livesD--;
            }
            else
            {
                if (livesD == 0)
                {
                    break;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (applyForce)
        {
            velocityMove = 0;
            rb.velocity = Vector2.zero;
            rb.AddForce(-directionDamange * 25, ForceMode2D.Impulse);
            applyForce = false;
        }
    }

    public void DarInmortalidad()
    {
        StartCoroutine(Inmortalidad());
    }

    private IEnumerator Inmortalidad()
    {
        isInmortal = true;
        float timeTranscurrido = 0;
        while (timeTranscurrido < timeInmortalidad)
        {
            sp.color = new Color(1, 1, 1, 0.5f);
            yield return new WaitForSeconds(timeInmortalidad / 20);
            sp.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(timeInmortalidad / 20);
            timeTranscurrido = timeInmortalidad / 10;

        }
        isInmortal = false;
    }


    private void Attack(Vector2 direction)
    {
        if (!Input.GetKeyDown(KeyCode.Z)) return;
        if (doingAttacking && doingShake) return;
        doingAttacking = true;
        animator.SetFloat(hashAttackX, direction.x);
        animator.SetFloat(hashAttackY, direction.y);
        animator.SetBool(hashAttack, true);
        Debug.Log("ATTACL");
    }

    public void FinishAttack()
    {
        animator.SetBool(hashAttack, false);
        doingAttacking = false;
        isBloking = false;
    }

    private Vector2 DirectionAttack(Vector2 directionMove, Vector2 direction)
    {
        if (rb.velocity.x == 0 && direction.y != 0) return new Vector2(0, direction.y);
        return new Vector2(directionMove.x, direction.y);
    }


    private IEnumerator AgitarCamara(float time = 0.3f)
    {
        doingShake = true;
        CinemachineBasicMultiChannelPerlin cinemachineBaseicMultiChannelPerlin = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBaseicMultiChannelPerlin.m_AmplitudeGain = 5;
        yield return new WaitForSeconds(time);
        cinemachineBaseicMultiChannelPerlin.m_AmplitudeGain = 0;
        doingShake = false;
    }

    private void Dash(float x, float y)
    {
        animator.SetBool(hashDashA, true);
        Vector2 pocitionPlayer = Camera.main.WorldToViewportPoint(transform.position);
        Camera.main.GetComponent<RippleEffect>().Emit(pocitionPlayer);
        StartCoroutine(AgitarCamara());
        canBeDash = true;
        rb.velocity = Vector2.zero;
        rb.velocity += new Vector2(x, y).normalized * velocityDash;
        StartCoroutine(PrepareDash());
    }

    private IEnumerator PrepareDash()
    {
        StartCoroutine(DashFloor());
        rb.gravityScale = 0f;
        doingDash = true;
        yield return new WaitForSeconds(0.3f);
        rb.gravityScale = 3;
        doingDash = false;
        FinishAnimDash();
    }

    private IEnumerator DashFloor()
    {
        yield return new WaitForSeconds(0.15f);
        if (inFloor)
        {
            canBeDash = false;
        }
    }

    public void FinishAnimDash()
    {
        animator.SetBool(hashDashA, false);
    }

    private void TouchFloor()
    {
        canBeDash = false;
        doingDash = false;
        animator.SetBool(hashJumpingA, false);
    }

    private void Agacharse()
    {
        agachandose = true;
        animator.SetBool(hashAgachadoA, true);
        collider.offset = new Vector2(0, 0.006196737f);
        collider.size = new Vector2(1.13f, 1.245377f);

        velocityMove = velocityMoveAxuliar / 3;

    }

    private void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");

        direction = new Vector2(x, y);
        Vector2 directionRaw = new Vector2(xRaw, yRaw);
        Walking();
        Attack(DirectionAttack(directionMove, directionRaw));

        if (Input.GetKey(KeyCode.LeftControl))
        {
            Agacharse();
            Debug.Log("AGA");
        }
        else if(agachandose)
        {
            collider.offset = new Vector2(0, 0);
            collider.size = new Vector2(1.13f, 1.58f);
            velocityMove = velocityMoveAxuliar;
            agachandose = false;
            animator.SetBool(hashAgachadoA, false);
        }

        if (inFloor && !doingDash)
        {
            jumpingMuro = false;
        }

        agarrarse = enMuro && Input.GetKey(KeyCode.LeftShift);

        if (agarrarse && !inFloor)
        {
            animator.SetBool(hashEscalar, true);
            if (rb.velocity == Vector2.zero)
            {
                animator.SetFloat(hashVelocity, 0);
            }
            else
            {
                animator.SetFloat(hashVelocity, 1);
            }
        }
        else
        {
            animator.SetBool(hashEscalar, false);
            animator.SetFloat(hashVelocity, 0);
        }

        if (agarrarse && !doingDash)
        {
            rb.gravityScale = 0;
            if (x > 0.2f || x < -0.2f)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
            float modifyVelocity = y > 0 ? 0.5f : 1;
            rb.velocity = new Vector2(rb.velocity.x, y * (velocityMove * modifyVelocity));

            if (muroLeft && transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            else if (muroRight && transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
        else
        {
            rb.gravityScale = 3f;
        }

        if (enMuro && !inFloor)
        {
            animator.SetBool(hashEscalar, true);
            if (x != 0 && !agarrarse)
            {
                DeslizarPared();
            }
        }

        ImproveJump();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (inFloor)
            {
                animator.SetBool(hashJumpingA, true);
                Jump();
            }
            if (enMuro && !inFloor)
            {
                animator.SetBool(hashEscalar, false);
                animator.SetBool(hashJumpingA, true);
                JumpingDesdeMuro();
            }
        }
        float velocity = rb.velocity.y > 0 ? 1 : -1;

        if (!inFloor)
        {
            animator.SetFloat(hashVelocityVA, velocity);
        }
        else
        {
            if (velocity == -1)
                FinishJump();
        }
        if (Input.GetKeyDown(KeyCode.X) && !doingDash && !canBeDash)
        {
            if (xRaw != 0 || yRaw != 0)
            {
                Dash(xRaw, yRaw);
            }
        }
        if (inFloor && !touchedFloor)
        {
            animator.SetBool(hashEscalar, false);
            TouchFloor();
            touchedFloor = true;
        }
        if (!inFloor && touchedFloor)
        {
            touchedFloor = false;
        }
    }

    private void DeslizarPared()
    {
        if (canBeMove)
        {
            rb.velocity = new Vector2(rb.velocity.x, -velocityDeslizar);
        }
    }

    private void JumpingDesdeMuro()
    {
        StopCoroutine(DisabledMove(0));
        StartCoroutine(DisabledMove(0.1f));

        Vector2 directionMuro = muroRight ? Vector2.left : Vector2.right;
        if (directionMuro.x < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (directionMuro.x > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        animator.SetBool(hashJumpingA, true);
        animator.SetBool(hashEscalar, false);
        Jump(Vector2.up + directionMuro, true);
        jumpingMuro = true;
    }

    private IEnumerator DisabledMove(float time)
    {
        canBeMove = false;
        yield return new WaitForSeconds(time);
        canBeMove = true;
    }



    public void FinishJump()
    {
        animator.SetBool(hashJumpingA, false);
    }

    private void ImproveJump()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (2.5f - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (2.0f - 1) * Time.deltaTime;
        }
    }

    private void Agagress()
    {
        inFloor = Physics2D.OverlapCircle((Vector2)transform.position + down, radioCollision, layerFloor);

        Collider2D collisionRigth = Physics2D.OverlapCircle((Vector2)transform.position + right, radioCollision, layerFloor);
        Collider2D collisionLefth = Physics2D.OverlapCircle((Vector2)transform.position + left, radioCollision, layerFloor);

        if (collisionRigth != null)
        {
            enMuro = !collisionRigth.CompareTag("Plataforma");
        }
        else if (collisionLefth != null)
        {
            enMuro = !collisionLefth.CompareTag("Plataforma");
        }
        else
        {
            enMuro = false;
        }

        muroRight = collisionRigth;
        muroLeft = collisionLefth;
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += Vector2.up * forceJumping;
    }

    private void Jump(Vector2 directionJump, bool muro)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += directionJump * forceJumping;
    }

    private void Walking()
    {
        if (canBeMove && !doingDash && !doingAttacking)
        {
            if (jumpingMuro)
            {
                rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(direction.x * velocityMove, rb.velocity.y)), Time.deltaTime / 2);
            }
            else
            {
                if (direction != Vector2.zero && !agarrarse)
                {
                    if (!inFloor)
                    {
                        if (agachandose)
                        {
                            animator.SetBool(hashWalkinggA, true);
                        }
                        else
                        {
                            animator.SetBool(hashJumpingA, true);
                        }
                    }
                    else
                    {
                        animator.SetBool(hashWalkinggA, true);
                    }

                    rb.velocity = new Vector2(direction.x * velocityMove, rb.velocity.y);

                    if (direction.x < 0 && transform.localScale.x > 0)
                    {
                        directionMove = DirectionAttack(Vector2.left, direction);
                        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    }
                    else if (direction.x > 0 && transform.localScale.x < 0)
                    {
                        directionMove = DirectionAttack(Vector2.right, direction);
                        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    }
                }
                else
                {
                    if (direction.y > 0 && direction.x == 0)
                    {
                        directionMove = DirectionAttack(direction, Vector2.up);
                    }
                    animator.SetBool(hashWalkinggA, false);
                }
            }

        }
        else
        {
            if (isBloking)
            {
                FinishAttack();
            }
        }

    }
}
