using UnityEngine;
using System.Collections;

public abstract class Entity : MonoBehaviour 
{
    // Entity static variables
    public string entityName;
    public string entityClass;
    public string entityType;
    public string entityInformation;
    //public string entityController;
    public float speedBase;

    public GameObject Ability;
    public GameObject Ability2;
    public int maxHP;
    // Entity active variables
    public int currentHP;
    public float speed;
    public bool ability_ready = true;
    public bool ability2_ready = true;
    public float ability_lockRelease;
    public float ability2_lockRelease;
    // This class will be expanded when we know more what we can move here. Preparations maybe for some simple AI :X
}
