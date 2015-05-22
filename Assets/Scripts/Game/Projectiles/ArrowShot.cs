using UnityEngine;
using System.Collections;

public class ArrowShot : Projectile
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
        float force = 2f;

        if (startLocation.x > this.transform.position.x)
        {
            projectileBody.AddForceAtPosition(projectileBody.velocity * -.1f, transform.TransformPoint(0f, force, 0f));
            //Debug.Log("Levo");
        }
        else if (startLocation.x < this.transform.position.x)
        {
            if ((this.transform.localEulerAngles.z < 90f && this.transform.localEulerAngles.z >= 0f) || (this.transform.localEulerAngles.z > 270f && this.transform.localEulerAngles.z <= 360f))
            {
                projectileBody.AddForceAtPosition(projectileBody.velocity * -.1f, transform.TransformPoint(0f, -force, 0f));
                //Debug.Log("Desno, YES z: " + this.transform.localEulerAngles.z.ToString());
            }
            else
            {
                //Debug.Log("Desno, NO z: " + this.transform.localEulerAngles.z.ToString());
            }
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

