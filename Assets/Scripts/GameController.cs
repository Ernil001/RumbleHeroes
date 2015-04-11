using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
    public GameObject[] roomUICleaningTexts;
    public GameObject[] roomUINames;
    public GameObject[] roomUIKickButtons;
    public GameObject[] extraOptionsUI;
    //

    // Predefined possiblities for allowedString 
    // "","roomLobby","running","endScore", 
    private string gameStatus = "";
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
            Dictionary<int, PhotonPlayer> diCk = new Dictionary<int, PhotonPlayer>();

            foreach (PhotonPlayer tmpPlayer in PhotonNetwork.playerList)
            {
                diCk.Add(tmpPlayer.ID, tmpPlayer);
            }

            List<int> list = diCk.Keys.ToList();
            list.Sort();

            int i = 0;
            foreach (int key in list)
            {
                //Debug.Log("ID: " + key + " Player Name: " + diCk[key].name + " is master?: " + diCk[key].isMasterClient);
                AddPlayerToRoomList(diCk[key].name, i, diCk[key].isMasterClient);
                i++;
                
            }
            setMasterOptionsForRoom();
            //Update once per second
            yield return new WaitForSeconds(1f);
        }
    }
    private void AddPlayerToRoomList(string PlayerName, int playerIndex, bool isMaster = false)
    {
        if (GameController.instance.roomUINames.Length > playerIndex)
        {
            if (isMaster)
            {
                GameController.instance.roomUINames[playerIndex].GetComponent<Text>().color = new Color32(255, 227, 0, 255);
                GameController.instance.roomUINames[playerIndex].GetComponent<Text>().text = PlayerName;
            }
            else
            {
                GameController.instance.roomUINames[playerIndex].GetComponent<Text>().color = new Color32(178, 178, 178, 255);
                GameController.instance.roomUINames[playerIndex].GetComponent<Text>().text = PlayerName;
            }
            
        }
    }
    private void CleanPlayerRoomList()
    {
        for(int i = 0; i < 4; i++)
        {
            if (GameController.instance.roomUINames.Length > i)
                GameController.instance.roomUINames[i].GetComponent<Text>().text = "";
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
    public void errorDisplay_open(string errorDescription, string errorId = "")
    {
        changeActiveStatus(errorUI, "open");
        if (errorId != "") errorText.GetComponent<Text>().text = "Error Code: " + errorId.ToString() + "\n Error Description:" + errorDescription;
        else errorText.GetComponent<Text>().text = errorDescription;
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
        foreach (GameObject key in roomUICleaningTexts)
        {
            key.GetComponent<Text>().text = "";
        }
    }
    // Set master options
    public void setMasterOptionsForRoom()
    {
        if (roomLobby.activeSelf)
        {
            for (int x = 0; x < roomUIKickButtons.Length; x++)
            {
                if (roomUINames[x].GetComponent<Text>().text != "") roomUIKickButtons[x].SetActive(PhotonNetwork.isMasterClient);
                else roomUIKickButtons[x].SetActive(false);
                //roomUIKickButtons[x].SetActive(PhotonNetwork.isMasterClient);
            }
        }
    }
    // Add to room console
    public void addToRoomConsole(string textToAdd, bool newLine = true)
    {
        string newLineSet = "\n";
        if (!newLine) newLineSet = "";
        if (GameController.instance.GameStatus == "roomLobby")
            GameController.instance.roomLobbyConsole.GetComponent<Text>().text += newLineSet + textToAdd;
        else errorDisplay_open("Something is trying to be display in the Console of an open room, however the room is not opened !", "0002");
    }
    //Display ExtraRoomUI
    // Parameter is a string value that invokes a method inside the GameController.instance
    public void displayExtraRoom(string selectedExtra)
    {
        // Hide All
        foreach (GameObject key in extraOptionsUI)
        {
            changeActiveStatus(key,"close");
        }
        // Show correct one.
        Type thisType = this.GetType();
        MethodInfo theMethod = thisType.GetMethod(selectedExtra);
        theMethod.Invoke(this, null);


    }
    // Open options panel
    public void extraRoom_openOptions()
    {
        foreach (GameObject key in extraOptionsUI)
        {
            if (key.name == "JoinRoomWrap") changeActiveStatus(key, "open");
        }
    }
    public void extraRoom_openJoin()
    {
        foreach (GameObject key in extraOptionsUI)
        {
            if (key.name == "JoinRoomWrap") changeActiveStatus(key, "open");
        }
    }
    public void extraRoom_openCreate()
    {
        foreach (GameObject key in extraOptionsUI)
        {
            if (key.name == "JoinRoomWrap") changeActiveStatus(key, "open");
        }
    }
    // Testing method linked to Testing Button
    public void testingMethod()
    {
        //var props : ExitGames.Client.Photon.Hashtable = new ExitGames.Client.Photon.Hashtable();

        /*
        Hashtable rp = new Hashtable();
        rp.Add("p1", "a string");
        rp.Add("p2", "a string");
        PhotonNetwork.room.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "prop1", "val" }, { "prop2", 10 } }, new string[] { "prop1" });
        */
        /*
        RoomOptions()
        */
        ExitGames.Client.Photon.Hashtable rp = new ExitGames.Client.Photon.Hashtable();
        //Hashtable rp = new ExitGames.Client.Photon.Hashtable();
        rp.Add("p1", "H01"); // Key is player pos in room, NOT ID, value is hero selected
        rp.Add("p2", "H02");
        rp.Add("p3", "H03");
        rp.Add("p4", "H04");
        PhotonNetwork.room.SetCustomProperties(rp);

        ExitGames.Client.Photon.Hashtable getHasTable = PhotonNetwork.room.customProperties;
        foreach (DictionaryEntry row in getHasTable)
        {
            Debug.Log(row.Key + "/" + row.Value);
        }
        //   
        //PhotonNetwork.room.SetCustomProperties(1, "test");


        //Debug.Log(PhotonNetwork.masterClient.name + " -- " + PhotonNetwork.masterClient.ID.GetType());
    }
}
