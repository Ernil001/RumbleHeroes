using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public float speed;
    private Rigidbody2D playerRigidBody;
    private PhotonView punView;
    private bool isGrounded;
    public GameObject projectile;


    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
        punView = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (punView.isMine)
        {
            InputMovement();
        }
    }

    void InputMovement()
    {
        Vector2 curVel = playerRigidBody.velocity;
        curVel.x = (float)(Input.GetAxis("Horizontal") * speed);
        playerRigidBody.velocity = curVel;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            playerRigidBody.AddForce(new Vector2(0, 14), ForceMode2D.Impulse);
            isGrounded = false;
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            Instantiate(projectile, transform.position, Quaternion.identity);
        }
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }
}
