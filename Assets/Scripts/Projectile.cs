using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Transform))]
public abstract class Projectile : MonoBehaviour {

    public int speed = 10;
    public int maxDistance = 100;
    public int damage = 100;

    public Rigidbody2D projectileBody;
    public Transform transform;

    private Vector3 basePosition;

	protected virtual void Start () {
        projectileBody = GetComponent<Rigidbody2D>();
        transform = GetComponent<Transform>();

        //Get projectile base position
        basePosition = transform.position;

        
        Debug.Log(basePosition);
	}
	
	protected virtual void Update () {
        projectileBody.MovePosition(projectileBody.position + new Vector2(1, 0)  * speed *
            Time.deltaTime);

        //Check if max distance has been reached

        float distance = Vector3.Distance(basePosition, transform.position);

        if(distance > maxDistance)
        {
            //Max distance reached, delete gameobject
            Destroy(gameObject);
        }
	}
}
