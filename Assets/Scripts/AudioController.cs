using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour 
{
    //*************
    //
    //  This controller will have the needed Methods and GameObjects which will be required for the Audio
    //
    //*************
    public static AudioController instance = null;
	//
    public AudioClip UI_ButtonHover;
    public AudioClip UI_ButtonClick;
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
	    // On start collect data from options on sound volume and set it apropriatelly for each subdivision of sound
        OptionController.instance.MasterVolume = OptionController.instance.MasterVolume; //Looks weird but the set encapsulation sets the sound aswell, so it works :X.


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
    // Different version for starting the same Method, playClipAtPoint;
    public void playClip_ability(AudioClip file, Vector3 pos, string objName = "Ability:", object[] customPar = null)
    {
        //AudioSource.PlayClipAtPoint(file, pos);
        playClipAtPoint(file, pos, objName, customPar);
    }
    public void playClip_default(AudioClip file, Vector3 pos, string objName = "Default:", object[] customPar = null)
    {
        playClipAtPoint(file, pos, objName, customPar);
    }
    // Main method for playing sound for UI; I might change this a bit, dont like it.
    public void playClip_UI_ButtonClick()
    {
        AudioSource.PlayClipAtPoint(UI_ButtonClick, new Vector3(0, 0, 0));
    }
    public void playClip_UI_ButtonHover()
    {
        AudioSource.PlayClipAtPoint(UI_ButtonHover, new Vector3(0, 0, 0));
    }


}