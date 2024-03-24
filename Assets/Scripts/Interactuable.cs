using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactuable : MonoBehaviour
{
    private bool canBeInteractue;
    private BoxCollider2D bc;
    private SpriteRenderer sp;
    private GameObject indicadorInteractuable;
    private Animator anim;

    [SerializeField] private GameObject[] objectos;
    [SerializeField] private bool esCofre;
    [SerializeField] private bool esPalanca;
    [SerializeField] private bool palacanAccionada;
    [SerializeField] private bool esCheckPoint;
    [SerializeField] private bool esSelector;

    public UnityEvent evento;

    private int hashAbrirCofreA = Animator.StringToHash("abrir");
    private int hashActivarPalancaA = Animator.StringToHash("activar");

    private void Awake()
    {
        bc = GetComponent<BoxCollider2D>();
        sp = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        if (transform.GetChild(0) != null)
        {
            indicadorInteractuable = transform.GetChild(0).gameObject;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canBeInteractue = true;
            indicadorInteractuable.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canBeInteractue = false;
            indicadorInteractuable.SetActive(false);
        }
    }

    private void Cofre()
    {
        if (esCofre)
        {
            Instantiate(objectos[Random.Range(0,objectos.Length-1)],transform.position,Quaternion.identity);
            anim.SetBool(hashAbrirCofreA, true);
            bc.enabled = false;
        }
    }

    private void Palanca()
    {
        if (esPalanca && !palacanAccionada)
        {
            anim.SetBool(hashActivarPalancaA, true);
            palacanAccionada = true;
            evento.Invoke();
            indicadorInteractuable.SetActive(false);
            enabled = false;
            bc.enabled = false;
        }
    }

    private void CheckPoint()
    {
        if (!esCheckPoint) return;
        evento.Invoke();
    }

    private void SelectLevel()
    {
        if (esSelector)
        {
            evento.Invoke();

        }
    }

    private void Update()
    {
        if (canBeInteractue && Input.GetKeyDown(KeyCode.C))
        {
            Cofre();
            Palanca();
            CheckPoint();
            SelectLevel();
        }
    }
}
