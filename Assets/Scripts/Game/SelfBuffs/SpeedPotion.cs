using UnityEngine;
using System.Collections;
using System;

public class SpeedPotion : SelfBuff 
{
	protected override void Start () 
    {
        if (ownerGameObject.photonView.isMine)
        {
            ownerGameObject.GetComponent<PlayerController>().speed = Convert.ToSingle(power);
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
        ownerGameObject.GetComponent<PlayerController>().speed = ownerGameObject.GetComponent<PlayerController>().speedBase;
    }

}
