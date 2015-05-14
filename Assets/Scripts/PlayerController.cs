using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public float speed;
    public int currentHP = 100;

    private Rigidbody2D playerRigidBody;
    private PhotonView punView;
    private Animator animator;
    private bool isGrounded;
    public GameObject projectile;
    public GameObject projectile2;
    public GameObject deathParticles;
    public GameObject hitParticles;

    private float someScale;

    private float lastFired;

    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
        punView = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();

        someScale = transform.localScale.x; // assuming this is facing right
    }

    void Update()
    {

        if (punView.isMine)
        {
            //Testing
            ExitGames.Client.Photon.Hashtable chInfo2 = new ExitGames.Client.Photon.Hashtable();
            chInfo2 = PhotonNetwork.player.customProperties;
            Debug.Log("HeroStatus in PlayerController: " + chInfo2["hs"].ToString());
            // ROFL //
            if(currentHP <= 0)
            {
                //
                object[] paramsForRPC = new object[1];
                paramsForRPC[0] = transform.position;
                punView.RPC("PlayDeathAnimation", PhotonTargets.All, paramsForRPC);
                //
                //GameController.instance.destroyPlayerHero(gameObject); 
                /*
                ExitGames.Client.Photon.Hashtable chInfo = new ExitGames.Client.Photon.Hashtable();
                chInfo = PhotonNetwork.player.customProperties;
                chInfo["hs"] = "d";
                 * */
                ExitGames.Client.Photon.Hashtable chInfo = new ExitGames.Client.Photon.Hashtable();
                chInfo.Add("hs", "d");
                PhotonNetwork.player.SetCustomProperties(chInfo);
                //Testing
                ExitGames.Client.Photon.Hashtable chInfo1 = new ExitGames.Client.Photon.Hashtable();
                chInfo1 = PhotonNetwork.player.customProperties;
                Debug.Log("HeroStatus in PlayerController: " + chInfo1["hs"].ToString());
                //
                GameController.instance.addDeathPoint();
                //
                PhotonNetwork.Destroy(gameObject);
                Destroy(gameObject);
                Debug.Log("HeroStatus in PlayerController AFTER DESTROY: " + chInfo1["hs"].ToString());
                // Depending on the GameMode this will be changed Spawning or well staying dead
                if (GameMode.Mode == "RoundMatch") 
                {
                    
                    ExitGames.Client.Photon.Hashtable roomCusInfo = PhotonNetwork.room.customProperties;
                    //string testa = roomCusInfo["rk"].ToString();
                    //Debug.Log(roomCusInfo["rk"] + " -- " + roomCusInfo["rk"].GetType());
                    int x = Convert.ToInt32(roomCusInfo["rk"].ToString());
                    x++;
                    if ((GameMode.PlayerCount - 1) <= x)
                    {
                        // Start new Round
                        Debug.Log("New round has started");
                        // Call RPC
                        GameController.instance.photonView.RPC("GameMode_RoundMatch_RoundEnd", PhotonTargets.All);
                        // Set Round deaths to 0
                        roomCusInfo["rk"] = "0";
                    }
                    else
                    {
                        // Add the round kill to room properties
                        roomCusInfo["rk"] = x.ToString();
                    }
                    // Transfer only one parameter
                    /*ExitGames.Client.Photon.Hashtable sInfoToTransfer = new ExitGames.Client.Photon.Hashtable();
                    sInfoToTransfer.Add("rk", roomCusInfo["rk"].ToString());
                    PhotonNetwork.room.SetCustomProperties(sInfoToTransfer);*/
                    //GameController.instance.spawnPlayerHero();
                }
                //else GameController.instance.spawnPlayerHero();
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

        if(playerRigidBody.velocity.x > 0)
        {
            animator.SetBool("Running", true);
            transform.localScale = new Vector2(-someScale, transform.localScale.y);
        }
        else if(playerRigidBody.velocity.x < 0)
        {
            animator.SetBool("Running", true);
            transform.localScale = new Vector2(someScale, transform.localScale.y);
        }
        else
        {
            animator.SetBool("Running", false);
        }
        //
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            playerRigidBody.AddForce(new Vector2(0, 25), ForceMode2D.Impulse);
            isGrounded = false;
            //animator.SetBool("Jumping", true);
        }

        if(Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (Time.time - lastFired > 0.8f)
            {
                //Quaternion projectileRotation = Quaternion.FromToRotation(transform.position,
                //Input.mousePosition);

                //Angle the projectile towards the mouse
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = 10;
                Vector3 playerPos = Camera.main.WorldToScreenPoint(transform.FindChild("ProjectileStartingPoint").transform.position);
                mousePos.x = mousePos.x - playerPos.x;
                mousePos.y = mousePos.y - playerPos.y;
                float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

                Quaternion projectileRotation = Quaternion.Euler(new Vector3(0, 0, angle));

                //PUN RPC Call
                object[] paramsForRPC = new object[4];

                paramsForRPC[0] = transform.FindChild("ProjectileStartingPoint").transform.position;
                paramsForRPC[1] = projectileRotation;
                paramsForRPC[2] = punView.ownerId;
                paramsForRPC[3] = this.projectile.name;

                animator.SetTrigger("attack");

                punView.RPC("FireProjectile", PhotonTargets.All, paramsForRPC);
                lastFired = Time.time;
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Time.time - lastFired > 0.8f)
            {
                //Quaternion projectileRotation = Quaternion.FromToRotation(transform.position,
                //Input.mousePosition);

                //Angle the projectile towards the mouse
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = 10;
                Vector3 playerPos = Camera.main.WorldToScreenPoint(transform.FindChild("ProjectileStartingPoint").transform.position);
                mousePos.x = mousePos.x - playerPos.x;
                mousePos.y = mousePos.y - playerPos.y;
                float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

                Quaternion projectileRotation = Quaternion.Euler(new Vector3(0, 0, angle));

                //PUN RPC Call
                object[] paramsForRPC = new object[4];

                paramsForRPC[0] = transform.FindChild("ProjectileStartingPoint").transform.position;
                paramsForRPC[1] = projectileRotation;
                paramsForRPC[2] = punView.ownerId;
                paramsForRPC[3] = this.projectile2.name;

                //animator.SetTrigger("attack");

                punView.RPC("FireProjectile", PhotonTargets.All, paramsForRPC);
                lastFired = Time.time;
            }
        }
    }

    [RPC] void PlayDeathAnimation(Vector3 pos)
    {
        Instantiate(deathParticles, pos, Quaternion.identity);
    }

    [RPC]public void ProjectileHit(int damage, int playerHitId, Vector3 positionOfImpact, int projectileOwnerPlayerId)
    {
        Instantiate(hitParticles, positionOfImpact, Quaternion.identity);

        //If I am the player who got hit
        if (punView.ownerId == playerHitId)
        {
            this.currentHP -= damage;
            if(this.currentHP <= 0)
            {
                GameController.instance.addKillPoint(projectileOwnerPlayerId);
            }
        
        }
        //Destroy()
    }

    [RPC] void FireProjectile(Vector3 pos, Quaternion rot, int ownerId, string projectileName)
    {
        GameObject tmpProjectile = null;

        if (projectileName == "DeathBolt")
            tmpProjectile = Instantiate(projectile2, pos, rot) as GameObject;
        else if (projectileName == "FireBolt")
            tmpProjectile = Instantiate(projectile, pos, rot) as GameObject;

        animator.SetTrigger("attack");
         
        tmpProjectile.GetComponent<Projectile>().ownerId = ownerId;
    }


    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            isGrounded = true;
            //animator.SetBool("Jumping", false);
        }
    }
}
