using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            AsignarItem();
        }
    }

    private void AsignarItem()
    {
        if (gameObject.CompareTag("Moneda"))
        {
            GameManager.Instance.UpdateCountCoins();
        }
        else if (gameObject.CompareTag("PowerUp"))
        {
            GameManager.Instance.player.DarInmortalidad();
        }

        Destroy(gameObject);
    }
}
