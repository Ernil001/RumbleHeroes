using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour 
{
    public static AudioController instance = null;
	//
    void Awake()
    {
        // Makes the current instance a static one.
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        //
        DontDestroyOnLoad(gameObject);
    }
	void Start () 
    {
	
	}
	void Update ()
    {
	}
    //

}