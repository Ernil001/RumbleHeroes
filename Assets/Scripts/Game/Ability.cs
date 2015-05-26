using UnityEngine;
using System.Collections;

public class Ability : Photon.MonoBehaviour 
{
    //-***********
    //  THE THIRD INHERITING CHILD, which is the last child aka FireBolt, should have its name same as the prefab resource. 
    //  Exaple: FireBolt.prefab == FireBolt.cs
    //-***********
    public string visualName;
    public float cd;
    // Time until object is self destroyed. if null is ignored
    public int secondsToLive;
    // Time of the object instantiation.
    protected float timeInstantiated;
    // Photon player owner id of the instantiated object
    protected int networkOwnerId;
    // Ability used by GameObject linked to PhotonView
    protected int ownerViewID;
    protected PhotonView ownerGameObject;
    // Sounds - Audio Source is left alone, as long as the component is there is fine. 
    public AudioClip abilityInstantiate; // Played on object Awake callback
    public AudioClip abilityDuration; // This will be looped
    public AudioClip abilityOnDestroy; // Played when forceDestroy is activated.
    //
    protected Vector3 playerPos;
    protected Vector3 mousePos;
    public int Owner
    {
        set
        {
            this.networkOwnerId = value;
        }
        get
        {
            return this.networkOwnerId;
        }
    }
    //
    protected virtual void Awake()
    {
        timeInstantiated = Time.time;
        // Get instantiated data from photonView for the objects, It gets it from PlayerController
        this.Owner = (int)this.GetComponent<PhotonView>().instantiationData[0];
        playerPos = (Vector3)this.GetComponent<PhotonView>().instantiationData[1];
        mousePos = (Vector3)this.GetComponent<PhotonView>().instantiationData[2];
        ownerViewID = (int)this.GetComponent<PhotonView>().instantiationData[3];
        ownerGameObject = PhotonView.Find(ownerViewID);
        // Play sound abilityInstantiate
        if (abilityInstantiate != null)
        {
            this.GetComponent<AudioSource>().PlayOneShot(abilityInstantiate, 1);
        }
    }
    protected virtual void Start()
    {
        // This will be different for types of abilities / Projectile, Selfbuffs, Area
    }
    protected virtual void Update()
    {
        // Adopted with base.Update();

        // Object Duration
        if ((secondsToLive != null) && (secondsToLive != 0) && (Time.time - timeInstantiated >= secondsToLive))
        {
            Destroy(gameObject);
        }
    }
    // This method represents the selfdestroction of the ability. It has a linked sound to it. Should be used for isntance on impacts.
    protected virtual void forceDestroy()
    {
        // Play sound for abilityOnDestroy
        if (abilityOnDestroy != null)
        {
            this.GetComponent<AudioSource>().PlayOneShot(abilityOnDestroy, 1);
        }
        //
        Destroy(this.gameObject);
    }
}
