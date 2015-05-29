using UnityEngine;
using System.Collections;

public class FireBall : Projectile
{
    private Vector3 startLocation;
    private bool hasHit = false;
    public float aoeX;
    public float aoeY;

    protected override void Start()
    {
        //
        projectileBody = GetComponent<Rigidbody2D>();
        Vector3 playerPos = (Vector3)this.GetComponent<PhotonView>().instantiationData[1];
        Vector3 mousePos = (Vector3)this.GetComponent<PhotonView>().instantiationData[2];
        Vector3 dir = (mousePos - playerPos).normalized;
        // Get angle related from mouse to startingProjectileLocation and ADD it
        float angle = Mathf.Atan2((mousePos.y - playerPos.y), mousePos.x - playerPos.x) * Mathf.Rad2Deg;
        Quaternion projectileRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        //
        this.GetComponent<Transform>().rotation = projectileRotation;
        // Add Force in angle direction
        projectileBody.AddForce(dir * speed * 100);
        // Save start location so we can calculate the change of the trajectory depending on the start location
        startLocation = this.transform.position;
        //
        this.Owner = (int)this.GetComponent<PhotonView>().instantiationData[0];
        timeInstantiated = Time.time;
    }
    protected override void Update()
    {
        //Check if it has surpassed the time to live
        if (Time.time - timeInstantiated >= secondsToLive)
        {
            Destroy(gameObject);
        }
        // Two options, depending on which way the shot is traveling.
        if (startLocation.x > this.transform.position.x)
        {
            this.transform.Rotate(new Vector3(0, 0, 360.0f + Vector3.Angle(this.transform.right, this.GetComponent<Rigidbody2D>().velocity.normalized)));
            //left
        }
        else if (startLocation.x < this.transform.position.x)
        {
            this.transform.Rotate(new Vector3(0, 0, 360.0f - Vector3.Angle(this.transform.right, this.GetComponent<Rigidbody2D>().velocity.normalized)));
            //right
        }
        //Check if max distance has been reached
        float distance = Vector3.Distance(basePosition, transform.position);
        if (distance > maxDistance)
        {
            //Max distance reached, delete gameobject
            Destroy(gameObject);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        //Only collision check your own projectiles
        if (PhotonNetwork.player.ID == this.Owner)
        {
            GameObject collidedObject = col.gameObject;

            if ((collidedObject.tag == tag_Player && collidedObject.GetComponent<PhotonView>().owner.ID != this.Owner) || col.tag == "Ground")
            {

                // Projectile has made proper collision for explosion, detect possible objects to damage around this area, and apply damage.
                this.GetComponent<PhotonView>().RPC("RemoveProjectileFromGame", PhotonTargets.All, null);
                // Check aoe for manually detected collisions and apply damage to those objects.
                Vector3 aPos = this.GetComponent<Transform>().position;
                foreach (GameObject activeHero in GameController.instance.activePlayerHeroes)
                {
                    Vector3 hPos = activeHero.GetComponent<Transform>().position;
                    if(
                        aPos.x+aoeX >= hPos.x 
                        &&
                        aPos.x-aoeX <= hPos.x
                        &&
                        aPos.y+aoeY >= hPos.y
                        &&
                        aPos.y-aoeY <= hPos.y
                       )
                    {
                        //Debug.Log(activeHero.name + " Has Been Hit");

                        object[] paramsForRPC = new object[4];
                        paramsForRPC[0] = this.damage;
                        paramsForRPC[1] = activeHero.gameObject.GetComponent<PhotonView>().ownerId;
                        paramsForRPC[2] = transform.position;
                        paramsForRPC[3] = this.Owner;

                        activeHero.gameObject.GetComponent<PhotonView>().RPC("ProjectileHit", PhotonTargets.All, paramsForRPC);
                    }
                }


                
            }
            /*
            else if (col.tag == "Ground")
            {
                //We hit ground, remove projectile
                this.GetComponent<PhotonView>().RPC("RemoveProjectileFromGame", PhotonTargets.All, null);
            }
                 * */
        }
    }

}

