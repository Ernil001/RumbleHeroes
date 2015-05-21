using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public int controllingPlayer_photonID;
    public string controllingPlayer_Username;
    Transform HeroUI_ConPlUser;
    //
    public float speed;
    public int currentHP = 100;
    public int maxHP = 100;
    private Rigidbody2D playerRigidBody;
    private PhotonView punView;
    private Animator animator;
    private bool isGrounded;
    public GameObject projectile;
    public GameObject projectile2;
    /*
    public GameObject deathParticles;
    public GameObject hitParticles;
    */ 

    private float someScale;

    private float lastFired;

    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
        punView = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();

        someScale = transform.localScale.x;
        // Sets starting HP values for Hero
        if(punView.isMine) GameController.instance.setHpValues_toPlayerCustomProp(this.currentHP, this.maxHP);
        //
        this.HeroUI_ConPlUser = this.transform.FindChild("HeroUI").transform.FindChild("ControllingPlayerUsername");
        //this.HeroUI_ControllingPlayerUsername_locSc = HeroUI_ConPlUser.GetComponent<RectTransform>().localScale;
        //
        this.controllingPlayer_photonID = this.punView.ownerId;
        this.controllingPlayer_Username = this.punView.owner.name;
        if(this.controllingPlayer_Username != null) addUsername();

    }


    void Update()
    {


        if (this.GetComponent<Transform>().localScale.x < 0f && HeroUI_ConPlUser.GetComponent<RectTransform>().localScale.x > 0f)
        {

            HeroUI_ConPlUser.GetComponent<RectTransform>().localScale = new Vector3(
                (HeroUI_ConPlUser.GetComponent<RectTransform>().localScale.x * -1),
                HeroUI_ConPlUser.GetComponent<RectTransform>().localScale.y,
                HeroUI_ConPlUser.GetComponent<RectTransform>().localScale.z
            );
        }
        else if (this.GetComponent<Transform>().localScale.x > 0f && HeroUI_ConPlUser.GetComponent<RectTransform>().localScale.x < 0f)
        {
            HeroUI_ConPlUser.GetComponent<RectTransform>().localScale = new Vector3(
                Math.Abs(HeroUI_ConPlUser.GetComponent<RectTransform>().localScale.x),
                HeroUI_ConPlUser.GetComponent<RectTransform>().localScale.y,
                HeroUI_ConPlUser.GetComponent<RectTransform>().localScale.z
            );
        }
        
        //
        if (punView.isMine)
        {
            if(currentHP <= 0)
            {
                //
                object[] paramsForRPC = new object[1];
                paramsForRPC[0] = transform.position;
                //punView.RPC("PlayDeathAnimation", PhotonTargets.All, paramsForRPC);

                GameController.instance.addDeathPoint();
                // Destroy the player completly with GameController.instance.destroyPlayerHero(); after the animation ends so timeout ?
                StartCoroutine(FinishDeath());
                //GameController.instance.destroyPlayerHero();
            }
            InputMovement();
        }
    }
    IEnumerator FinishDeath()
    {
        int x = 0;
        while (x<2)
        {
            if (x == 1)
            {
                GameController.instance.destroyPlayerHero();
            }
            x++;
            yield return new WaitForSeconds(1f);
        }
    }
    //
    private void addUsername()
    {
        this.HeroUI_ConPlUser.GetComponent<Text>().text = controllingPlayer_Username;
    }
    //
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
                paramsForRPC[2] = PhotonNetwork.player.ID;
                paramsForRPC[3] = "Primary";

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

                object[] instantiateData = new object[1];

                instantiateData[0] = PhotonNetwork.player.ID;

                GameObject tmpProjectile = null;

                tmpProjectile = PhotonNetwork.Instantiate(projectile.name, transform.FindChild("ProjectileStartingPoint").transform.position, projectileRotation, 0, instantiateData) as GameObject;

                animator.SetTrigger("attack");

                lastFired = Time.time;
            }
        }
    }

    [RPC] void PlayDeathAnimation(Vector3 pos)
    {
        //Instantiate(deathParticles, pos, Quaternion.identity);
    }

    [RPC]public void ProjectileHit(int damage, int playerHitId, Vector3 positionOfImpact, int projectileOwnerPlayerId)
    {
        //Instantiate(hitParticles, positionOfImpact, Quaternion.identity);
        //If I am the player who got hit
        if (punView.ownerId == playerHitId)
        {
            this.currentHP -= damage;
            if (punView.isMine) GameController.instance.setHpValues_toPlayerCustomProp(this.currentHP);
            
            //Debug.Log("My HP was reduced");
            if (this.currentHP <= 0)
            {
                this.animator.SetTrigger("death");
                Debug.Log("My ID: " + punView.owner.ID);
                Debug.Log("Killer ID: " + projectileOwnerPlayerId);
                GameController.instance.addKillPoint(projectileOwnerPlayerId);
            }
            else
            {
                this.animator.SetTrigger("hit");
            }
        }
    }

    [RPC] void FireProjectile(Vector3 pos, Quaternion rot, int ownerId, string projectileName)
    {
        GameObject tmpProjectile = null;

        if (projectileName == "Primary")
        {
            tmpProjectile = Instantiate(projectile2, pos, rot) as GameObject;
        }
        else if (projectileName == "Secondary")
        {
            tmpProjectile = Instantiate(projectile, pos, rot) as GameObject;
        }

        animator.SetTrigger("attack");
         
        tmpProjectile.GetComponent<Projectile>().Owner = ownerId;
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
