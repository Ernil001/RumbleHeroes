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
    public string inputType = "MainMenu";
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
        // For Testing
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            //GameController.instance.addKillPoint();
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
    public void InputGameDead()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.InputType = "Game";
            GameController.instance.changeActiveStatus(GameController.instance.UI_mainMenu);
        }
    }
}
