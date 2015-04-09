using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour 
{
    public static GameController instance = null;
    // Player name input
    public GameObject playerNameInput;

    // Ui Section GameObjects
    public GameObject errorUI;
    public GameObject errorText;
    //
    public GameObject roomLobbyConsole;
    public GameObject roomLobby;
    public GameObject roomName;
    public GameObject[] roomUINames;
    //

    // Predefined possiblities for allowedString 
    // "","roomLobby","running","endScore", 
    public string gameStatus = "";
    /*
    public string player1 = "";
    public string player2 = "";
    public string player3 = "";
    public string player4 = "";
    */
    //

    void Awake()
    {
        // Makes the current instance a static one.
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        //
        DontDestroyOnLoad(gameObject);
        //this.errorDisplay_open("test");
           
    }

    private void AddPlayerToRoomList(string PlayerName, int playerIndex)
    {
        if(GameController.instance.roomUINames.Length > playerIndex)
            GameController.instance.roomUINames[playerIndex].GetComponent<Text>().text = PlayerName;
    }

    void Update()
    {
        if (gameStatus == "roomLobby")
        {
            int i = 0;

            foreach (PhotonPlayer key in PhotonNetwork.playerList)
            {
                i++;

                AddPlayerToRoomList(key.name, i);
            }


        }
        else if (gameStatus == "running")
        {
 
        }
        else if (gameStatus == "endScore")
        {

        }
        /*
        else if (gameStatus == "")
        {
 
        }
        */
    }
    // Static RPC for GameInfo and updates

    //
    public void errorDisplay_open(string errorDescription)
    {
        changeActiveStatus(errorUI, "open");
        errorText.GetComponent<Text>().text = errorDescription;
    }
    public void errorDisplay_close()
    {
        errorText.GetComponent<Text>().text = "";
        changeActiveStatus(errorUI, "close");
    }
    // Utility function to change an objects activit from false to true or the other way around.
    // Possible paramaters for force (STRICT ALLOWED: STRING - "open" || "close")
    public void changeActiveStatus(GameObject targetObj, string force = "")
    {
        if (force == "")
        {
            if (targetObj.activeSelf == true) targetObj.SetActive(false);
            else targetObj.SetActive(true);
        }
        else if (force == "open") targetObj.SetActive(true);
        else if(force == "close") targetObj.SetActive(false);
    }
    // CLeaning up RoomLobbyUI on leave room.
    public void cleanRoomLobby()
    {
        foreach (GameObject key in roomUINames)
        {
            key.GetComponent<Text>().text = "";
        }
    }
    // Testing method linked to Testing Button
    public void testingMethod()
    {
        Debug.Log(GameController.instance.roomUINames[1].name.GetType());
    }
}
