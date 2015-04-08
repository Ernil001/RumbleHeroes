using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public float speed;
    public int currentHP = 100;

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
            // ROFL //
            if(currentHP <= 0)
            {
                PhotonNetwork.Destroy(gameObject);
                Destroy(gameObject);
            }
            // ENDROFL //
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
            playerRigidBody.AddForce(new Vector2(0, 25), ForceMode2D.Impulse);
            isGrounded = false;
        }

        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            //Quaternion projectileRotation = Quaternion.FromToRotation(transform.position,
                //Input.mousePosition);

            //Angle the projectile towards the mouse
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10;
            Vector3 playerPos = Camera.main.WorldToScreenPoint(transform.position);
            mousePos.x = mousePos.x - playerPos.x;
            mousePos.y = mousePos.y - playerPos.y;
            float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

            Quaternion projectileRotation = Quaternion.Euler(new Vector3(0, 0, angle));

            //PUN RPC Call
            object[] paramsForRPC = new object[3];

            paramsForRPC[0] = transform.position;
            paramsForRPC[1] = projectileRotation;
            paramsForRPC[2] = punView.ownerId;


            punView.RPC("FireProjectile", PhotonTargets.All, paramsForRPC);
        }
    }

    /*[RPC] void ChangePlayerName(string newName)
    {
        Debug.Log("In OnChangePlayerName");
        gameObject.name = newName;
    }*/

    [RPC] void hpERNIL(int damage, int ownerId)
    {
        if(punView.ownerId == ownerId)
            this.currentHP -= damage;
    }

    [RPC] void FireProjectile(Vector3 pos, Quaternion rot, int ownerId)
    {
        Debug.Log("In Projectile");
        GameObject tmpProjectile = Instantiate(projectile, pos, rot) as GameObject;
        tmpProjectile.GetComponent<FireBolt>().ownerId = ownerId;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }
}
