using UnityEngine;
using System.Collections;

public class Ability : MonoBehaviour {

    private string sName;
    private int cooldown;
    private int damage;

    public Ability(string _name, int _cooldown, int _damage)
    {
        this.sName =        _name;
        this.cooldown =     _cooldown;
        this.damage =       _damage;
    }
}
