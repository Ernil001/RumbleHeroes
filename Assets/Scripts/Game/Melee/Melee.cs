using UnityEngine;
using System.Collections;

public class Melee : Ability 
{
    //-***********
    //  Parent script for all Melee abilities
    //-***********
    public int damage;
    //
    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        base.Update();
        //
        if (ownerGameObject != null )
        {
            if (ownerGameObject.transform.FindChild("ProjectileStartingPoint").gameObject != null)
                this.GetComponent<Transform>().position = ownerGameObject.transform.FindChild("ProjectileStartingPoint").GetComponent<Transform>().position;
            else
            {
                Debug.Log("ProjectileStartingPoint GameObject does not exist in the hero resource !");
                forceDestroy();
            }
        }
        else forceDestroy();
        
    }
    // Melee Contact
    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        // Only check your own swings
        if (PhotonNetwork.player.ID == this.Owner)
        {
            GameObject collidedObject = col.gameObject;


            if ((collidedObject.tag == tag_Player && collidedObject.GetComponent<PhotonView>().owner.ID != this.Owner))
            {
                //
                Debug.Log("You have hit: " + collidedObject.name);
                //
                object[] paramsForRPC = new object[4];
                paramsForRPC[0] = this.damage;
                paramsForRPC[1] = collidedObject.GetComponent<PhotonView>().ownerId;
                paramsForRPC[2] = transform.position;
                paramsForRPC[3] = this.Owner;
                collidedObject.GetComponent<PhotonView>().RPC("AbilityHit", PhotonTargets.All, paramsForRPC);

                this.GetComponent<PhotonView>().RPC("RemoveItself", PhotonTargets.All, null);
            }
        }
    }
    [RPC] private void RemoveItself()
    {
        this.forceDestroy();
    }
}
