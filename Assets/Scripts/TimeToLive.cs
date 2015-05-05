using UnityEngine;
using System.Collections;

public class TimeToLive : MonoBehaviour {

    public int timeToLive = 2;

    private float timeInitialised = 0f;

	// Use this for initialization
	void Start () {
        timeInitialised = Time.timeSinceLevelLoad;
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.timeSinceLevelLoad - timeInitialised > timeToLive)
            Destroy(gameObject);
	}
}
