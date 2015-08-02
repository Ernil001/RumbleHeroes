using UnityEngine;
using System.Collections;

public class WaterMines : Projectile 
{
    // A special case of projectile where after shot it sticks to the ground and waits a certain amount of time until selfdestroy AKA explode.
    public bool selfDamage;
    public float aoeX;
    public float aoeY;
    //
    protected override void Start()
    {
        //
        /*
        mousePos.z = 10;
        mousePos.x = mousePos.x - playerPos.x;
        mousePos.y = mousePos.y - playerPos.y;
        //float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        Quaternion projectileRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        //
        this.GetComponent<Transform>().rotation = projectileRotation;
        */
        //

        projectileBody = GetComponent<Rigidbody2D>();
        // Get projectile base position
        //basePosition = transform.position;
    }
    protected override void Update()
    {
        if ((secondsToLive != 0) && (Time.time - timeInstantiated >= secondsToLive))
        {
            detonate();
        }
        //else Debug.Log("Not time yet");

        //base.Update();
    }
    protected override void OnTriggerEnter2D(Collider2D col)
    {
        if (PhotonNetwork.player.ID == this.Owner)
        {
            GameObject collidedObject = col.gameObject;
            if ((collidedObject.tag == tag_Player && collidedObject.GetComponent<PhotonView>().owner.ID != this.Owner) /*|| col.tag == GameController.instance.tag_Ground*/)
            {
                // Check aoe for manually detected collisions and apply damage to those objects.
                detonate();
            }
        }
        if (col.tag == GameController.instance.tag_Ground || col.tag == GameController.instance.tag_NormalWall)
        {
            this.projectileBody.isKinematic = true;
        }
    }
    private void detonate()
    {
        Vector3 aPos = this.GetComponent<Transform>().position;
        foreach (GameObject activeHero in GameController.instance.activePlayerHeroes)
        {
            Vector3 hPos = activeHero.GetComponent<Transform>().position;
            if (
                aPos.x + aoeX >= hPos.x
                &&
                aPos.x - aoeX <= hPos.x
                &&
                aPos.y + aoeY >= hPos.y
                &&
                aPos.y - aoeY <= hPos.y
               )
            {
                Debug.Log("A hit has happened to resource:" + activeHero.name);
                if (selfDamage)
                {
                    object[] paramsForRPC = new object[4];
                    paramsForRPC[0] = this.damage;
                    paramsForRPC[1] = activeHero.gameObject.GetComponent<PhotonView>().owner.ID;
                    paramsForRPC[2] = transform.position;
                    paramsForRPC[3] = this.Owner;

                    activeHero.gameObject.GetComponent<PhotonView>().RPC("ProjectileHit", PhotonTargets.All, paramsForRPC);
                }
                else if (Owner != activeHero.GetComponent<PhotonView>().owner.ID)
                {
                    object[] paramsForRPC = new object[4];
                    paramsForRPC[0] = this.damage;
                    paramsForRPC[1] = activeHero.gameObject.GetComponent<PhotonView>().owner.ID;
                    paramsForRPC[2] = transform.position;
                    paramsForRPC[3] = this.Owner;

                    activeHero.gameObject.GetComponent<PhotonView>().RPC("ProjectileHit", PhotonTargets.All, paramsForRPC);
                }
            }
        }
        this.GetComponent<PhotonView>().RPC("RemoveProjectileFromGame", PhotonTargets.All, null);
    }
}
