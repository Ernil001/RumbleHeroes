using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroInformation : MonoBehaviour 
{
    //This class will only handle populating heroes as objects to save information;
    public static HeroInformation instance = null;
    //
    public GameObject[] heroSelectionPrefabs_heroes;
    //
    public List<HeroClass> heroes;

    void Awake()
    {
        // Makes the current instance a static one.
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        // Prepares the objects as references
        // Create an object here and add the needed information in the hero.cs script.
        heroes = new List<HeroClass>();
        heroes.Add(new HeroClass(HeroEnum.Constantine));
        heroes.Add(new HeroClass(HeroEnum.Rolfo));
        heroes.Add(new HeroClass(HeroEnum.Kreml));
        heroes.Add(new HeroClass(HeroEnum.Nejito));
        
    }
    void Start()
    {
      
    }
    // DUnno how to do this yet.
    public string return_HeroName_OnCode(string heroCode)
    {
        string heroName = "";
        for (int x = 0; x < HeroInformation.instance.heroes.Count; x++)
        {
            if (heroCode == HeroInformation.instance.heroes[x].Code)
                heroName = HeroInformation.instance.heroes[x].Name;

        }
        if (heroName == "") GameController.instance.errorDisplay_open("Hero with the code: " + heroCode + ", does not exist !");
        return heroName;
    }
    public string return_HeroCode_OnName(string heroName)
    {
        string heroCode = "";
        for (int x = 0; x < HeroInformation.instance.heroes.Count; x++)
        {
            if (heroName == HeroInformation.instance.heroes[x].Name)
                heroCode = HeroInformation.instance.heroes[x].Code;

        }
        if (heroCode == "") GameController.instance.errorDisplay_open("Hero with the name: " + heroName + ", does not exist !");
        return heroCode;
    }
}
