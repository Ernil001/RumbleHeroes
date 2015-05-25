using UnityEngine;
using System.Collections;

public abstract class Entity : MonoBehaviour 
{
    // Entity static variables
    public string entityName;
    public float speedBase;
    public GameObject Ability;
    public GameObject Ability2;
    public int maxHP;
    // Entity active variables
    public int currentHP;
    public float speed;
    
    // This class will be expanded when we know more what we can move here. Preparations maybe for some simple AI :X
}
