using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroInformation : MonoBehaviour 
{
    public static HeroInformation instance = null;

    //public List<Hero> heroes;
    public Hero[] heroList;
    public Hero FireMage;
    public class Hero
    {
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
        //
        /*
        DontDestroyOnLoad(gameObject);
        FireMage = new Hero("Constantine");
        heroList[0] = FireMage;

        //heroList[1] = new Hero("Rolfo");

        Debug.Log(heroList[0].name);
        Debug.Log(heroList[1].name);
         * */

    }
    void Start()
    {
        //This class will only handle populating heroes as objects to save information;
        // Fire Mage
        //heroes = new List<Hero>();
        /*
        heroes.Add(new Hero("Constantine"));

        heroes.Add(new Hero("Rolfo"));
        */
        //FireMage = new Hero("Constantine");
        //FireMage.name = "Constantine";
        /*
        Hero[0] heroList = new Hero();

        heroList[0].name = "Constantine";
        heroList[0].heroClass = "Fire Mage";
        heroList[0].hp = 100;
        heroList[0].movementSpeed = 0;
        heroList[0].information = "Information for the FireMage here";
        heroList[0].ability_1_name = "Fire Arrow";
        heroList[0].ability_1_cd = 2;
        heroList[0].ability_1_dmg = 20;
        heroList[0].ability_2_name = "Fireball";
        heroList[0].ability_2_cd = 10;
        heroList[0].ability_2_dmg = 50;
        //The Archer
        /*
        heroList[1].name = "Rolfo";
        heroList[1].heroClass = "The Ranger";
        heroList[1].hp = 120;
        heroList[1].movementSpeed = 0;
        heroList[1].information = "Information for the The Ranger here";
        heroList[1].ability_1_name = "Shoot Arrow";
        heroList[1].ability_1_cd = 2;
        heroList[1].ability_1_dmg = 20;
        heroList[1].ability_2_name = "Self Healing";
        heroList[1].ability_2_cd = 20;
        heroList[1].ability_2_dmg = 0;
     * */
        //

    }
}
