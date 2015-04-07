using UnityEngine;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Sync : MonoBehaviour {
    private Vector3 latestCorrectPos;
    private Vector3 onUpdatePos;
    private float fraction;
    PhotonView punView;

    void Awake()
    {
        punView = GetComponent<PhotonView>();

        if (punView.isMine)
        {
            this.enabled = false;   // due to this, Update() is not called on the owner client.
        }

        latestCorrectPos = transform.position;
        onUpdatePos = transform.position;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            Vector3 pos = transform.localPosition;
            Quaternion rot = transform.localRotation;
            stream.Serialize(ref pos);
            stream.Serialize(ref rot);
        }
        else
        {
            // Receive latest state information
            Vector3 pos = Vector3.zero;
            Quaternion rot = Quaternion.identity;

            stream.Serialize(ref pos);
            stream.Serialize(ref rot);
            Debug.Log(onUpdatePos + " - " + latestCorrectPos);
            latestCorrectPos = pos;                 // save this to move towards it in FixedUpdate()
            onUpdatePos = transform.localPosition;  // we interpolate from here to latestCorrectPos
            fraction = 0;                           // reset the fraction we alreay moved. see Update()

            transform.localRotation = rot;          // this sample doesn't smooth rotation
        }
    }
	
	// Update is called once per frame
    public void Update()
    {
        // We get 10 updates per sec. sometimes a few less or one or two more, depending on variation of lag.
        // Due to that we want to reach the correct position in a little over 100ms. This way, we usually avoid a stop.
        // Lerp() gets a fraction value between 0 and 1. This is how far we went from A to B.
        //
        // Our fraction variable would reach 1 in 100ms if we multiply deltaTime by 10.
        // We want it to take a bit longer, so we multiply with 9 instead.

        fraction = fraction + Time.deltaTime * 9;
        transform.localPosition = Vector3.Lerp(onUpdatePos, latestCorrectPos, fraction);    // set our pos between A and B
    }
}
