using UnityEngine;
using System.Collections;

public class InputKeys : MonoBehaviour {

    // This should be enough for now, after we inplement key configging we can adjust this class.
    public static InputKeys instance = null;
    // This value defines which keySet is selected to work.
    // Possible Values  - STRING
    //  - MainMenu
    //  - Game
    //  - GameMainMenu
    //  - GameDead // When a player is dead and is either waiting for respawn or waiting for round restart, same thing iknit >X
    //  - 
    public string inputType = "";
    /// <summary>
    /// Get/Set Either set the Input Type or return its value.
    /// </summary>
    public string InputType
    {
        set
        {
            this.inputType = value;
        }
        get
        {
            return this.inputType;
        }
    }
    //
	void Awake () 
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
	}
	//
	void Update () 
    {
        // Global Inputs
        InputGlobal();
        // Specific Inputs
        if (this.InputType == "MainMenu")
        {
            InputMainMenu();
        }
        else if (this.InputType == "Game")
        {
            InputGame();
        }
        else if (this.InputType == "GameMainMenu")
        {
            InputGameMainMenu();
        }
        else if (this.InputType == "GameDead")
        {
            InputGameDead();
        }
        
	}
    //
    //////////////////////
    // Global Input values that are active throughout the game statuses.
    private void InputGlobal()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            MouseCursorController.instance.BorderLockState = false;
        }
        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            MouseCursorController.instance.BorderLockState = true;
        }
        // For Testing
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            //GameController.instance.addKillPoint();
            // Testing for switching camera point of view.
            //GameController.instance.rotateDeathCamera();
            GameController.instance.errorDisplay_open(MouseCursorController.instance.defaultMouseCursor.name + " // " + Cursor.visible);
        }
    }
    //
    public void InputMainMenu()
    {

    }
    //
    public void InputGame()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            GameController.instance.changeActiveStatus(GameController.instance.UI_GameUI_ScoreBoard, true);
            MouseCursorController.instance.BorderLockState = false;
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            GameController.instance.changeActiveStatus(GameController.instance.UI_GameUI_ScoreBoard, false);
            MouseCursorController.instance.BorderLockState = true;
        }
        //
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.InputType = "GameMainMenu";
            GameController.instance.changeActiveStatus(GameController.instance.UI_mainMenu);
        }
    }
    public void InputGameDead()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            GameController.instance.changeActiveStatus(GameController.instance.UI_GameUI_ScoreBoard, true);
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            GameController.instance.changeActiveStatus(GameController.instance.UI_GameUI_ScoreBoard, false);
        }
        //
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.InputType = "GameMainMenu";
            GameController.instance.changeActiveStatus(GameController.instance.UI_mainMenu);
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GameController.instance.rotateDeathCamera();
        }
    }
    //
    public void InputGameMainMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.InputType = "Game";
            GameController.instance.changeActiveStatus(GameController.instance.UI_mainMenu);
        }
    }
    //
}
