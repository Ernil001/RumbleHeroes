using UnityEngine;
using System.Collections;

public class FireBall : Projectile
{
    private Vector3 startLocation;

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

}

