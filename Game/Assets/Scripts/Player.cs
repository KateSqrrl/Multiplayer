using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class Player : MonoBehaviourPunCallbacks, IPunObservable
{
    public int score = 0;
    public float speed;
    public bool isDead;

    public TextMeshProUGUI coinText;
    public TextMeshProUGUI NicknameText;
    public GameObject healthBar;
    public Sprite deadPlayerSprite;
    public PhotonView _photonView;

    private float x, y;
    private float minX = -8.1f, maxX = 8f, minY = -4f, maxY = 2.3f;
    private bool isFlipping;
    private bool isRight = true;

    private Quaternion targetRotation;
    private Rigidbody2D rb;
    private Joystick joystick;
    private Joystick joystickShot;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _photonView = GetComponent<PhotonView>();
        joystick = GameObject.FindGameObjectWithTag("Joystick").GetComponent<Joystick>();
        joystickShot = GameObject.FindGameObjectWithTag("ShotJoystick").GetComponent<Joystick>();
        coinText.text = "Coins: " + score;

        joystick.enabled = false;
        joystickShot.enabled = false;


        spriteRenderer = GetComponent<SpriteRenderer>();
        NicknameText.SetText(photonView.Owner.NickName);

        if (_photonView.IsMine)
        {
            NicknameText.color = Color.green;
        }
    }

    void Update()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            if (!_photonView.IsMine || isDead)
            return;

            joystick.enabled = true;
            joystickShot.enabled = true;

            x = joystick.Horizontal * speed;
            y = joystick.Vertical * speed;
            rb.velocity = new Vector2(x, y);

            transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX),
                Mathf.Clamp(transform.position.y, minY, maxY), transform.position.z);

            if (!isDead)
            {
                if (x > 0 && !isRight)
                {
                    _photonView.RPC("Flip", RpcTarget.AllBuffered);
                }
                else if (x < 0 && isRight)
                {
                    _photonView.RPC("Flip", RpcTarget.AllBuffered);
                }
            }
        }
    }


    void FixedUpdate()
    {
        if (isFlipping)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
                isFlipping = false;
        }
    }

    public void Kill()
    {
        isDead = true;
        _photonView.RPC("Die", RpcTarget.AllBuffered);
        _photonView.RPC("DisableMovement", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void DisableMovement()
    {
        rb.simulated = false;
        rb.velocity = Vector2.zero;
    }

    [PunRPC]
    public void Die()
    {
        spriteRenderer.sprite = deadPlayerSprite;
        if (_photonView.IsMine)
        {
            joystick.enabled = false;
            joystickShot.enabled = false;
        }
    }

    [PunRPC]
    void Flip()
    {
        if (isDead)
        {
            return;
        }
        else if (!isDead)
        {
            isRight = !isRight;
            isFlipping = true;
            targetRotation = Quaternion.Euler(0f, isRight ? 0f : 180f, 0f);
            NicknameText.transform.Rotate(0, 180, 0);
            coinText.transform.Rotate(0, 180, 0);
            healthBar.transform.Rotate(0, 180, 0);
        }
        
    }


    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isFlipping);
            stream.SendNext(targetRotation);
            stream.SendNext(isDead);
            stream.SendNext(isRight); 
        }
        else
        {
            isFlipping = (bool)stream.ReceiveNext();
            targetRotation = (Quaternion)stream.ReceiveNext();
            isDead = (bool)stream.ReceiveNext();
            isRight = (bool)stream.ReceiveNext(); 
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2 && collision.CompareTag("Coin"))
        {
            score++;
            Destroy(collision.gameObject);

            coinText.text = "Coins: " + score;
        }
    }
}
