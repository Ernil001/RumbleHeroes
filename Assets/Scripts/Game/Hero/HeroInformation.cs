using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroInformation : MonoBehaviour 
{
    //***********//
    //
    // REWORK in PROGRESS DO NOT USE OLD CODE FOR GRABBING INFORMATION ANYMORE (REWORK FINISHED)
    // Everything about heroes is now saved inside the allHeroes array of GameObjects (Compiled from actual resource entities)
    //
    //***********//

    public static HeroInformation instance = null;
    /// <summary>
    /// All saved hero resource entities that have a PlayerController Script.
    /// </summary>
    public GameObject[] allHeroes;
    /// <summary>
    /// Static value of how many player Heros exist // MANUAL
    /// </summary>
    public int heroAmount;
    //
    void Awake()
    {

        // Makes the current instance a static one.
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);    
    }
    void Start()
    {
        // Displaying Console logs if any of the information is either not set or forgotten. Help prevent forest Fires :D!
        if (heroAmount != allHeroes.Length)
            Debug.Log("Hero Amount is not equal to allHeroes Length !\nBe sure to check whether all heroResources are set in HeroInformation.cs \nProblems could happen with updating hero resources and not setting them again in the GameObject allHeroes array.");
        foreach (GameObject chHe in allHeroes)
        {
            if (chHe == null)
                Debug.Log("A hero Resource is not set in HeroInformation.instance.allHeroes[]");
        }
        //
    }
    // Utility Tools Probably reworked, Fine for now.
    public string return_HeroName_OnCode(string heroCode)
    {
        string heroName = "";
        foreach (GameObject hero in HeroInformation.instance.allHeroes)
        {
            if (heroCode == hero.GetComponent<PlayerController>().heroCode)
                heroName = hero.GetComponent<PlayerController>().entityName;

        }

        if (heroName == "") GameController.instance.errorDisplay_open("Hero with the code: " + heroCode + ", does not exist !");
        return heroName;
    }
    public string return_HeroCode_OnName(string heroName)
    {
        string heroCode = "";
        foreach (GameObject hero in HeroInformation.instance.allHeroes)
        {
            if (heroName == hero.GetComponent<PlayerController>().entityName)
                heroCode = hero.GetComponent<PlayerController>().heroCode;

        }
        if (heroCode == "") GameController.instance.errorDisplay_open("Hero with the name: " + heroName + ", does not exist !");
        return heroCode;
    }
}
