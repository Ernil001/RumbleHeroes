using UnityEngine;
using System.Collections;
using System;

public class DamageFloatingText : MonoBehaviour 
{
    //*************
    //
    //  This simple script is used for moving the floating text. Could be connected with some options.
    //
    //*************
    public float moveUp;
    public float moveLimit;
	//
    private GameObject parentHeroResource;
    //
    void Start () 
    {
        this.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 4.4f, 0);
        this.GetComponent<RectTransform>().localScale = new Vector3(0.1f, 0.1f, 0.1f);
        // If the hero resource will be changed, this path will also change.// Might have to make a more global thing in the future, i think the whole hero GAMEOBJECT is a bit bad. speciialy its UI.
        parentHeroResource = this.transform.parent.transform.parent.gameObject;
        //Debug.Log("Start");
	}
	void Update () 
    {
        // Rotate Text scale depending on the rotation of the hero, inverted.
        if (parentHeroResource.GetComponent<Transform>().localScale.x < 0f && this.GetComponent<RectTransform>().localScale.x > 0f)
        {
            this.GetComponent<RectTransform>().localScale = new Vector3(
                (this.GetComponent<RectTransform>().localScale.x * -1),
                this.GetComponent<RectTransform>().localScale.y,
                this.GetComponent<RectTransform>().localScale.z
            );
        }
        else if (parentHeroResource.GetComponent<Transform>().localScale.x > 0f && this.GetComponent<RectTransform>().localScale.x < 0f)
        {
            this.GetComponent<RectTransform>().localScale = new Vector3(
                Math.Abs(this.GetComponent<RectTransform>().localScale.x),
                this.GetComponent<RectTransform>().localScale.y,
                this.GetComponent<RectTransform>().localScale.z
            );
        }
        //moveUp a bit
        Vector3 curPos = this.GetComponent<RectTransform>().anchoredPosition;
        if (curPos.y < moveLimit)
        {
            //Debug.Log(curPos.y);
            this.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, (curPos.y + moveUp), 0);
            //this.GetComponent<RectTransform>().localPosition = new Vector3(0, (curPos.y + moveUp), 0);
        }
        else
        {
            Destroy(this.gameObject);
        }
	}
}