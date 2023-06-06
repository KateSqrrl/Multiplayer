using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPunCallbacks
{
    private float bulletSpeed = 20f;
    private Photon.Realtime.Player owner;

    void Update()
    {
        if (photonView.IsMine)
            transform.Translate(Vector3.right * bulletSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!photonView.IsMine)
            return;

        PhotonView targetPhotonView = collision.GetComponent<PhotonView>();

        if (targetPhotonView != null && targetPhotonView.Owner != null && targetPhotonView.Owner != owner)
        {
            Health enemyHealth = collision.GetComponent<Health>();

            if (enemyHealth != null)
            {
                enemyHealth.LoseLifePoint();
            }

            DestroyBullet();
        }
    }

    void DestroyBullet()
    {
        if (photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }

    public void SetOwner(Photon.Realtime.Player player)
    {
        owner = player;
    }
}
