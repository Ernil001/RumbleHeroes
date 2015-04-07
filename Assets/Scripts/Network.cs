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
    /*
    public GameObject playerNameInput;
    public GameObject roomListUI;
    public GameObject roomButton;
    public GameObject roomListLocation;
     * */
    //
    //
    // Use this for initialization
    void Start()
    {
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
    /*
    public void createPhotonRoom()
    {
        //string roomName = "RandomName" + UnityEngine.Random.Range(gRoomMin, gRoomMax).ToString();
        //Debug.Log(playerNameInput.GetComponent<InputField>().text);
        if (playerNameInput.GetComponent<InputField>().text == "")
        {
            return;
        }
        else
        {
            string roomName = "Room " + playerNameInput.GetComponent<InputField>().text;
            PhotonNetwork.CreateRoom(roomName, true, true, 2);
            //GameObject.FindGameObjectWithTag("LobbyUI").SetActive(false);
            GameObject.Find("LobbyUI").SetActive(false);
        }
    }
    */


    void OnReceivedRoomListUpdate()
    {
        roomsList = PhotonNetwork.GetRoomList();
    }

    void OnJoinedRoom()
    {
        Debug.Log("Connected to Room");
        Vector3 location = new Vector3(0, 0, 0);
        //PhotonNetwork.Instantiate(playerPrefab.name, Vector3.up * 5, Quaternion.identity, 0);
        PhotonNetwork.Instantiate(playerPrefab.name, location, Quaternion.identity, 0);
    }

    /*
    void OnJoinedRoom()
    {
        Debug.Log("Connected to Room");
        Vector3 location = new Vector3(UnityEngine.Random.Range(-5, 5), 0.5f, UnityEngine.Random.Range(-5, 5));
        //PhotonNetwork.Instantiate(playerPrefab.name, Vector3.up * 5, Quaternion.identity, 0);
        PhotonNetwork.Instantiate(playerPrefab.name, location, Quaternion.identity, 0);
    }
    public void joinRoomFromRoomList(GameObject room)
    {
        //PhotonNetwork.JoinRoom();
    }
    */
}
