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

    // MAX possible players in lobby for cleaning or setting purposes
    public int maxPlayerPossible = 4;
    //
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
    public GameObject[] roomUIClassHolders;
    public GameObject[] extraOptionsUI;
    // List of Rooms
    public GameObject ListOfRoomsContent;
    public GameObject roomRow;  // prefab RoomRow
    public GameObject listOfRoomsScrollBar;
    // Hero selection
    public GameObject HeroSelectionUI;
    public GameObject listOfHeroes;
    public GameObject heroSelectionButton;
    public GameObject selectedHeroPortrait;
    public GameObject selectedHeroInformation;
    public GameObject selectedHeroName;

    public GameObject selectHeroButton;
    public GameObject selectHeroText;
    //

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
    }
    void Start()
    {

        //Populate the list of Heroes at Hero Selection UI
        populateHeroSelectionList();
        
    }
    //
    public void populateHeroSelectionList()
    {
        //Clean all childreen in the listOfHeroes
        destroyAllChildGameObjects(listOfHeroes);
        //
        if (PhotonNetwork.inRoom)
        {
            List<string> takenHeroes = new List<string>();
            // Adds already selected heroes.
            foreach (PhotonPlayer tarPlayer in PhotonNetwork.playerList)
            {
                //Disables taken or unavailable heroes
                if (!tarPlayer.isLocal)
                {
                    ExitGames.Client.Photon.Hashtable tm = tarPlayer.customProperties;
                    if (tm["h"] != "")
                    {
                        takenHeroes.Add(tm["h"].ToString());
                    }
                }
            }
            // Adds heroes that the player does not own - Possible later on with player accounting.
            for (int i = 0; i < HeroInformation.instance.heroes.Count; i++)
            {
                GameObject bu = Instantiate(heroSelectionButton, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                bu.transform.SetParent(listOfHeroes.transform);
                string tempHoldName = HeroInformation.instance.heroes[i].ToString();
                bu.GetComponent<Button>().onClick.AddListener(() => this.heroSelection(tempHoldName));
                bu.transform.FindChild("HeroName").GetComponent<Text>().text = HeroInformation.instance.heroes[i].Name + " (" + HeroInformation.instance.heroes[i].Class + ")";
                
                if (takenHeroes.Contains(HeroInformation.instance.heroes[i].Code))
                    bu.GetComponent<Button>().interactable = false;
                else
                    bu.GetComponent<Button>().interactable = true;
            }

        }
        else
        {
            for (int i = 0; i < HeroInformation.instance.heroes.Count; i++)
            {
                GameObject bu = Instantiate(heroSelectionButton, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                bu.transform.SetParent(listOfHeroes.transform);
                string tempHoldName = HeroInformation.instance.heroes[i].ToString();
                bu.GetComponent<Button>().onClick.AddListener(() => this.heroSelection(tempHoldName));
                bu.transform.FindChild("HeroName").GetComponent<Text>().text = HeroInformation.instance.heroes[i].Name + " (" + HeroInformation.instance.heroes[i].Class + ")";
            }

        }
    }
    // Select Hero
    // Parameters:
    //  -_heroName  - String value of the hero name. Example: Constantine
    //  - heroAvailable - Bool, true if he can select the hero, false if hero select is not possible. (Account for later when hero selecting will be limited. Might be deprecated since i might just populate the takenHero array isntead)
    void heroSelection(string _heroName = "", bool heroAvailable = true)
    {
        for (int i = 0; i < HeroInformation.instance.heroes.Count; i++)
        {
            if (HeroInformation.instance.heroes[i].ToString() == _heroName)
            {
                //Add picture or Animated prefab
                foreach (GameObject key in HeroInformation.instance.heroSelectionPrefabs_heroes)
                {
                    if (key.name == _heroName)
                    {
                        GameObject heroPortrait = Instantiate(key, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                        heroPortrait.transform.SetParent(selectedHeroPortrait.transform);
                        heroPortrait.GetComponent<Transform>().localPosition = new Vector3(0, -20, 0);
                    }
                }
                

                //Add Information Text
                selectedHeroInformation.GetComponent<Text>().text = HeroInformation.instance.heroes[i].Information;
                //Add Hero Name
                selectedHeroName.GetComponent<Text>().text = HeroInformation.instance.heroes[i].Name;
                break;
            }
        }
        //Debug.Log(_heroName);
    }
    // Confirm selected Hero
    public void confirmHeroSelection()
    {
        //CHeck if available in room properties.
            // If returns false, meaning the players changed their selection during the active selection process, Reloads populateHeroSelectionList()
        List<string> takenHeroes = new List<string>();
        foreach (PhotonPlayer tarPlayer in PhotonNetwork.playerList)
        {
            //Disables taken or unavailable heroes
            if (!tarPlayer.isLocal)
            {
                ExitGames.Client.Photon.Hashtable tm = tarPlayer.customProperties;
                if (tm["h"] != "")
                {
                    takenHeroes.Add(tm["h"].ToString());
                }
            }
        }
        string tmpSelectedHeroCode = "";
        // I wonder if this part could be rewritten to ask directly into the references of the objects inside the list, instead of looping through them ?
        string heroName = selectedHeroName.GetComponent<Text>().text;
        for(int x = 0; x < HeroInformation.instance.heroes.Count; x++)
        {
            if(heroName == HeroInformation.instance.heroes[x].Name)
                tmpSelectedHeroCode = HeroInformation.instance.heroes[x].Code;
        }
        // Asks if Hero Code is available to take.
        if(takenHeroes.Contains(tmpSelectedHeroCode))
        {
            // Populate the list again, Display Error that the hero is taken
            errorDisplay_open("This hero has been taken !");
            populateHeroSelectionList();
        }
        else
        {
            //Save into player properties (Rewrite)
            ExitGames.Client.Photon.Hashtable tmpTab = new ExitGames.Client.Photon.Hashtable();
            tmpTab.Add("h",tmpSelectedHeroCode);
            PhotonNetwork.player.SetCustomProperties(tmpTab);
            //return to roomView
            closeHeroSelectionPanel();
        }
    }
    // Opens heroSelection UI
    public void openHeroSelectionPanel()
    {
        //Debug.Log("Pride not");
        changeActiveStatus(HeroSelectionUI, "open");
        // List heroes again, check if there should be some disabled.
        populateHeroSelectionList();

    }
    public void closeHeroSelectionPanel()
    {
        changeActiveStatus(HeroSelectionUI, "close");
    }
    public void setPlayerHeroSelection(int pos, PhotonPlayer curClient)
    {
        ExitGames.Client.Photon.Hashtable playerCusProp = curClient.allProperties;

        if (roomUIClassHolders[pos].transform.childCount == 1)
        {
            if (curClient.isLocal)
            {
                if (playerCusProp["h"] != "")
                {

                    string tempHeroCode = playerCusProp["h"].ToString(); ;
                    roomUIClassHolders[pos].transform.GetChild(0).transform.FindChild("Text").GetComponent<Text>().text = HeroInformation.instance.return_HeroName_OnCode(tempHeroCode);
                }
                else
                {
                    roomUIClassHolders[pos].transform.GetChild(0).transform.FindChild("Text").GetComponent<Text>().text = "Select your Hero !";
                }
            }
            else
            {
                if (playerCusProp["h"] != "")
                {
                    string tempHeroCode = playerCusProp["h"].ToString();
                    roomUIClassHolders[pos].transform.GetChild(0).GetComponent<Text>().text = HeroInformation.instance.return_HeroName_OnCode(tempHeroCode);
                }
                else
                {
                    roomUIClassHolders[pos].transform.GetChild(0).GetComponent<Text>().text = "";
                }
            }
        }
        else if (roomUIClassHolders[pos].transform.childCount == 0)
        {
            if (curClient.isLocal)
            {
                GameObject sht = Instantiate(selectHeroButton, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                sht.transform.SetParent(roomUIClassHolders[pos].transform);
                sht.transform.localScale = Vector3.one;
                sht.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
                sht.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                sht.GetComponent<Button>().onClick.AddListener(() => openHeroSelectionPanel());
            }
            else
            {
                GameObject sht = Instantiate(selectHeroText, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                sht.transform.SetParent(roomUIClassHolders[pos].transform);
                sht.transform.localScale = Vector3.one;
                sht.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
                sht.GetComponent<RectTransform>().anchoredPosition =new Vector2(50,0);
                sht.GetComponent<Text>().text = "";
            }
        }
        else 
        {
            // We have a problem here >X
            errorDisplay_open("Too many GameObjects in " + roomUIClassHolders[pos].name, "0003");
        }
    }
    IEnumerator UpdateGameLobby ()
    {
        while(this.gameStatus == "roomLobby")
        {
            // Clean Players
            CleanPlayerRoomList();
            // Sorting
            Dictionary<int, PhotonPlayer> diCk = new Dictionary<int, PhotonPlayer>();

            foreach (PhotonPlayer tmpPlayer in PhotonNetwork.playerList)
            {
                //tmpPlayer.
                diCk.Add(tmpPlayer.ID, tmpPlayer);
            }

            List<int> list = diCk.Keys.ToList();
            list.Sort();

            int i = 0;
            foreach (int key in list)
            {
                //Debug.Log("ID: " + key + " Player Name: " + diCk[key].name + " is master?: " + diCk[key].isMasterClient);
                AddPlayerToRoomList(diCk[key].name, i, diCk[key].isMasterClient);
                ExitGames.Client.Photon.Hashtable playerProp = diCk[key].allProperties;
                setPlayerHeroSelection(i, diCk[key]);
                i++;
            }
            // Cleans class selections
            if (i < maxPlayerPossible) // ?
            {
                for (int k = i; k < maxPlayerPossible; k++)
                {
                    destroyAllChildGameObjects(this.roomUIClassHolders[k]);
                }
            }
            //
            setMasterOptionsForRoom();
            //Update once per second
            yield return new WaitForSeconds(1f);
        }
    }
    
    //
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
        for (int i = 0; i < maxPlayerPossible; i++)
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
        // Removing GameObjects under Class
        foreach (GameObject key in roomUIClassHolders)
        {
            if (key.transform.childCount > 0)
            {
                Destroy(key.transform.GetChild(0).gameObject);
            }
            
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
        bool windowStatus = true;
        // Hide All
        foreach (GameObject key in extraOptionsUI)
        {
            if (key.GetActive() == true && selectedExtra+"Wrap" == key.name)
            {
                windowStatus = false;
            }

            changeActiveStatus(key,"close");
        }
        // Show correct one.
        if (windowStatus)
        {
            Type thisType = this.GetType();
            MethodInfo theMethod = thisType.GetMethod("extraRoom_open" + selectedExtra);
            theMethod.Invoke(this, null);
            // Clears the Join ROom List on all  method calls
            listOfRooms_clearList();
        }
    }
    // Open options panel
    public void extraRoom_openOptions()
    {
        foreach (GameObject key in extraOptionsUI)
        {
            if (key.name == "OptionsWrap") changeActiveStatus(key, "open");
        }

    }
    public void extraRoom_openJoin()
    {
        foreach (GameObject key in extraOptionsUI)
        {
            if (key.name == "JoinWrap") changeActiveStatus(key, "open");
        }

        // What to do on open list of rooms ? List teh rooms ane. baaak.

        // Call function refreshPhotonRooms

    }
    public void extraRoom_closeJoin()
    {
        // Stopping the loop for finding rooms ?
    }
    public void extraRoom_openCreate()
    {
        foreach (GameObject key in extraOptionsUI)
        {
            if (key.name == "CreateWrap") changeActiveStatus(key, "open");
        }

    }
    // Clear all rooms in List Of Rooms
    public void listOfRooms_clearList()
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform tran in ListOfRoomsContent.transform)
        {
            children.Add(tran.gameObject);
        }
        foreach (GameObject key in children)
        {
            Destroy(key);
        }
    }
    public void destroyAllChildGameObjects(GameObject parentObj)
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform tran in parentObj.transform)
        {
            children.Add(tran.gameObject);
        }
        foreach (GameObject key in children)
        {
            Destroy(key);
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
        /*
        ExitGames.Client.Photon.Hashtable rp = new ExitGames.Client.Photon.Hashtable();
        
        rp.Add("h1", "H01"); // Key is player pos in room, NOT ID, value is hero selected
        rp.Add("h2", "H02");
        rp.Add("h3", "H03");
        rp.Add("h4", "H04");
        PhotonNetwork.room.SetCustomProperties(rp);
        */
        /*
        ExitGames.Client.Photon.Hashtable getHasTable = PhotonNetwork.room.customProperties;
        foreach (DictionaryEntry row in getHasTable)
        {
            Debug.Log(row.Key + "/" + row.Value);
        }
         * */

        //
        Debug.Log(HeroInformation.instance.return_HeroName_OnCode("H02"));
        Debug.Log(HeroInformation.instance.return_HeroCode_OnName("Constantine"));
        //   
        //PhotonNetwork.room.SetCustomProperties(1, "test");


        //Debug.Log(PhotonNetwork.masterClient.name + " -- " + PhotonNetwork.masterClient.ID.GetType());
    }
}
