using UnityEngine;
using System.Collections;

public class MouseCursorController : MonoBehaviour 
{
    //
    public Texture2D defaultMouseCursor;
    //
    public static MouseCursorController instance = null;
    void Awake()
    {
        // Makes the current instance a static one.
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        //
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        //defaultMouseCursor.te
        changeMouseCursor();
        //Cursor.lockState = CursorLockMode.Confined;

    }
    void Update()
    {
        //changeMouseCursor();
        Cursor.lockState = CursorLockMode.Confined;
    }
    /// <summary>
    /// Change the cursor used in game to a texture2d
    /// </summary>
    public void changeMouseCursor()
    {
        Cursor.SetCursor(defaultMouseCursor, new Vector2(defaultMouseCursor.width / 2f, defaultMouseCursor.height / 2f), CursorMode.Auto);
    }
    public void changeMouseCursor(Texture2D textureObj)
    {
        Cursor.SetCursor(textureObj, new Vector2(textureObj.width / 2f, textureObj.height / 2f), CursorMode.Auto);
    }

}
