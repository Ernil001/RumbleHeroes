using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroInformation : MonoBehaviour 
{
    //This class will only handle populating heroes as objects to save information;
    public static HeroInformation instance = null;

    public List<HeroClass> heroes;

    void Awake()
    {
        // Makes the current instance a static one.
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        // Prepares the objects as references
        heroes = new List<HeroClass>();
        heroes.Add(new HeroClass(HeroEnum.Constantine));
        heroes.Add(new HeroClass(HeroEnum.Rolfo));
        heroes.Add(new HeroClass(HeroEnum.Kreml));
        heroes.Add(new HeroClass(HeroEnum.Nejito));

        
        foreach (HeroClass tmpHero in heroes)
        {
            Debug.Log(tmpHero.ToString());
        }
        
        
    }
    void Start()
    {
      
    }
}
