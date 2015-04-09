using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour 
{
    public static GameController instance = null;
    // Player name input
    public GameObject playerNameInput;

    // Ui Section GameObjects
    public GameObject errorUI;
    public GameObject errorText;
    public int[] idList = new int[4];
    //
    public GameObject roomLobbyConsole;
    public GameObject roomLobby;
    public GameObject roomName;
    public GameObject[] roomUINames;
    public GameObject[] roomUIKickButtons;
    //

    // Predefined possiblities for allowedString 
    // "","roomLobby","running","endScore", 
    private string gameStatus = "";
    /*
    public string player1 = "";
    public string player2 = "";
    public string player3 = "";
    public string player4 = "";
    */
    //

    //gameStatus properties
    public string GameStatus
    {
        set
        {
            //Always set the gameStatus to value;
            this.gameStatus = value;

            //If we enter room lobby, start coroutine
            if(value == "roomLobby")
            {
                StartCoroutine(UpdateGameLobby());
            }
        }
        get
        {
            return this.gameStatus;
        }
    }

    void Awake()
    {
        // Makes the current instance a static one.
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        //
        DontDestroyOnLoad(gameObject);
        //this.errorDisplay_open("test");
           
    }

    IEnumerator UpdateGameLobby ()
    {
        while(this.gameStatus == "roomLobby")
        {
            // Clean
            CleanPlayerRoomList();
            // Sorting
            
            PhotonPlayer[] test = PhotonNetwork.playerList;
           
            //Dictionary<int, string> nameOfDic = new Dictionary<int, string>();
            //int[] idList;
            /*
            int x = 0;
            foreach (PhotonPlayer key in PhotonNetwork.playerList)
            {
                idList[x] = key.ID;
                x++;
            }
            */
            //Array.Sort(idList);
            
           
            //
            int i = 0;

            foreach (PhotonPlayer key in PhotonNetwork.playerList)
            {
                i++;

                if(key.isMasterClient) AddPlayerToRoomList(key.name, i, true);
                else AddPlayerToRoomList(key.name, i);
                //Debug.Log(key.ID + " -- " + key.name);
            }

            //Update once per second
            yield return new WaitForSeconds(1f);
        }
    }
    private void AddPlayerToRoomList(string PlayerName, int playerIndex, bool isMaster = false)
    {
        if (isMaster) PlayerName = PlayerName + "*";
        if (GameController.instance.roomUINames.Length > playerIndex)
            GameController.instance.roomUINames[playerIndex].GetComponent<Text>().text = PlayerName;
    }
    private void CleanPlayerRoomList()
    {
        for(int i = 1; i < 5; i++)
        {
            if (GameController.instance.roomUINames.Length > i)
                GameController.instance.roomUINames[i].GetComponent<Text>().text = 
                    "Player - " + i + " not connected";
        }
    }
    void Update()
    {
        if (gameStatus == "roomLobby")
        {

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
        foreach (int value in idList)
        {
            Debug.Log(value);
        }
        //Debug.Log(PhotonNetwork.masterClient.name + " -- " + PhotonNetwork.masterClient.ID.GetType());
    }
}
