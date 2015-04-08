using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Network_Kekke : MonoBehaviour
{
    private const string roomName = "RoomName";
    private RoomInfo[] roomsList;

    public GameObject playerPrefab;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings("0.1");
    }
    void OnGUI()
    {

        if (!PhotonNetwork.connected)
        {
            GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
        }
        else if (PhotonNetwork.room == null)
        {
            // Create Room
            if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
                PhotonNetwork.CreateRoom(roomName + UnityEngine.Random.Range(0.00f, 100.00f).ToString(), true, true, 5);
 
            // Join Room
            if (roomsList != null)
            {
                for (int i = 0; i < roomsList.Length; i++)
                {
                    if (GUI.Button(new Rect(100, 250 + (110 * i), 250, 100), "Join " + roomsList[i].name))
                        PhotonNetwork.JoinRoom(roomsList[i].name);
                }
            }
        }
    }

    void OnReceivedRoomListUpdate()
    {
        roomsList = PhotonNetwork.GetRoomList();
    }
    /*
    void OnJoinedRoom()
    {
        Debug.Log("Connected to Room");
    }
    */
    void OnJoinedRoom()
    {
        Debug.Log("Connected to Room");
        Vector3 location = new Vector3(0, 0, 0);
        //PhotonNetwork.Instantiate(playerPrefab.name, Vector3.up * 5, Quaternion.identity, 0);
        PhotonNetwork.Instantiate(playerPrefab.name, location, Quaternion.identity, 0);
    }
}
