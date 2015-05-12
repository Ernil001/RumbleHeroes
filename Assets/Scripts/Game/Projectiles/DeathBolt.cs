using UnityEngine;
using System.Collections;

public class DeathBolt : Projectile
{
    protected override void Start()
    {
        base.Start();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        GameObject collidedObject = col.gameObject;
        Debug.Log(col.name);

        if ((collidedObject.tag == "Player" &&
            collidedObject.GetComponent<PhotonView>().ownerId == this.ownerId)
            || collidedObject.tag == "Projectile")
        {
            //If the projectile hit ourself, don't do anything
            return;
        }
        else
        {
            Debug.Log("We hit something other than ourself");

            if (collidedObject.tag == "Player")
            {
                object[] paramsForRPC = new object[3];
                paramsForRPC[0] = this.damage;
                paramsForRPC[1] = collidedObject.GetComponent<PhotonView>().ownerId;
                paramsForRPC[2] = transform.position;
                //We hit another player

                collidedObject.GetComponent<PhotonView>().RPC("ProjectileHit", PhotonTargets.All, paramsForRPC);
            }

            //This is where we check if we hit a player, and apply damage if needed
            /*PlayerController playerController = collidedObject.GetComponent<PlayerController>();
            playerController.currentHP = playerController.currentHP - this.damage;
            Debug.Log(playerController.currentHP);*/
            Destroy(this.gameObject);
        }
    }
}
