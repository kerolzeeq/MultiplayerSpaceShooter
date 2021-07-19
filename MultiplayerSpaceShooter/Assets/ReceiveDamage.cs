using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Networking;


public class ReceiveDamage : NetworkBehaviour
{
    [SerializeField]
    private int maxHealth = 10;

    [SyncVar]
    private int currentHealth;

    [SerializeField]
    private string enemyTag;

    [SerializeField]
    private bool destroyOnDeath;

    private Vector2 initialPosition;

    public GameObject effect;
    void Start()
    {
        this.currentHealth = this.maxHealth;
        this.initialPosition = this.transform.position;
    }

     void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == this.enemyTag)
        {
            this.TakeDamage(1);
            Destroy(collider.gameObject);
        }
    }

    void TakeDamage(int amount)
    {
        if (this.isServer)
        {
            this.currentHealth -= amount;

            if (this.currentHealth <= 0)
            {
                if (this.destroyOnDeath)
                {
                    CmdDeathAnimation();
                    
                }
                else
                {
                    this.currentHealth = this.maxHealth;
                    RpcRespawn();
                }
            }
        }
    }

    [ClientRpc]
    void CmdDeathAnimation()
    {
        GameObject particle = Instantiate(effect, this.transform.position, Quaternion.identity);
        NetworkServer.Spawn(particle);
        Destroy(this.gameObject);
    }

    [ClientRpc]
    void RpcRespawn()
    {
        this.transform.position = this.initialPosition;
    }
}
