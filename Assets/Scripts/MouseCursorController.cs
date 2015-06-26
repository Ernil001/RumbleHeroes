using UnityEngine;
using System.Collections;

public class MouseCursorController : MonoBehaviour 
{
    //
    public Texture2D defaultMouseCursor;
    //
    private bool borderLockState;
    /// <summary>
    /// Locks the cursor to the borders of the Window
    /// </summary>
    public bool BorderLockState
    {
        get 
        {
            return borderLockState;
        }
        set
        {
            borderLockState = value;
        }
    }
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
        //changeMouseCursor();
        BorderLockState = false;
    }
    void Update()
    {
        changeMouseCursor();
        if (BorderLockState) Cursor.lockState = CursorLockMode.Confined;
        else Cursor.lockState = CursorLockMode.None;
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
