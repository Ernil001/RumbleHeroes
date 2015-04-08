using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Network : MonoBehaviour 
{

    //
    //private const string roomName = "RoomName";
    private RoomInfo[] roomsList;
    //
    public GameObject playerPrefab;
    private string roomName = "Room - ";
    
    public GameObject playerNameInput;
    //public GameObject roomListUI;
    //public GameObject roomButton;
    //public GameObject roomListLocation;
    //public RoomInfo curRoom = PhotonNetwork.room;
    //
    //
    // Use this for initialization
    void Start()
    {
        //GameController.instance.errorDisplay_open("Testing static method call");
        //This function enables us host and join rooms of our game based on the appID of photon.
        PhotonNetwork.ConnectUsingSettings("0.1");
    }
    void OnGUI()
    {
        //PhotonNetwork.Connected; BOOL, checks if we are connected to the network AND NOT to another players server.
        if (!PhotonNetwork.connected)
        {
            GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        }
        else if (PhotonNetwork.room == null /*&& roomListUI.activeSelf*/)
        {
            // Create Room
            
            if (GUI.Button(new Rect(20, 20, 150, 50), "Start Server"))
                PhotonNetwork.CreateRoom(roomName + UnityEngine.Random.Range(0.00f,100.00f), true, true, 2);
            
            // Join Room

            if (roomsList != null)
            {
                for (int i = 0; i < roomsList.Length; i++)
                {
                    /*
                    GameObject go = Instantiate(roomButton, new Vector3(roomListLocation.GetComponent<Transform>().position.x, roomListLocation.GetComponent<Transform>().position.y - (i * 40), roomListLocation.GetComponent<Transform>().position.z), Quaternion.identity) as GameObject;
                    go.transform.FindChild("Text").GetComponent<Text>().text = roomsList[i].name;
                    go.transform.parent = roomListLocation.transform;
                    */

                    if (GUI.Button(new Rect(20, 200 + (110 * i), 150, 50), roomsList[i].name))
                        PhotonNetwork.JoinRoom(roomsList[i].name);
                }
            }
        }
    }
    
    public void createPhotonRoom()
    {
        Debug.Log("Starts createPhotonRoom");
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
            }

            GameController.instance.player1 = playerNameInput.GetComponent<InputField>().text;
        }
    }
    


    void OnReceivedRoomListUpdate()
    {
        roomsList = PhotonNetwork.GetRoomList();
    }
    //
    void photonJoinRoom_prepare()
    {
 
    }
    void OnJoinedRoom()
    {
        Debug.Log("Connected to Room");

        GameController.instance.changeActiveStatus(GameController.instance.roomLobby);
        GameController.instance.gameStatus = "roomLobby";
        GameController.instance.roomName.GetComponent<Text>().text = PhotonNetwork.room.name;
        /*
        Vector3 location = new Vector3(0, 0, 0);
        //PhotonNetwork.Instantiate(playerPrefab.name, Vector3.up * 5, Quaternion.identity, 0);
        PhotonNetwork.Instantiate(playerPrefab.name, location, Quaternion.identity, 0);
        */
    }
    // On leading a room in photon it should clear all textx inside the RoomLobby UI section.
    public void leavePhotonRoom()
    {
        if (PhotonNetwork.LeaveRoom())
        {
            Debug.Log("You have succesfully leave the room");
            //Cleans the UI on the client side
            GameController.instance.cleanRoomLobby();
            GameController.instance.changeActiveStatus(GameController.instance.roomLobby,"close");
            GameController.instance.gameStatus = "";

        }
        else GameController.instance.errorDisplay_open("Error while leaving the joined room");
    }
}
