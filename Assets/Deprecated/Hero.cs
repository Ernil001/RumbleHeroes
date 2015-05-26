using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroClass {
    // OLD CODE // This will be pointless, Player Controller will Control most of the ENTITY
    private string code; // This code is a 3 char string that goes from H01 to H99.
    private HeroEnum hero;
    private string sClass; // THe class of the hero, example, The Ninja, Fire Mage
    private int hp; // Health points
    private float movementSpeed;
    private string information;
    private string name;

    private List<Ability> abilities;

    public string Information
    {
        get
        {
            return this.information;
        }
    }
    public string Class
    {
        get
        {
            return this.sClass;
        }
    }
    public string Name
    {
        get
        {
            return this.name;
        }
    }
    public string Code
    {
        get
        {
            return this.code;
        }
    }

    public HeroClass(HeroEnum hero)
    {
        this.hero = hero;
        this.abilities = new List<Ability>();

        switch (this.hero)
        {
            case HeroEnum.Constantine:
                this.name = "Constantine";
                this.sClass = "Fire Mage";
                this.code = "H01";
                this.hp = 100;
                this.movementSpeed = 0;
                this.information = "Information for the FireMage here";
                /*
                abilities.Add(new Ability("Fire Arrow", 2, 20));
                abilities.Add(new Ability("Fireball", 10, 50));
                 * */
                break;

            case HeroEnum.Rolfo:
                this.name = "Rolfo";
                this.sClass = "The Ranger";
                this.code = "H02";
                this.hp = 120;
                this.movementSpeed = 0;
                this.information = "Information for The Ranger here";
                /*
                abilities.Add(new Ability("Shoot Arrow", 2, 20));
                abilities.Add(new Ability("Self Healing", 20, 0));*/
                break;

            case HeroEnum.Kreml:
                this.name = "Kreml";
                this.sClass = "The Black Knight";
                this.code = "H03";
                this.hp = 120;
                this.movementSpeed = 0;
                this.information = "Information for The Black Knight here";
                /*
                abilities.Add(new Ability("Shoot Arrow", 2, 20));
                abilities.Add(new Ability("Self Healing", 20, 0));*/
                break;

            case HeroEnum.Nejito:
                this.name = "Nejito";
                this.sClass = "The Ninja";
                this.code = "H04";
                this.hp = 120;
                this.movementSpeed = 0;
                this.information = "Information for The Ninja here";
                /*
                abilities.Add(new Ability("Shoot Arrow", 2, 20));
                abilities.Add(new Ability("Self Healing", 20, 0));*/
                break;

            default:
                break;
        }
    }

    public override string ToString()
    {
        switch(this.hero)
        {
            case HeroEnum.Constantine:
                return "Constantine";

            case HeroEnum.Rolfo:
                return "Rolfo";

            case HeroEnum.Kreml:
                return "Kreml";

            case HeroEnum.Nejito:
                return "Nejito";

            default:
                return "";
        }
    }
    // OLD CODE //
}
