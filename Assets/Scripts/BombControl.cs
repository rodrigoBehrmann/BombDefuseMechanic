using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombControl : MonoBehaviour
{
    public static BombControl instance;

    private GameObject bomb;
    [SerializeField] private Text bombExplosion;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void BombPlanted()
    {
        bomb = GameObject.FindWithTag("Bomb");
        if (bomb)
        {
            PlaceC4.instance.timeToExplosion.text = "Bomb explosion BOOOOOOM";
            PlaceC4.instance.bombExplosed = true;
        }
        else
        {
            PlaceC4.instance.timeToExplosion.text = "nao tem bomba maifrinend";
        }
    }
}
