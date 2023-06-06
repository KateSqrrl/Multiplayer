using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Health : MonoBehaviourPunCallbacks
{
    public int maxHealth;
    public HealthBar healthBar;

    private int currentHealth;
    private bool isDefeated;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        photonView.RPC("SetInitialHealth", RpcTarget.OthersBuffered, currentHealth);
    }

    [PunRPC]
    private void SetInitialHealth(int health)
    {
        currentHealth = health;
        healthBar.SetHealth(currentHealth);
    }

    public void LoseLifePoint()
    {
        if (isDefeated == false && currentHealth > 0)
        {
            currentHealth--;
            healthBar.SetHealth(currentHealth);
            Debug.Log("lost 1 life point. Remaining life points: " + currentHealth);

            photonView.RPC("SetCurrentHealth", RpcTarget.OthersBuffered, currentHealth);
        }

        if (currentHealth == 0)
        {
            healthBar.SetHealth(currentHealth);
            isDefeated = true;
            Player playerScript = GetComponent<Player>();
            playerScript.Kill();
        }
    }

    [PunRPC]
    private void SetCurrentHealth(int health)
    {
        currentHealth = health;
        healthBar.SetHealth(currentHealth);
    }
}
