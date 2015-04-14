using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroInformation : MonoBehaviour 
{
    //This class will only handle populating heroes as objects to save information;
    public static HeroInformation instance = null;

    public List<Hero> heroes;
    public class Hero
    {
   
        public string code; // This code is a 3 char string that goes from H01 to H99.
        public string name;
        public string heroClass; // THe class of the hero, example, The Ninja, Fire Mage
        public int hp; // Health points
        public float movementSpeed;
        public string information;

        public string ability_1_name;
        public int ability_1_cd;
        public int ability_1_dmg;

        public string ability_2_name;
        public int ability_2_cd;
        public int ability_2_dmg;
        public Hero(string _name = "")
        {
            this.name = _name;
        }
    }
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
        // Fire Mage
        //heroes[0].name = "Constantine";
        heroes[0].code = "H01";
        heroes[0].heroClass = "Fire Mage";
        heroes[0].hp = 100;
        heroes[0].movementSpeed = 0;
        heroes[0].information = "Information for the FireMage here";
        heroes[0].ability_1_name = "Fire Arrow";
        heroes[0].ability_1_cd = 2;
        heroes[0].ability_1_dmg = 20;
        heroes[0].ability_2_name = "Fireball";
        heroes[0].ability_2_cd = 10;
        heroes[0].ability_2_dmg = 50;
        //The Archer
        //heroes[1].name = "Rolfo";
        heroes[1].heroClass = "The Ranger";
        heroes[0].code = "H02";
        heroes[1].hp = 120;
        heroes[1].movementSpeed = 0;
        heroes[1].information = "Information for the The Ranger here";
        heroes[1].ability_1_name = "Shoot Arrow";
        heroes[1].ability_1_cd = 2;
        heroes[1].ability_1_dmg = 20;
        heroes[1].ability_2_name = "Self Healing";
        heroes[1].ability_2_cd = 20;
        heroes[1].ability_2_dmg = 0;
        //
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
