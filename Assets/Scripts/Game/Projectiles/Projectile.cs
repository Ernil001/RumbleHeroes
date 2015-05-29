using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Transform))]

public abstract class Projectile : Ability 
{
    //
    public GameObject projectileHitAnimation;
    //
    public int speed = 10;
    public int maxDistance = 100;
    public int damage = 100;
    //
    public Rigidbody2D projectileBody;
    protected Vector3 basePosition;
    
    protected override void Awake()
    {
        base.Awake();
    }

	protected override void Start () 
    {
        
        // Moved the projectile type code here
        mousePos.z = 10;
        mousePos.x = mousePos.x - playerPos.x;
        mousePos.y = mousePos.y - playerPos.y;
        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        Quaternion projectileRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        //
        this.GetComponent<Transform>().rotation = projectileRotation;
        //
        projectileBody = GetComponent<Rigidbody2D>();
        //Get projectile base position
        basePosition = transform.position;
        //
	}

    protected override void Update()
    {
        base.Update();
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
        //Only collision check your own projectiles
        if (PhotonNetwork.player.ID == this.Owner)
        {
            GameObject collidedObject = col.gameObject;

            if ((collidedObject.tag == tag_Player &&
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

    [RPC] public void RemoveProjectileFromGame()
    {
        // Create EndOfProjectileStatus, AKA. explosion, random animation, etc etc
        if (projectileHitAnimation != null) Instantiate(projectileHitAnimation, transform.position, Quaternion.identity);
        //
        forceDestroy();
    }
}
