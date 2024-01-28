using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Singleton
    public static PlayerManager instance;
    public Vector3 respawnPoint;
    #endregion

    public GameObject player;
    private void Awake()
    {
        instance = this;
        respawnPoint = new Vector3 (0, 10, 0);
    }

    public void RespawnPlayer()
    {
        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = respawnPoint;
        player.GetComponent<CharacterController>().enabled = true;
        player.GetComponent<PlayerCombat>().RevivePlayer();
        Debug.Log("Respawn player");
    }
}
