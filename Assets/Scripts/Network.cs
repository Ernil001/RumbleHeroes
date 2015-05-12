using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
public class Network : MonoBehaviour 
{
    //
    //private const string roomName = "RoomName";
    private RoomInfo[] roomsList;
    //
    public GameObject playerPrefab;
    //private string roomName = "Room - ";
    
    public GameObject playerNameInput;
    //
    private IEnumerator setRoomProp;
    //

    // Use this for initialization
    void Start()
    {
        //This function enables us host and join rooms of our game based on the appID of photon.
        PhotonNetwork.ConnectUsingSettings("0.1");
    }
    public void createPhotonRoom()
    {
        if (playerNameInput.GetComponent<InputField>().text == "")
        {
            GameController.instance.errorDisplay_open("You need to enter your name before Creating a new room !");
            return;
        }
        else
        {
            bool tempRoomCreated = false;
            int addToName = 0;
            while (!tempRoomCreated)
            {

                string temp_roomName;
                if (addToName == 0) temp_roomName = "Room " + playerNameInput.GetComponent<InputField>().text;
                else temp_roomName = "Room " + playerNameInput.GetComponent<InputField>().text + "(" + addToName.ToString() + ")";

                if (PhotonNetwork.CreateRoom(temp_roomName, true, true, 4)) tempRoomCreated = true;
                //Adding Raw values to server might be after or before not sure yet.
            }
            if (tempRoomCreated && PhotonNetwork.inRoom)
            {
                // Prepare all properties for the room
                setRoomProperties();

            }
            else
            {
                setRoomProp = waitForRoomCreation();
                StartCoroutine(setRoomProp);
            } 
            //GameController.instance.errorDisplay_open("kle prie");
        }
    }
    //
    IEnumerator waitForRoomCreation()
    {
        while (true)
        {
            if (PhotonNetwork.inRoom)
            {
                setRoomProperties();
                StopCoroutine(setRoomProp);
                //Debug.Log("Working");
            }
            yield return new WaitForSeconds(0.5f);
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
        prop.Add("sc", ""); // score to obtain to win
        /*
        prop.Add("h1", "");
        prop.Add("h2", "");
        prop.Add("h3", "");
        prop.Add("h4", "");
        */
        //Saave
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

        // Clear all rooms first
        GameController.instance.listOfRooms_clearList();
        //Populate refresh
        if (roomsList != null)
        {
            string temp_roomNameHold;
            for (int i = 0; i < roomsList.Length; i++)
            {
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
        if (playerNameInput.GetComponent<InputField>().text != "")
        {
            PhotonNetwork.JoinRoom(roomName_photon);
        }
        else
        {
            GameController.instance.errorDisplay_open("You need to enter your name before Joining a Room!");
            return;
        }
    }
    void OnJoinedRoom()
    {
        //Here set name on your id.
        setYourPhotonName(playerNameInput.GetComponent<InputField>().text);
        //Set player properties.
        setNewPlayerCusProperties(/*playerNameInput.GetComponent<InputField>().text*/);
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
        cusProp.Add("hs", "a");
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
    // testing 
    public void OnPhotonCustomRoomPropertiesChanged()
    {
        //GameController.instance.errorDisplay_open("Something changed !");
    }
}
