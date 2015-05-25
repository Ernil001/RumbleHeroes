using UnityEngine;
using System.Collections;

public class Ability : Photon.MonoBehaviour 
{
    public string name;
    public int cd;
    // Time until object is self destroyed. if null is ignored
    public int secondsToLive;
    // Time of the object instantiation.
    protected float timeInstantiated;
    // Photon player owner id of the instantiated object
    protected int networkOwnerId;
    //Ability used by GameObject linked to PhotonView
    protected int ownerViewID;
    protected PhotonView ownerGameObject;
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
    }
    protected virtual void Start()
    {
        // This will be different for types of abilities / Projectile, Selfbuffs, Area
    }
    protected virtual void Update()
    {
        // Adopted with base.Update();
        //Object Duration
        if ((secondsToLive != null) && (secondsToLive != 0) && (Time.time - timeInstantiated >= secondsToLive))
        {
            Destroy(gameObject);
        }
    }
    protected virtual void forceDestroy()
    {
        Destroy(this.gameObject);
    }
}
