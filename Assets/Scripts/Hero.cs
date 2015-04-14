using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hero : MonoBehaviour {

    private string code; // This code is a 3 char string that goes from H01 to H99.
    private string name;
    private string heroClass; // THe class of the hero, example, The Ninja, Fire Mage
    private int hp; // Health points
    private float movementSpeed;
    private string information;

    private List<Ability> abilities;

    public Hero(string _name = "")
    {
        this.name = _name;

        switch (this.name)
        {
            case "Constantine":
                this.code = "H01";
                this.heroClass = "Fire Mage";
                this.hp = 100;
                this.movementSpeed = 0;
                this.information = "Information for the FireMage here";

                abilities.Add(new Ability("Fire Arrow", 2, 20));
                abilities.Add(new Ability("Fireball", 10, 50));
                break;
            case "Rolfo":
                this.heroClass = "The Ranger";
                this.code = "H02";
                this.hp = 120;
                this.movementSpeed = 0;
                this.information = "Information for the The Ranger here";

                abilities.Add(new Ability("Shoot Arrow", 2, 20));
                abilities.Add(new Ability("Self Healing", 20, 0));
                break;
            case "Kreml":
                this.heroClass = "The Ranger";
                this.code = "H02";
                this.hp = 120;
                this.movementSpeed = 0;
                this.information = "Information for the The Ranger here";

                abilities.Add(new Ability("Shoot Arrow", 2, 20));
                abilities.Add(new Ability("Self Healing", 20, 0));
                break;
            case "Nejito":
                this.heroClass = "The Ranger";
                this.code = "H02";
                this.hp = 120;
                this.movementSpeed = 0;
                this.information = "Information for the The Ranger here";

                abilities.Add(new Ability("Shoot Arrow", 2, 20));
                abilities.Add(new Ability("Self Healing", 20, 0));
                break;
            default:
                break;
        }
    }
}
