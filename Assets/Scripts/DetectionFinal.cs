using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionFinal : MonoBehaviour
{
    public bool avanzando;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.ActivePanelTransition();
            GameManager.Instance.avanandoNivel = avanzando;
            StartCoroutine(WaitingChangePocition());
        }
    }
    private IEnumerator WaitingChangePocition()
    {
        yield return new WaitForSeconds(0.1f);
        GameManager.Instance.ChangePositionPlayer();
        if (avanzando)
        {
            GameManager.Instance.nivelActual++;
        }
        else
        {
            GameManager.Instance.nivelActual--;
        }
    }
}
