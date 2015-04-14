using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroInformation : MonoBehaviour 
{
    //This class will only handle populating heroes as objects to save information;
    public static HeroInformation instance = null;

    public List<Hero> heroes;

    void Awake()
    {
        // Makes the current instance a static one.
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        // Prepares the objects as references
        heroes = new List<Hero>();
        heroes.Add(new Hero("Constantine"));
        heroes.Add(new Hero("Rolfo"));
        heroes.Add(new Hero("Kreml"));
        heroes.Add(new Hero("Nejito"));
        /*
        foreach (Hero key in heroes)
        {
            Debug.Log(key.name);
        }
        Debug.Log(heroes[0].name);
        */
    }
    void Start()
    {
      
    }
}
