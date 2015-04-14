using UnityEngine;
using System.Collections;

public class Ability : MonoBehaviour {

    private string name;
    private int cooldown;
    private int damage;

    public Ability(string _name, int _cooldown, int _damage)
    {
        this.name =     _name;
        this.cooldown = _cooldown;
        this.damage =   _damage;
    }
}
