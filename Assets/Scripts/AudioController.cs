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
    // Remaking the default AudioSource.PlayClipAtPoint function // Custom Parameters dont work yet
    /// <summary>
    /// Reworked original AudioSource.PlayClipAtPoint (Able to change AudioSource Attributes)
    /// </summary>
    private AudioSource playClipAtPoint(AudioClip file, Vector3 pos, string objName = "Default:Generated", object[] customPar = null)
    {
        GameObject obj = new GameObject(objName); // create the temp object
        obj.transform.position = pos; // set its position
        AudioSource aSource = obj.AddComponent<AudioSource>();
        aSource.clip = file;
        //
        aSource.rolloffMode = AudioRolloffMode.Linear;
        // Set Custom properties - Might rework this, that we could invoke different special properties
        if (customPar != null)
        {

        }
        else 
        {
            // Sets Default
            aSource.spatialBlend = 1f;
            aSource.maxDistance = 20f;
            aSource.minDistance = 10f;
            aSource.spread = 0f;
            aSource.dopplerLevel = 1f;
        }
        //
        aSource.Play();
        Destroy(obj, file.length); 
        return aSource; 

    }
    //
    public void playClip_ability(AudioClip file, Vector3 pos, string objName = "Default:AbilitySound", object[] customPar = null)
    {
        //AudioSource.PlayClipAtPoint(file, pos);
        playClipAtPoint(file, pos, objName, customPar);
    }

}