using UnityEngine;
using System.Collections;

public class RadiusOverTime : MonoBehaviour {

    public float startRadius;
    public float endRadius;
    public float overTime;

    private float timeStarted;
    private CircleCollider2D collider;

	// Use this for initialization
	void Start () {
        this.timeStarted = Time.time;
        collider = this.GetComponent<CircleCollider2D>();
        collider.radius = startRadius;
    }
	
	// Update is called once per frame
	void Update () {
        if(Time.time - this.timeStarted < this.overTime)
        {
            collider.radius = (Time.time - this.timeStarted) * endRadius;
        }
	}
}
