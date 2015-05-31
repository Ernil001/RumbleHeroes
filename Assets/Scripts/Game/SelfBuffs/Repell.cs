using UnityEngine;
using System.Collections;
using System;

public class Repell : SelfBuff
{
    protected override void Start()
    {
        if (ownerGameObject.photonView.isMine)
        {
            Physics2D.IgnoreCollision(ownerGameObject.GetComponent<BoxCollider2D>(), this.GetComponent<CircleCollider2D>());
        }
    }
    /*
	protected override void Update () 
    {
        base.Update();
	}
    */
    public void OnDestroy()
    {
        //ownerGameObject.GetComponent<PlayerController>().speed = ownerGameObject.GetComponent<PlayerController>().speedBase;
    }

}
