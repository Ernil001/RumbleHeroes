using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Transform))]
public abstract class Projectile : MonoBehaviour {

    public int speed = 10;
    public int maxDistance = 100;
    public int damage = 100;
    public int secondsToLive = 10;

    public Rigidbody2D projectileBody;

    private Vector3 basePosition;
    private float timeInstantiated;
    public int networkOwnerId;

    public int Owner
    {
        set
        {
            this.networkOwnerId = value;
        }
        get
        {
            return this.networkOwnerId;
        }
    }
	protected virtual void Start () {

        timeInstantiated = Time.time;

        projectileBody = GetComponent<Rigidbody2D>();

        //Get projectile base position
        basePosition = transform.position;
        
        Debug.Log(basePosition);
	}
	
	protected virtual void Update () {

        //Check if it has surpassed the time to live
        if(Time.time - timeInstantiated >= secondsToLive)
        {
            Destroy(gameObject);
        }


        // Get angle towards mouse position
        float angle = projectileBody.transform.eulerAngles.magnitude * Mathf.Deg2Rad;

        Vector2 projectileNextPos = new Vector2(Mathf.Cos(angle),
            Mathf.Sin(angle));

        projectileBody.MovePosition(projectileBody.position + 
            projectileNextPos * speed *
            Time.fixedDeltaTime);


        //Check if max distance has been reached

        float distance = Vector3.Distance(basePosition, transform.position);

        if(distance > maxDistance)
        {
            //Max distance reached, delete gameobject
            Destroy(gameObject);
        }
	}

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (PhotonNetwork.player.isMasterClient)
        {
            Debug.Log(col.gameObject.tag);

            GameObject collidedObject = col.gameObject;

            if ((collidedObject.tag == "Player" &&
                collidedObject.GetComponent<PhotonView>().owner.ID != this.Owner))
            {

                object[] paramsForRPC = new object[4];
                paramsForRPC[0] = this.damage;
                paramsForRPC[1] = collidedObject.GetComponent<PhotonView>().ownerId;
                paramsForRPC[2] = transform.position;
                paramsForRPC[3] = this.Owner;
    
                collidedObject.GetComponent<PhotonView>().RPC("ProjectileHit", PhotonTargets.All, paramsForRPC);

                //Remove projectiles from all clients
                this.GetComponent<PhotonView>().RPC("RemoveProjectileFromGame", PhotonTargets.All, null);
            }
            else if(col.tag == "Ground")
            {
                //We hit ground, remove projectile
                this.GetComponent<PhotonView>().RPC("RemoveProjectileFromGame", PhotonTargets.All, null);
            }
        }
    }

    [RPC]
    public void RemoveProjectileFromGame()
    {
        Destroy(gameObject);
    }
}
