using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerBonus : MonoBehaviour
{
    public int timeBonus;
    GameManager gm;
    GameObject gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager");
        gm = gameManager.GetComponent<GameManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.tag == "Player")
        {
            gm.timer = gm.timer + timeBonus;  
        }
        Destroy(gameObject);
    }
}
