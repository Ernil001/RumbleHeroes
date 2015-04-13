using UnityEngine;
using System.Collections;

public class HeroInformation : MonoBehaviour 
{
    public static HeroInformation instance = null;
    public class Heroes
    {
        public int test = 3;
    }
    
    
    
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
 
    }
}
