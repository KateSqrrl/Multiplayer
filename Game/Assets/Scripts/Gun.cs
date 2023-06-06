using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using Photon.Pun;

public class Gun : MonoBehaviourPunCallbacks
{
    public GameObject bulletPrefab;
    public Transform shotPoint;

    private float time;
    private float startTime;
    private float rotZ;

    private PhotonView _photonView;
    private Joystick joystick;


    void Start()
    {
        _photonView = GetComponent<PhotonView>();
        joystick = GameObject.FindGameObjectWithTag("ShotJoystick").GetComponent<Joystick>();

        if (!_photonView.IsMine && PhotonNetwork.IsConnected)
        {
            enabled = false;
        }
    }

    void Update()
    {
        if (!_photonView.IsMine && PhotonNetwork.IsConnected)
            return;

        rotZ = Mathf.Atan2(joystick.Vertical, joystick.Horizontal) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

        if (time <= 0)
        {
            if (joystick.Vertical != 0 || joystick.Horizontal != 0)
            {
                SpawnBullet(shotPoint.position, transform.rotation);
                time = startTime;
            }
        }
        else
        {
            time -= Time.deltaTime;
        }
    }

    void SpawnBullet(Vector3 position, Quaternion rotation)
    {
        GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, position, rotation);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.SetOwner(_photonView.Owner);
    }
}
