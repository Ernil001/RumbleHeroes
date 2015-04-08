using UnityEngine;
using System.Collections;

public class FireBolt : Projectile {

    private GameObject owner;

    protected override void Start()
    {
        base.Start();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        GameObject collidedObject = col.gameObject;

        if (collidedObject.name == "Player-" + this.name || collidedObject.tag == "Projectile")
        {
            //If the projectile hit ourself, don't do anything
            return;
        }  
        else
        {
            Debug.Log("We hit something other than ourself");

            //This is where we check if we hit a player, and apply damage if needed
            PlayerController playerController = collidedObject.GetComponent<PlayerController>();
            playerController.currentHP = playerController.currentHP - this.damage;
            Debug.Log(playerController.currentHP);

            Destroy(gameObject);
        }
    }
}
