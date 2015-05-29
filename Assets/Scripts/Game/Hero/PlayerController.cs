using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class PlayerController : Entity
{
    // Hero Information Variables
    public string heroCode;
    //public string heroClass;
    // Other
    public int controllingPlayer_photonID;
    public string controllingPlayer_Username;
    Transform HeroUI_ConPlUser;
    private Rigidbody2D playerRigidBody;
    private PhotonView punView;
    private Animator animator;
    private bool isGrounded;
    private float someScale;

    void Start()
    {
        //
        playerRigidBody = GetComponent<Rigidbody2D>();
        punView = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();

        someScale = transform.localScale.x;
        // Sets starting HP values for Hero
        if (punView.isMine)
        {
            GameController.instance.setHpValues_toPlayerCustomProp(this.currentHP, this.maxHP);
            //Sets the default color of the skill is ready

            GameController.instance.UI_GameUI_Bottom_Center_FirstAbilityCD.GetComponent<Image>().color = GameController.instance.bGreen;
            GameController.instance.UI_GameUI_Bottom_Center_SecondAbilityCD.GetComponent<Image>().color = GameController.instance.bGreen;
        }
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
                // Destroy the player completly with GameController.instance.destroyPlayerHero(); after the animation ends so timeout ?
                StartCoroutine(FinishDeath());
                //GameController.instance.destroyPlayerHero();
            }
            InputMovement();
        }
        //
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
        //Ability
        if ((Input.GetKeyDown(KeyCode.Mouse0)) && ability_ready)
        {
            //
            Vector3 playerPos = Camera.main.WorldToScreenPoint(transform.FindChild("ProjectileStartingPoint").transform.position);
            //
            object[] instantiateData = new object[4];                
            instantiateData[0] = PhotonNetwork.player.ID;                
            instantiateData[1] = playerPos;            
            instantiateData[2] = Input.mousePosition;            
            instantiateData[3] = punView.viewID;            
            //            
            GameObject tmpProjectile = null;            
            tmpProjectile = PhotonNetwork.Instantiate(Ability.name, transform.FindChild("ProjectileStartingPoint").transform.position, Quaternion.identity, 0, instantiateData) 
                as GameObject;                
            // Original idea was to move the attack animations to ability, but ill keep it here for now.   
            object[] data = new object[1];
            data[0] = this.ability_animator;
            this.punView.RPC("defaultAttackAnimation", PhotonTargets.All, data);        
            //            
            StartCoroutine(cd_ability());
        }
        //Ability2
        if ((Input.GetKeyDown(KeyCode.Mouse1)) && ability2_ready)
        {
            //
            Vector3 playerPos = Camera.main.WorldToScreenPoint(transform.FindChild("ProjectileStartingPoint").transform.position);
            //
            object[] instantiateData = new object[4];
            instantiateData[0] = PhotonNetwork.player.ID;
            instantiateData[1] = playerPos;
            instantiateData[2] = Input.mousePosition;
            instantiateData[3] = punView.viewID;
            //            
            GameObject tmpProjectile = null;
            tmpProjectile = PhotonNetwork.Instantiate(Ability2.name, transform.FindChild("ProjectileStartingPoint").transform.position, Quaternion.identity, 0, instantiateData)
                as GameObject;
            //     
            object[] data = new object[1];
            data[0] = this.ability2_animator;
            this.punView.RPC("defaultAttackAnimation", PhotonTargets.All, data);
            //            
            StartCoroutine(cd_ability2());
        }
    }
    [RPC] void defaultAttackAnimation(string AbilityAnimator = "")
    {
        if(AbilityAnimator != "")
            this.animator.SetTrigger(AbilityAnimator);
    }
    // CoolDowns and their "Animation" :P.
    IEnumerator cd_ability()
    {
        ability_ready = false;
        //ability_lockRelease = Ability.gameObject.transform.GetComponent<Ability>().cd + Time.time;
        int timeToWait = Convert.ToInt32(Ability.gameObject.transform.GetComponent<Ability>().cd);
        Transform tmp_count = GameController.instance.UI_GameUI_Bottom_Center_FirstAbilityCD.transform.FindChild("Count");
        GameController.instance.UI_GameUI_Bottom_Center_FirstAbilityCD.GetComponent<Image>().color = GameController.instance.bRed;
        //
        int x = 0;
        while (x <= timeToWait)
        {
            if (x == timeToWait)
            {
                tmp_count.GetComponent<Text>().text = "";
                ability_ready = true;
                GameController.instance.UI_GameUI_Bottom_Center_FirstAbilityCD.GetComponent<Image>().color = GameController.instance.bGreen;
            }
            else tmp_count.GetComponent<Text>().text = (timeToWait - x).ToString();
            x++;
            yield return new WaitForSeconds(1f);
        }
    }
    IEnumerator cd_ability2()
    {
        ability2_ready = false;
        int timeToWait = Convert.ToInt32(Ability2.gameObject.transform.GetComponent<Ability>().cd);
        Transform tmp_count = GameController.instance.UI_GameUI_Bottom_Center_SecondAbilityCD.transform.FindChild("Count");
        GameController.instance.UI_GameUI_Bottom_Center_SecondAbilityCD.GetComponent<Image>().color = GameController.instance.bRed;
        //
        int x = 0;
        while (x <= timeToWait)
        {
            if (x == timeToWait)
            {
                tmp_count.GetComponent<Text>().text = "";
                ability2_ready = true;
                GameController.instance.UI_GameUI_Bottom_Center_SecondAbilityCD.GetComponent<Image>().color = GameController.instance.bGreen;
            }
            else tmp_count.GetComponent<Text>().text = (timeToWait - x).ToString();
            x++;
            yield return new WaitForSeconds(1f);
        }
    }
    
    //
    [RPC] void PlayDeathAnimation(Vector3 pos)
    {
        //Instantiate(deathParticles, pos, Quaternion.identity);
    }
    /*positionOfImpact is not needed right now, might be removed later*/
    [RPC]public void ProjectileHit(int damage, int playerHitId, Vector3 positionOfImpact, int projectileOwnerPlayerId)
    {
        //Instantiate(hitParticles, positionOfImpact, Quaternion.identity);
        //If I am the player who got hit
        if (punView.ownerId == playerHitId)
        {
            this.currentHP -= damage;
            // Local hero specific
            if (punView.isMine)
            {
                GameController.instance.setHpValues_toPlayerCustomProp(this.currentHP);
                if (this.currentHP <= 0) GameController.instance.addDeathPoint();
            }
            // All
            if (this.currentHP <= 0)
            {
                this.animator.SetTrigger("death");
                /*
                Debug.Log("My ID: " + punView.owner.ID);
                Debug.Log("Killer ID: " + projectileOwnerPlayerId);
                */
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
            tmpProjectile = Instantiate(Ability2, pos, rot) as GameObject;
        }
        else if (projectileName == "Secondary")
        {
            tmpProjectile = Instantiate(Ability, pos, rot) as GameObject;
        }

        animator.SetTrigger("attack");
         
        tmpProjectile.GetComponent<Projectile>().Owner = ownerId;
    }
    //
    //
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            isGrounded = true;
            //animator.SetBool("Jumping", false);
        }
    }
}
