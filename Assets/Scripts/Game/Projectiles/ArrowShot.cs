using UnityEngine;
using System.Collections;

public class ArrowShot : Projectile
{
    /*
    private override Vector3 basePosition;
    private override float timeInstantiated;
    public override Rigidbody2D projectileBody;
    */

    protected override void Start()
    {
        //timeInstantiated = Time.time;

        //Set owner
        //this.Owner = (int)this.GetComponent<PhotonView>().instantiationData[0];

        //projectileBody = GetComponent<Rigidbody2D>();

        //Get projectile base position
        //basePosition = transform.position;


        base.Start();
    }
    /*
    protected override void Update()
    {
        //Debug.Log("ASD");





        base.Start();
    }
    */
}

