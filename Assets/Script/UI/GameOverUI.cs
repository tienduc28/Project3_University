using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    GameObject gameoverPanel;

    private void Awake()
    {
        gameoverPanel = GameObject.Find("GameoverPanel");
        gameoverPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerManager.instance.player.GetComponent<PlayerCombat>().isDead)
        {
            gameoverPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void Respawn()
    {
        PlayerManager.instance.RespawnPlayer();
        gameoverPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }
}
