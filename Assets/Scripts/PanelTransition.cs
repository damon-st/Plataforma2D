using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelTransition : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void AparecerGame()
    {
        anim.SetTrigger("aparecer");
    }

    public void DefaultTransition()
    {
        anim.SetTrigger("default");
    }
}
