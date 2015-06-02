using UnityEngine;
using System.Collections;

/*[RequireComponent(typeof(Rigidbody2D))]*/
[RequireComponent(typeof(Transform))]

public abstract class SelfBuff : Ability 
{
    // Duration of self buff in seconds Can also use Seconds to Live, both are viable, depending how you do the inheriting child An extra value to be able to play with.
    public int duration;
    // This simple value will represent a power of effect that will get used by the child class. Every self buff will have a POWER value.
    public int power;
    public float heightDisplay;
    // Duration for the visual effect on the hero
    public int visualDuration; // This variable is good that if we leave it empty it should be 0 sintead of undefined
    /// ///////////////////////////////////////////////////////////////////////////////////
    protected override void Awake()
    {
        base.Awake();
    }
    
	protected override void Start () 
    {
        
	}
    
    protected override void Update()
    {
        if (ownerGameObject != null)
        {
            Vector3 tmp_savePos = ownerGameObject.GetComponent<Transform>().position;
            this.GetComponent<Transform>().position = new Vector3(tmp_savePos.x, tmp_savePos.y + heightDisplay, tmp_savePos.z);
        }
        else forceDestroy();
        //
        base.Update();
        // Visual Duration
        if ( /*(visualDuration != null) &&*/ (visualDuration != 0) && (Time.time - timeInstantiated >= visualDuration))
        {
            GameController.instance.changeActiveStatus(this.gameObject, false);
        }
        //
	}
}
