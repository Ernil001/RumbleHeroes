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
    Transform HeroUI;
    Transform HeroUI_ConPlUser;
    private Rigidbody2D playerRigidBody;
    private PhotonView punView;
    private Animator animator;
    private bool isGrounded;
    private float someScale;
    /// <summary>
    /// Returns true if the InputKeys are set on Game
    /// </summary>
    private bool IsGameInput
    {
        get 
        {
            if (InputKeys.instance.InputType == "Game") return true;
            else return false;
        }
    }
    void Awake()
    {
        //
        playerRigidBody = GetComponent<Rigidbody2D>();
        punView = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        //
    }

    void Start()
    {
        //
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
        this.HeroUI = this.transform.FindChild("HeroUI");
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
        if (punView.isMine && IsGameInput && this.isEntityAlive)
        {
            InputMovement();
        }
        // Might do an extra check for if player is dead.

        //
        /*
        Vector3 horizontalMove = playerRigidBody.velocity;
        // Don't use the vertical velocity
        horizontalMove.y = 0;
        // Calculate the approximate distance that will be traversed
        float distance = horizontalMove.magnitue * Time.fixedDeltaTime;
        // Normalize horizontalMove since it should be used to indicate direction
        horizontalMove.Normalize();
        RaycastHit hit;

        // Check if the body's current velocity will result in a collision
        if (playerRigidBody.SweepTest(horizontalMove, out hit, distance))
        {
            // If so, stop the movement
            playerRigidBody.velocity = new Vector3(0, playerRigidBody.velocity.y, 0);
        }
        */
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
            animator.SetBool("Running", isGrounded);
            transform.localScale = new Vector2(-someScale, transform.localScale.y);
        }
        else if(playerRigidBody.velocity.x < 0)
        {
            animator.SetBool("Running", isGrounded);
            transform.localScale = new Vector2(someScale, transform.localScale.y);
        }
        else
        {
            animator.SetBool("Running", false);
        }
        //
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            playerRigidBody.AddForce(new Vector2(0, entityJumpPower), ForceMode2D.Impulse);
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
            PhotonNetwork.Instantiate("Abilities/" + Ability.name, transform.FindChild("ProjectileStartingPoint").transform.position, Quaternion.identity, 0, instantiateData);
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
            PhotonNetwork.Instantiate("Abilities/" + Ability2.name, transform.FindChild("ProjectileStartingPoint").transform.position, Quaternion.identity, 0, instantiateData);
            //     
            object[] data = new object[1];
            data[0] = this.ability2_animator;
            this.punView.RPC("defaultAttackAnimation", PhotonTargets.All, data);
            //            
            StartCoroutine(cd_ability2());
        }
    }
    [PunRPC] void defaultAttackAnimation(string AbilityAnimator = "")
    {
        if(AbilityAnimator != "")
            this.animator.SetTrigger(AbilityAnimator);
    }
    // CoolDowns and their "Animation" :P.
    IEnumerator cd_ability()
    {
        ability_ready = false;
        //ability_lockRelease = Ability.gameObject.transform.GetComponent<Ability>().cd + Time.time;
        float timeToWait = Ability.gameObject.transform.GetComponent<Ability>().cd;
        Transform tmp_count = GameController.instance.UI_GameUI_Bottom_Center_FirstAbilityCD.transform.FindChild("Count");
        GameController.instance.UI_GameUI_Bottom_Center_FirstAbilityCD.GetComponent<Image>().color = GameController.instance.bRed;
        //
        
        //
        float x = 0;
        while (x <= timeToWait)
        {
            if (x == timeToWait)
            {
                tmp_count.GetComponent<Text>().text = "";
                ability_ready = true;

                GameController.instance.UI_GameUI_Bottom_Center_FirstAbilityCD.GetComponent<Image>().color = GameController.instance.bGreen;
            }
            else if(x%1 == 0)
            {
                tmp_count.GetComponent<Text>().text = (timeToWait - x).ToString();
            }
            x=x+0.5f;
            yield return new WaitForSeconds(0.5f);
        }
    }
    IEnumerator cd_ability2()
    {
        ability2_ready = false;
        float timeToWait = Ability2.gameObject.transform.GetComponent<Ability>().cd;
        Transform tmp_count = GameController.instance.UI_GameUI_Bottom_Center_SecondAbilityCD.transform.FindChild("Count");
        GameController.instance.UI_GameUI_Bottom_Center_SecondAbilityCD.GetComponent<Image>().color = GameController.instance.bRed;
        //
        float x = 0;
        while (x <= timeToWait)
        {
            if (x == timeToWait)
            {
                tmp_count.GetComponent<Text>().text = "";
                ability2_ready = true;
                GameController.instance.UI_GameUI_Bottom_Center_SecondAbilityCD.GetComponent<Image>().color = GameController.instance.bGreen;
            }
            else if (x % 1 == 0)
            {
                tmp_count.GetComponent<Text>().text = (timeToWait - x).ToString();
            }
            x = x + 0.5f;
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    //
    [PunRPC] void PlayDeathAnimation(Vector3 pos)
    {
        //Instantiate(deathParticles, pos, Quaternion.identity);
    }

    /*positionOfImpact is not needed right now, might be removed later*/
    [PunRPC]public void ProjectileHit(int damage, int playerHitId, Vector3 positionOfImpact, int projectileOwnerPlayerId)
    {
        // Projectile only Code IF NEEDED


        //
        AbilityHit(damage, playerHitId, positionOfImpact, projectileOwnerPlayerId);
    }
    [PunRPC] public void AbilityHit(int damage, int playerHitId, Vector3 positionOfImpact, int projectileOwnerPlayerId)
    {
        
        //If I am the player who got hit
        if (punView.ownerId == playerHitId)
        {
            // Reduces HP
            this.currentHP -= damage;
            // Creates animation
            GameObject tempMove = Instantiate(GameController.instance.DamageFloatingText, new Vector3(0,0,0), Quaternion.identity) as GameObject;
            //if (tempMove != null) Debug.Log(tempMove);
            tempMove.GetComponent<Text>().text = damage.ToString();
            tempMove.transform.SetParent(HeroUI);
            tempMove.GetComponent<RectTransform>().position = new Vector3(0, 0, 0);
            // Local hero specific

            if (this.isEntityAlive)
            {
                if (punView.isMine)
                {
                    GameController.instance.setHpValues_toPlayerCustomProp(this.currentHP);
                    if (this.currentHP <= 0)
                    {
                        GameController.instance.addDeathPoint();
                        StartCoroutine(FinishDeath());

                    }
                }
                // All
                if (this.currentHP <= 0)
                {
                    this.animator.SetTrigger("death");
                    this.isEntityAlive = false;
                    GameController.instance.addKillPoint(projectileOwnerPlayerId);
                }
                else
                {
                    this.animator.SetTrigger("hit");
                }
            }


        }
    }
    //
    IEnumerator FinishDeath()
    {
        int x = 0;
        while (x < 2)
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
    [PunRPC] void FireProjectile(Vector3 pos, Quaternion rot, int ownerId, string projectileName)
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
        if (col.gameObject.tag == GameController.instance.tag_Ground)
        {
            isGrounded = true;
        }
    }
}
