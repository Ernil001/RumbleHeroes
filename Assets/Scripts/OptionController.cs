using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OptionController : MonoBehaviour 
{
    //*************
    // This controller will focus on displaying and saving options.
    // Attributes here will be public so any method can have access to them when they are needed, for instance, Sound Volumes.
    //*************

    public static OptionController instance = null;
    // GameObjects that will interact with options
    public GameObject usernameObj;
    public GameObject masterVolumeObj;
    
    // Saved Options Value if they need to be // Default values for these will later derive from INI.file

    public float username;
    
    private float masterVolume = 1f;
    /// <summary>
    /// Set/Get this value controlls AudioListener.volume 
    /// </summary>
    public float MasterVolume
    {
        set 
        {
            masterVolume = value;
            AudioListener.volume = masterVolume;
        }
        get 
        {
            return masterVolume;
        }
        
    }

    //
    void Awake()
    {
        // Makes the current instance a static one.
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        //
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        loadOptions();
    }
    // Load options directly from INI.file
    public void loadOptions()
    {
        
    }
    // Loading options for display purpose. They will be displayed in the extraOptions -> OptionsWrap window
    public void displayOptions()
    {
        // Before displaying Load all Options
        this.loadOptions();
        // Load Master Volume Sound
        masterVolumeObj.GetComponent<Slider>().value = MasterVolume;
        //
    }
    // Main method for saving all options // lazy way out :D
    public void saveOptions()
    {
        // Saving Master Volume sound
        if (MasterVolume != masterVolumeObj.GetComponent<Slider>().value)
        {
            MasterVolume = masterVolumeObj.GetComponent<Slider>().value;
        }

        // Save the new options into the ini file
        this.saveInFile();
        // After all options have been set in OptionController and Into the Desired INI.file Close the ExtraOptions window
        GameController.instance.extraRoom_closeOptions();
    }
    //
    private void saveInFile()
    {
 
    }
}
