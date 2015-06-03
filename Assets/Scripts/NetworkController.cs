using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
public class NetworkController : Photon.MonoBehaviour 
{
    public static NetworkController instance = null;
    //
    private RoomInfo[] roomsList;
    //
    public GameObject playerPrefab;
    private int roomNameExtension;
    private bool connectedToMaster = false;
    //
    private string Username
    {
        get
        {
            return OptionController.instance.Username;
        }
    }
    //
    private IEnumerator setRoomProp;
    //
    void Awake()
    {
        // Makes the current instance a static one.
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        //
        DontDestroyOnLoad(gameObject);
    }
    // Use this for initialization
    void Start()
    {
        //This function enables us host and join rooms of our game based on the appID of photon.
        PhotonNetwork.ConnectUsingSettings("0.1");
    }
    // Main Photon resolvers for the network
    public void OnConnectedToPhoton()
    {
        //Debug.Log("Connected to Master");
        this.connectedToMaster = true;
    }
    public void OnFailedToConnectToPhoton()
    {
        Debug.Log("Failed to connect to Photon.");
        this.connectedToMaster = false;
        // This area will need a proper resolve for the game. Like disabling the createRoom etc etc.. Might be more linked to GameController this.

    }
    //
    public void createPhotonRoom(int _roomNameExtension = 0)
    {
        //Debug.Log(_roomNameExtension);
        if (_roomNameExtension != 0)
        {
            roomNameExtension = _roomNameExtension;
        }
        //
        if (Username == "")
        {
            GameController.instance.errorDisplay_open("You need to enter your name in Options before Creating a new room !");
            return;
        }
        else if (connectedToMaster)
        {

            string temp_roomName;
            if (roomNameExtension == 0) temp_roomName = "Room " + Username;
            else temp_roomName = "Room " + Username + " (" + roomNameExtension.ToString() + ")";
            //
            RoomOptions roomOpt = new RoomOptions();
            roomOpt.maxPlayers = 4;
            roomOpt.isOpen = true;
            roomOpt.isVisible = true;
            PhotonNetwork.CreateRoom(temp_roomName, roomOpt, null);
            //GameController.instance.errorDisplay_open("kle prie");
        }
        else 
        {
            Debug.Log(connectedToMaster);
        }
    }
    public void OnCreateRoom() //This callback doesnt work for some reason
    {
        Debug.Log("Accessed OnCreateRoom callback from PUN");
        //if (PhotonNetwork.isMasterClient) setRoomProperties();
        //else GameController.instance.errorDisplay_open("ERROR: OnCreateRoom() was called even when you were not the master client for the room.");
    }
    // On room fail to create room
    public void OnPhotonCreateRoomFailed(object[] CodeMsg)
    {
        // If failed to create room due to Name Error Request another room creation with different roomName
        if (Convert.ToInt32(CodeMsg[0]) == 32766)
        {
            createPhotonRoom(roomNameExtension+1);
        }
        else 
        {
            GameController.instance.errorDisplay_open("ERROR: Failed to create the room. \nPhotonErrorID: " + CodeMsg[0].ToString() + "\n PhotonErrorMSG: " + CodeMsg[1].ToString());
        }
    }
    // Prepares defaults for hero selection
    public void setRoomProperties()
    {
        ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable();
        // hero selection
        prop.Add("h1", "");
        prop.Add("h2", "");
        prop.Add("h3", "");
        prop.Add("h4", "");
        // Room Mode Settings // These settings will be taken from the UI later, now it will set default as Round Match // 10 kills
        prop.Add("gm", "RoundMatch"); // game mode
        prop.Add("rk", "0"); // RoundKills
        prop.Add("sc", "10"); // score to obtain to win
        //Save
        PhotonNetwork.room.SetCustomProperties(prop);
    }
    //
    void OnReceivedRoomListUpdate()
    {
        roomsList = PhotonNetwork.GetRoomList();
    }
    //
    // Refresh ListOfRoom
    public void refreshPhotonRooms()
    {
        roomsList = PhotonNetwork.GetRoomList();
        //Debug.Log("Refresh");
        // Clear all rooms first
        GameController.instance.listOfRooms_clearList();
        //Populate refresh
        if (roomsList != null)
        {
            string temp_roomNameHold;
            for (int i = 0; i < roomsList.Length; i++)
            {
                //Debug.Log(i);
                GameObject go;
                go = Instantiate(GameController.instance.roomRow, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                go.transform.parent = GameController.instance.ListOfRoomsContent.transform;
                go.transform.localScale = new Vector3(1, 1, 1);
                go.transform.FindChild("RoomName").GetComponent<Text>().text = roomsList[i].name;
                //Debug.Log("i: " + i.ToString() + "/ name:" + roomsList[i].name);
                temp_roomNameHold = roomsList[i].name;
                go.transform.FindChild("JoinButton").GetComponent<Button>().onClick.AddListener(() => this.photonJoinRoom_prepare(temp_roomNameHold));
                go.transform.FindChild("Players").GetComponent<Text>().text = roomsList[i].playerCount.ToString()+"/"+roomsList[i].maxPlayers.ToString();
            }
        }
    }
    //Join room handle
    void photonJoinRoom_prepare(string roomName_photon)
    {
        if (Username == "")
        {
            GameController.instance.errorDisplay_open("You need to enter your name before Joining a Room!");
            return;
        }
        else if (connectedToMaster)
        {
            PhotonNetwork.JoinRoom(roomName_photon);
        }
        else 
        {
            GameController.instance.errorDisplay_open("Not Connected to MasterPhoton. Try again.");
        }
    }
    void OnJoinedRoom()
    {
        //
        if (PhotonNetwork.isMasterClient) setRoomProperties();
        //Here set name on your id.
        setYourPhotonName(Username);
        //Set player properties.
        setNewPlayerCusProperties();
        // Set room view and create the update loop for room
        GameController.instance.changeActiveStatus(GameController.instance.roomLobby);
        GameController.instance.GameStatus = "roomLobby";
        GameController.instance.roomName.GetComponent<Text>().text = PhotonNetwork.room.name;
        GameController.instance.addToRoomConsole("Connected !");
        
    }
    void setNewPlayerCusProperties()
    {
        //Set default values for custom properties
        ExitGames.Client.Photon.Hashtable cusProp = new ExitGames.Client.Photon.Hashtable();
        // Selected hero
        cusProp.Add("h", "");
        // Kills
        cusProp.Add("k", "0");
        // Deaths
        cusProp.Add("d", "0");
        // Hero Status
        // - a - Alive
        // - d - Dead
        cusProp.Add("hs", "");
        // Hero HP MAX
        cusProp.Add("hp", "0");
        cusProp.Add("mhp", "0");
        PhotonNetwork.player.SetCustomProperties(cusProp);
    }
    // On leading a room in photon it should clear all textx inside the RoomLobby UI section. MANUAL LEAVE
    public void leavePhotonRoom()
    {
        if (PhotonNetwork.LeaveRoom())
        {
            //Cleans the UI on the client side
            cleanAfterRoomLeft();

        }
        else GameController.instance.errorDisplay_open("Error while leaving the joined room");
    }
    private void setYourPhotonName(string newName)
    {
        PhotonNetwork.playerName = newName;
        //PhotonNetwork.player.name = newName;
    }
    // Remove player from Room // Kick
    public void removePhotonPlayer(GameObject targObj)
    {
        // Might need to redo the GameObjects array for room lobby to faciliate IDs aswell
        bool kickSucc = false; 
        if(PhotonNetwork.isMasterClient)
        {
            foreach (PhotonPlayer key in PhotonNetwork.playerList)
            {
                if (key.name == targObj.GetComponent<Text>().text)
                {
                    if(PhotonNetwork.CloseConnection(PhotonPlayer.Find(key.ID)))
                        kickSucc = true;
                    break;
                }
            }
            if (kickSucc)
            {
                GameController.instance.addToRoomConsole("You have kicked a player " + targObj.GetComponent<Text>().text);
            }
            else GameController.instance.errorDisplay_open("Error: Occured while trying to kick the player.","0001");
        }
        //Debug.Log("removePhotonPlayer named: " + targObj.GetComponent<Text>().text);
    }
    // Updates for console while in Photon Room and gameStatus == "roomLobby"
    public void OnPhotonPlayerConnected()
    {
        GameController.instance.addToRoomConsole("Player has connected");
    }
    public void OnPhotonPlayerDisconnected()
    {
        GameController.instance.addToRoomConsole("Player has disconected");

        if (GameController.instance.GameStatus == "running")
        {
            //reload the UI elements.
            GameController.instance.gameUI_loadElements();
        }
    }
    public void OnLeftRoom()
    {
        // CHeck if you were kicked
        if (GameController.instance.GameStatus != "")
        {
            GameController.instance.errorDisplay_open("You have been Kicked");
            cleanAfterRoomLeft();
        }
    }
    // Room has been left one way or another, time to clean up !
    public void cleanAfterRoomLeft()
    {
        GameController.instance.cleanRoomLobby();
        GameController.instance.changeActiveStatus(GameController.instance.roomLobby, "close");
        GameController.instance.GameStatus = "";
    }
    // Quick join an available room.
    public void quickJoinGameRoom()
    {
        // Probably not, but in future might need an IF here to check which type of lobby you are connecting to. but i doubt it :D
        // This will be kept as simple for now. No Point creating a proper searching algorythm.

        if (Username == "")
        {
            GameController.instance.errorDisplay_open("You need to enter your name before Joining a Room!");
            return;
        }
        else if (connectedToMaster)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            GameController.instance.errorDisplay_open("Not Connected to MasterPhoton. Try again.");
        }
        //
    }

    // testing 
    public void OnPhotonCustomRoomPropertiesChanged()
    {
        /*
        Hashtable props = playerAndUpdatedProps[1] as Hashtable;
        Debug.Log("Changed custom properties:" + props["hs"].ToString());
         * */
        //GameController.instance.errorDisplay_open("Something changed !");
    }
}
