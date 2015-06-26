using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class GameController : Photon.MonoBehaviour 
{
    //
    public string GameVersion;
    // All varied Tags for searching parameters
    public string tag_Player;
    public string tag_MapSpawn;
    public string tag_Ground;
    public string tag_NormalWall;
    //
    public Material[] skyBox;

    //Private vars, cuz i can' // private is private no more ! Private ! Sir Yes Sir !
    private List<Vector3> spawnPositions;

    public static GameController instance = null;
    // Design color variables tbh dunno why, just felt like it :D
    // r - roomLobby
    // b - Basic colors
    public Color32 bMain = new Color32();
    public Color32 rRegBack = new Color32(255,255,255,0);
    public Color32 rDifBack = new Color32(67, 88, 70, 255);

    public Color32 bGreen = new Color32(35, 58, 0, 150);
    public Color32 bRed = new Color32(128, 27, 27, 150);
    //Main UI layers
    public GameObject UI_mainMenu;
    public GameObject UI_game;
    // Player name input
    public GameObject playerNameInput;

    // MAX possible players in lobby for cleaning or setting purposes
    public int maxPlayerPossible = 4;
    //
    // Ui Section GameObjects
    public GameObject DamageFloatingText;
    public GameObject errorUI;
    public GameObject errorText;
    public GameObject CreateNew_RoomName;
    public GameObject UI_GameUI_Top;
    public GameObject UI_GameUI_ScoreBoard;
    public GameObject UI_GameUI_ScoreBoard_Score;
    public GameObject UI_GameUI_ScoreBoard_GameModeDescription;
    public GameObject UI_GameUI_Bottom_Center_FirstAbilityName;
    public GameObject UI_GameUI_Bottom_Center_SecondAbilityName;

    public GameObject UI_GameUI_Bottom_Center_FirstAbilityCD;
    public GameObject UI_GameUI_Bottom_Center_SecondAbilityCD;

    public GameObject UI_MainMenuUI_MainMenuWrap_InputField; //This will be remade later.
    public GameObject UI_MainMenuUI_MainMenuWrap_CreateRoom;
    public GameObject UI_MainMenuUI_MainMenuWrap_QuickJoin;
    public GameObject UI_MainMenuUI_MainMenuWrap_JoinRoom;
    public GameObject UI_MainMenuUI_MainMenuWrap_Credits;
    public GameObject UI_MainMenuUI_MainMenuWrap_ReturnToGame;
    public GameObject UI_MainMenuUI_MainMenuWrap_ReturnToMainMenu;

    /// <summary>
    /// Ui Text GameObject that is a mini console for displaying CURRENT PhotonConnection Information.
    /// </summary>
    public GameObject UI_MainMenuUI_MainMenuWrap_PhotonConnection;
    public GameObject UI_MainMenuUI_MainMenuWrap_GameVersion;
    public GameObject UI_MainMenuUI_MainMenuWrap_ServerPing;
    //
    public GameObject roomLobbyConsole;
    public GameObject roomLobby;
    public GameObject roomName;
    public GameObject[] roomUICleaningTexts;
    public GameObject[] roomUINames;
    public GameObject[] roomUIKickButtons;
    public GameObject[] roomUIClassHolders;
    public GameObject[] extraOptionsUI;
    // Prefabs
    public GameObject roomLobbyStartButton;
    public GameObject gamePlayerIcon;
    public GameObject score_PlayerWrap;
    // Maps
    public GameObject[] mapsFolder;
    /// <summary>
    /// An array of active player heroes for the current round   (Collects GameObjects with tag "Player")
    /// </summary>
    public GameObject[] activePlayerHeroes;
    /// <summary>
    /// Your local Hero resource
    /// </summary>
    public GameObject activeLocalHero = null;
    // Main Camera
    public GameObject mainCamera;
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
    //private IEnumerator prepareNextRound;
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
            else if (value == "running")
            {
                StartCoroutine(UpdateGameScreen());
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
        // Set Default Main Menu acting
        setEnvironment_mainMenu();
        //Populate the list of Heroes at Hero Selection UI
        populateHeroSelectionList();
        // Set version
        UI_MainMenuUI_MainMenuWrap_GameVersion.GetComponent<Text>().text = GameVersion;
        /*
        Resolution[] resolutions = Screen.resolutions;
        foreach (Resolution test in resolutions)
        {
            Debug.Log(test.width + "x" + test.height);
        }
        Debug.Log(resolutions.Length);
        */
        //Screen.SetResolution(resolutions[0].width, resolutions[0].height, true);
    }
    //
    private void setEnvironment_mainMenu()
    {
        //
        InputKeys.instance.InputType = "MainMenu";
        // Hide unnecessary buttons and show the neccesarry ones
        changeActiveStatus(UI_MainMenuUI_MainMenuWrap_CreateRoom, true);
        changeActiveStatus(UI_MainMenuUI_MainMenuWrap_QuickJoin, true);
        changeActiveStatus(UI_MainMenuUI_MainMenuWrap_JoinRoom, true);
        changeActiveStatus(UI_MainMenuUI_MainMenuWrap_Credits, true);
        //changeActiveStatus(UI_MainMenuUI_MainMenuWrap_InputField, true);
        changeActiveStatus(UI_MainMenuUI_MainMenuWrap_ReturnToGame, false);
        changeActiveStatus(UI_MainMenuUI_MainMenuWrap_ReturnToMainMenu, false);
        //
        this.GameStatus = "";
        //
        changeActiveStatus(this.UI_mainMenu, true);
        changeActiveStatus(this.UI_game, false);
        //
        changeActiveStatus(UI_GameUI_ScoreBoard, false);
        //
        selectSkyBox();
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
                    if (tm["h"].ToString() != "")
                    {
                        takenHeroes.Add(tm["h"].ToString());
                    }
                }
            }
            // Adds heroes that the player does not own - Possible later on with player accounting.

            // Populate the list
            foreach (GameObject hero in HeroInformation.instance.allHeroes)
            {
                GameObject bu = Instantiate(heroSelectionButton, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                bu.transform.SetParent(listOfHeroes.transform);
                string temp_holdName = hero.GetComponent<PlayerController>().entityName;
                bu.GetComponent<Button>().onClick.AddListener(() => this.heroSelection(temp_holdName));
                bu.transform.FindChild("HeroName").GetComponent<Text>().text = hero.GetComponent<PlayerController>().entityName;
                
                if (takenHeroes.Contains(hero.GetComponent<PlayerController>().heroCode))
                    bu.GetComponent<Button>().interactable = false;
                else
                    bu.GetComponent<Button>().interactable = true;
            }

        }
        else
        {
            foreach (GameObject hero in HeroInformation.instance.allHeroes)
            {
                GameObject bu = Instantiate(heroSelectionButton, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                bu.transform.SetParent(listOfHeroes.transform);
                string temp_holdName = hero.GetComponent<PlayerController>().entityName;
                bu.GetComponent<Button>().onClick.AddListener(() => this.heroSelection(temp_holdName));
                bu.transform.FindChild("HeroName").GetComponent<Text>().text = hero.GetComponent<PlayerController>().entityName;
            }

        }
    }
    // Select Hero
    // Parameters:
    //  -_heroName  - String value of the hero name. Example: Constantine
    //  - heroAvailable - Bool, true if he can select the hero, false if hero select is not possible. (Account for later when hero selecting will be limited. Might be deprecated since i might just populate the takenHero array isntead)
    void heroSelection(string _heroName = ""/*, bool heroAvailable = true*/)
    {
        foreach (GameObject hero in HeroInformation.instance.allHeroes)
        {
            if (hero.GetComponent<PlayerController>().entityName == _heroName)
            {
                //Add picture or Animated prefab
                // CURRENTLY NOT WORKING DUE TO NO CORRECT PREFABS HUE HUE >X
                selectedHeroPortrait.GetComponent<Image>().sprite = hero.GetComponent<PlayerController>().entityDescriptionImage;

                //Clear parent GameObject
                destroyAllChildGameObjects(selectedHeroPortrait);


                /*  Testing
                foreach (GameObject key in HeroInformation.instance.heroSelectionPrefabs_heroes)
                {
                    if (key.name == _heroName)
                    {
                        GameObject heroPortrait = Instantiate(key, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                        heroPortrait.transform.SetParent(selectedHeroPortrait.transform);
                        heroPortrait.GetComponent<Transform>().localPosition = new Vector3(0, -20, 0);
                    }
                }
                */

                //Add Information Text
                selectedHeroInformation.GetComponent<Text>().text = hero.GetComponent<PlayerController>().entityInformation;
                //Add Hero Name
                selectedHeroName.GetComponent<Text>().text = hero.GetComponent<PlayerController>().entityName;
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
                if (tm["h"].ToString() != "")
                {
                    takenHeroes.Add(tm["h"].ToString());
                }
            }
        }
        string tmpSelectedHeroCode = HeroInformation.instance.return_HeroCode_OnName(selectedHeroName.GetComponent<Text>().text);
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
                // CHeck if proper exists, prevents error // Gotta fix this proper at some point
                // this fucking code. Fyck uy. NIGGER ASS CODE. Cant do it in the same loop it recognizes still 2 childreen.
                if (roomUIClassHolders[pos].transform.GetChild(0).name == "SelectedHeroesText(Clone)")
                {
                    setPlayerHeroSelection_createChild(curClient.isLocal, pos);
                }
                else
                {
                    if (playerCusProp["h"].ToString() != "")
                    {
                        string tempHeroCode = playerCusProp["h"].ToString();
                        // Check if correct child exists to populate information, otherwise create it.
                        //Debug.Log(roomUIClassHolders[pos].transform.GetChildCount().ToString());
                        roomUIClassHolders[pos].transform.GetChild(0).transform.FindChild("Text").GetComponent<Text>().text = HeroInformation.instance.return_HeroName_OnCode(tempHeroCode);
                        // Populate image // HeroInformation Should be made to return a single hero depending on some unique attribute.
                        foreach (GameObject hero in HeroInformation.instance.allHeroes)
                        {
                            if (hero.GetComponent<PlayerController>().heroCode == tempHeroCode)
                            {
                                roomUIClassHolders[pos].transform.GetChild(0).transform.FindChild("Image").GetComponent<Image>().sprite = hero.GetComponent<PlayerController>().entityDescriptionImage;
                                roomUIClassHolders[pos].transform.GetChild(0).transform.FindChild("Image").GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);
                            }
                        }
                    }
                    else
                    {
                        roomUIClassHolders[pos].transform.GetChild(0).transform.FindChild("Text").GetComponent<Text>().text = "Select your Hero !";
                    }
                }
            }
            else
            {
                //Checks if proper child exists, prevents errors
                if (roomUIClassHolders[pos].transform.GetChild(0).name != "SelectedHeroesText(Clone)")
                {
                    setPlayerHeroSelection_createChild(curClient.isLocal, pos);
                }
                else
                {
                    //
                    if (playerCusProp["h"].ToString() != "")
                    {
                        string tempHeroCode = playerCusProp["h"].ToString();
                        roomUIClassHolders[pos].transform.GetChild(0).GetComponent<Text>().text = HeroInformation.instance.return_HeroName_OnCode(tempHeroCode);
                        foreach (GameObject hero in HeroInformation.instance.allHeroes)
                        {
                            if (hero.GetComponent<PlayerController>().heroCode == tempHeroCode)
                            {
                                roomUIClassHolders[pos].transform.GetChild(0).transform.FindChild("Image").GetComponent<Image>().sprite = hero.GetComponent<PlayerController>().entityDescriptionImage;
                                roomUIClassHolders[pos].transform.GetChild(0).transform.FindChild("Image").GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);
                            }
                        }
                    }
                    else
                    {
                        roomUIClassHolders[pos].transform.GetChild(0).GetComponent<Text>().text = "";
                    }
                }
            }
        }
        else if (roomUIClassHolders[pos].transform.childCount == 0)
        {
            setPlayerHeroSelection_createChild(curClient.isLocal, pos);
        }
        else 
        {
            errorDisplay_open("Too many GameObjects in " + roomUIClassHolders[pos].name, "0003");
        }
    }
    // Functions for creating correct child.
    public void setPlayerHeroSelection_createChild(bool isLocal, int pos)
    {
        // Delete child if exists
        destroyAllChildGameObjects(roomUIClassHolders[pos]);
        // Repopulate with currect child
        if (isLocal)
        {
            GameObject sht = Instantiate(selectHeroButton, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            sht.transform.SetParent(roomUIClassHolders[pos].transform);
            sht.transform.localScale = Vector3.one;
            sht.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            sht.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            sht.GetComponent<Button>().onClick.AddListener(() => openHeroSelectionPanel());


            //Debug.Log(roomUIClassHolders[pos].transform.GetChild(0).name + " / " + roomUIClassHolders[pos].transform.GetChildCount().ToString());
        }
        //
        else 
        {
            GameObject sht = Instantiate(selectHeroText, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            sht.transform.SetParent(roomUIClassHolders[pos].transform);
            sht.transform.localScale = Vector3.one;
            sht.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            sht.GetComponent<RectTransform>().anchoredPosition = new Vector2(87, 0);
            sht.GetComponent<Text>().text = "";
        }
    }
    //
    //////////////////////////////////////// - RoundMatch GameMode RPC calls for Rounds and End check - ////////////////////////////////
    [PunRPC] public void GameMode_RoundMatch_RoundEnd()
    {
        StartCoroutine(GameMode_RoundMatch_PrepareToSpawn());
    }
    // Prepare to spawn your local Client Hero in a time interval and do the needed 
    IEnumerator GameMode_RoundMatch_PrepareToSpawn()
    {
        //Set KeyBinds
        InputKeys.instance.InputType = "MainMenu";
        //
        changeActiveStatus(UI_GameUI_ScoreBoard, true);
        int x = 0;
        while (x<2)
        {
            //Debug.Log("Loop " + x.ToString());
            if (x == 1)
            {
                Debug.Log("Starting new round!");
                InputKeys.instance.InputType = "Game";
                changeActiveStatus(UI_GameUI_ScoreBoard, false);
                spawnPlayerHero("",true);
            }
            x++;
            yield return new WaitForSeconds(3f);
        }
    }
    [PunRPC] public void GameMode_RoundMatch_GameEnd()
    {
        StartCoroutine(GameMode_RoundMatch_PrepareToEndGame());
    }
    // Ends the game after 5 sec, shows end screen
    IEnumerator GameMode_RoundMatch_PrepareToEndGame()
    {
        //Set KeyBinds
        InputKeys.instance.InputType = "MainMenu";
        //
        changeActiveStatus(UI_GameUI_ScoreBoard, true);
        // Set Ending screen here

        //
        int x = 0;
        while (x < 2)
        {
            if (x == 1)
            {
                endGame_client();
            }
            x++;
            yield return new WaitForSeconds(5f);
        }
    }
    //////////////////////////////////////// - END - ////////////////////////////////
    //
    IEnumerator UpdateGameScreen()
    {
        while (this.gameStatus == "running")
        {
            // Add all GameObjects with Player to activePlayerHeroes array
            this.activePlayerHeroes = GameObject.FindGameObjectsWithTag(this.tag_Player);
            //Displaying PlayerIconTop
            if (UI_GameUI_Top.transform.childCount > 0)
            {
                int x = 0;
                int aliveCount = 0;
                foreach (PhotonPlayer pl in PhotonNetwork.playerList)
                {
                    // Future problem with displaying content in the ones that have left the game.
                    ExitGames.Client.Photon.Hashtable plInfo = new ExitGames.Client.Photon.Hashtable();
                    plInfo = pl.customProperties;
                    // UI ScoreBoard Loading
                    Transform temp_info = UI_GameUI_Top.transform.GetChild(x).FindChild("Info");
                    temp_info.transform.FindChild("PlayerName").GetComponent<Text>().text = pl.name + " / ID: " + pl.ID;
                    //
                    temp_info.transform.FindChild("HealthBar").GetComponent<Text>().text = plInfo["hp"].ToString() + " / " + plInfo["mhp"].ToString();
                    //
                    RectTransform temp_rectT = temp_info.transform.FindChild("HealthBarRed").transform.FindChild("HealthBarGreen").GetComponent<RectTransform>();
                    temp_rectT.anchorMax = new Vector2(0, 1);
                    temp_rectT.anchorMin = new Vector2(0, 0);
                    temp_rectT.anchoredPosition = new Vector2(0f, 0.5f);
                    /*
                    float maxHP = Convert.ToSingle(plInfo["mhp"].ToString());
                    float HP = Convert.ToSingle(plInfo["hp"].ToString());
                    float hpProcentDec = (HP / maxHP) * 100;
                    int hpProcent = (int)Math.Round(hpProcentDec, 0);
                    int changeValue =(int)Math.Round((hpProcent * 1.5f),0);
                    Debug.Log(maxHP.ToString() + "/" + HP.ToString() + "/" + hpProcentDec.ToString() + "/" + hpProcent.ToString());
                    temp_rectT.sizeDelta = new Vector2(changeValue, 0);
                    temp_rectT.localPosition = new Vector3(((changeValue/2f)-75),0,0);
                    */
                    int temp_value = (int)Math.Round((((int)Math.Round((Convert.ToSingle(plInfo["hp"].ToString()) / Convert.ToSingle(plInfo["mhp"].ToString())) * 100, 0)) * 1.5f), 0);
                    temp_rectT.sizeDelta = new Vector2(temp_value, 0);
                    temp_rectT.localPosition = new Vector3(((temp_value / 2f) - 75), 0, 0);

                    //
                    UI_GameUI_ScoreBoard_Score.transform.GetChild(x).FindChild("Name").GetComponent<Text>().text = pl.name;
                    UI_GameUI_ScoreBoard_Score.transform.GetChild(x).FindChild("Hero").GetComponent<Text>().text = HeroInformation.instance.return_HeroName_OnCode(plInfo["h"].ToString());
                    UI_GameUI_ScoreBoard_Score.transform.GetChild(x).FindChild("Kills").GetComponent<Text>().text = plInfo["k"].ToString();
                    UI_GameUI_ScoreBoard_Score.transform.GetChild(x).FindChild("Deaths").GetComponent<Text>().text = plInfo["d"].ToString();
                    // GameMode Checking
                    if (plInfo["hs"].ToString() == "a")
                    {
                        aliveCount++;
                    }
                    //
                    x++;
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }
    //
    IEnumerator UpdateGameLobby ()
    {
        while(this.gameStatus == "roomLobby")
        {
            //Debug.Log("roomLobby");
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
                //ExitGames.Client.Photon.Hashtable playerProp = diCk[key].allProperties;
                setPlayerHeroSelection(i, diCk[key]);
                // Visual difference for you and other players
                setDifferentDisplayForPlayers(i, diCk[key]);
                
                //if(diCk[key].isLocal) 

                i++;
            }
            // Cleans class selections
            if (i < maxPlayerPossible) // ?
            {
                for (int k = i; k < maxPlayerPossible; k++)
                {
                    destroyAllChildGameObjects(this.roomUIClassHolders[k]);
                    //Sets default color

                }
            }
            //
            setMasterOptionsForRoom();
            //Update once per second
            yield return new WaitForSeconds(1f);
        }
    }
    //
    public void setDifferentDisplayForPlayers(int playerIndex, PhotonPlayer player)
    {
        if (player.isLocal)
        {
            roomUINames[playerIndex].transform.parent.GetComponent<Image>().color = this.bMain;
        }
        else
        {
            roomUINames[playerIndex].transform.parent.GetComponent<Image>().color = this.rRegBack;
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
        // This polymorphism allows the second parameter to be unset and then does the automatic thing.
        if (force == "")
        {
            if (targetObj.activeSelf == true) targetObj.SetActive(false);
            else targetObj.SetActive(true);
        }
        else if (force == "open") targetObj.SetActive(true);
        else if(force == "close") targetObj.SetActive(false);
    }
    public void changeActiveStatus(GameObject targetObj, bool force)
    {
        if (force == true) targetObj.SetActive(true);
        else if (force == false) targetObj.SetActive(false);
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
            // hides or shows kick buttons
            for (int x = 0; x < roomUIKickButtons.Length; x++)
            {
                if (roomUINames[x].GetComponent<Text>().text != "") roomUIKickButtons[x].SetActive(PhotonNetwork.isMasterClient);
                else roomUIKickButtons[x].SetActive(false);
                //roomUIKickButtons[x].SetActive(PhotonNetwork.isMasterClient);
            }
            // If master display start button || if not hides
            if(PhotonNetwork.isMasterClient) changeActiveStatus(roomLobbyStartButton, "open");
            else changeActiveStatus(roomLobbyStartButton, "close");
        }
        
    }
    // Add to room console
    public void addToRoomConsole(string textToAdd, bool newLine = true)
    {
        if (gameStatus == "roomLobby")
        {
            string newLineSet = "\n";
            if (!newLine) newLineSet = "";
            if (GameController.instance.GameStatus == "roomLobby")
                GameController.instance.roomLobbyConsole.GetComponent<Text>().text += newLineSet + textToAdd;
            else errorDisplay_open("Something is trying to be display in the Console of an open room, however the room is not opened !", "0002");
        }
    }
    //Display ExtraRoomUI
    // Parameter is a string value that invokes a method inside the GameController.instance Its optional if no string value it hides all
    public void displayExtraRoom(string selectedExtra = "")
    {
        bool windowStatus = true;
        // Hide All
        foreach (GameObject key in extraOptionsUI)
        {
            if (key.GetActive() == true && selectedExtra+"Wrap" == key.name)
            {
                windowStatus = false;
            }

            changeActiveStatus(key, false);
        }
        // Show correct one.
        if (windowStatus && selectedExtra != "")
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
        // THis will pull all information to display the saved options
        OptionController.instance.displayOptions();

    }
    public void extraRoom_closeOptions()
    {
        foreach (GameObject key in extraOptionsUI)
        {
            if (key.name == "OptionsWrap") changeActiveStatus(key, false);
        }
    }
    public void extraRoom_openJoin()
    {
        foreach (GameObject key in extraOptionsUI)
        {
            if (key.name == "JoinWrap") changeActiveStatus(key, "open");
        }

        // What to do on open list of rooms ? List teh rooms ane. baaak.
        //this.GetComponent<Network>().refreshPhotonRooms(); // Doesnt work, didnt debug yet.
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
    public void extraRoom_closeCreate()
    {
        foreach (GameObject key in extraOptionsUI)
        {
            if (key.name == "CreateWrap") changeActiveStatus(key, false);
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
    //
    public void startGame_host()
    {
        bool continueLoad = true;
        string errorMsg = "";
        //Checks if room still exists
        if(PhotonNetwork.room == null)
        {
            errorMsg += "ERROR: Room does not exist anymore. \nCreate a new room or join another !\n";
            continueLoad = false;

        }
        // Verifies needed data if 4 players ^^ if all players have set heroes
        if(continueLoad)
        {
            List<string> selHeroes = new List<string>();
            foreach (PhotonPlayer pl in PhotonNetwork.playerList)
            {
                if (!continueLoad) break;
                ExitGames.Client.Photon.Hashtable ch = new ExitGames.Client.Photon.Hashtable();
                ch = pl.customProperties;
                if (ch["h"].ToString() == "")
                {
                    errorMsg += "All players must choose a hero ! \n";
                    continueLoad = false;
                }
                else
                {
                    selHeroes.Add(ch["h"].ToString());
                }
            }
            // check for if characters are same
            if (selHeroes.Distinct().Count() != selHeroes.Count())
            {
                errorMsg += "ERROR: 2 players are more have selected the same hero. \n\nCreate a new room ! \n";
                continueLoad = false;
            }
        }
        /* Disabled for testing purposes.
        if (continueLoad)
        {
            if (PhotonNetwork.room.playerCount != PhotonNetwork.room.maxPlayers)
            {
                errorMsg += "The room needs " + PhotonNetwork.room.maxPlayers.ToString() + " players !\n";
                continueLoad = false;
            }
        }
        */
        //
        if (continueLoad)
        {
            // Load special room properties
            ExitGames.Client.Photon.Hashtable newSkyBox = new ExitGames.Client.Photon.Hashtable();
            newSkyBox.Add("sky",selectSkyBox_randomReturn().ToString());
            PhotonNetwork.room.SetCustomProperties(newSkyBox);
            //Disable joining into room and make it invisible
            PhotonNetwork.room.open = false;
            PhotonNetwork.room.visible = false;
            // Must send RPC call to all players.
            photonView.RPC("startGame_client", PhotonTargets.All);
            //startGame_client();
        }
        else
        {
            // Game cannot continue
            errorDisplay_open(errorMsg);
        }
    }
    //
    // Main function to go from idle game in main menu and lobbies to an active game with evetything set up.
    //
    [PunRPC] public void startGame_client()
    { 
        // All the proccesses here should be synched and displayed only when all players have loaded their side.
        /////////////////////////
        // Get player settings
        ExitGames.Client.Photon.Hashtable plInfo = new ExitGames.Client.Photon.Hashtable();
        plInfo = PhotonNetwork.player.customProperties;
        // Hide the UI
        changeActiveStatus(this.UI_mainMenu, false);
        displayExtraRoom();
        // Clean room UI
        this.cleanRoomLobby();
        changeActiveStatus(GameController.instance.roomLobby, false);
        // Sets InputKeys
        InputKeys.instance.InputType = "Game";
        // Hide unnecessary buttons and show the neccesarry ones
        changeActiveStatus(UI_MainMenuUI_MainMenuWrap_CreateRoom, false);
        changeActiveStatus(UI_MainMenuUI_MainMenuWrap_QuickJoin, false);
        changeActiveStatus(UI_MainMenuUI_MainMenuWrap_JoinRoom, false);
        changeActiveStatus(UI_MainMenuUI_MainMenuWrap_Credits, false);
        //changeActiveStatus(UI_MainMenuUI_MainMenuWrap_InputField, false);
        changeActiveStatus(UI_MainMenuUI_MainMenuWrap_ReturnToGame, true);
        changeActiveStatus(UI_MainMenuUI_MainMenuWrap_ReturnToMainMenu, true);
        // Display game UI
        changeActiveStatus(this.UI_game, true);
        // Stopping the lobby loop with changin gameStatus
        this.GameStatus = "running";
        // Loads the GameMode // sets default or room customRoomProperties photon
        ExitGames.Client.Photon.Hashtable roomCusInfo = PhotonNetwork.room.customProperties;
        GameMode.ScoreCondition = Convert.ToInt32(roomCusInfo["sc"]);
        GameMode.Mode = roomCusInfo["gm"].ToString();
        GameMode.PlayerCount = PhotonNetwork.room.playerCount;
        GameMode.map = this.mapsFolder[1];
        // Load the map // Presumes files have not been tempered with
        GameMode.instantiatedMap = Instantiate(GameMode.map, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        // Initialise the spawn points // Searches for all GameObject that have the tag_MapSpawn (Set in Editor)
        spawnPositions = new List<Vector3>();
        foreach (GameObject spawn in GameObject.FindGameObjectsWithTag(this.tag_MapSpawn))
        {
            spawnPositions.Add(spawn.GetComponent<Transform>().position);
        }
        // Load the player prefab
        spawnPlayerHero(HeroInformation.instance.return_HeroName_OnCode(plInfo["h"].ToString()));
        // Load the UI // Might change this to load the number of listed players and not the players that actually exists
        gameUI_loadElements();
        // Load the GameMode Description
        UI_GameUI_ScoreBoard_GameModeDescription.GetComponent<Text>().text = GameMode.ModeDescription;
        // Load the skybox - Right now it will be random.
        //UnityEngine.Random.Range(0, skyBox.Length);
        selectSkyBox(Convert.ToInt32(PhotonNetwork.room.customProperties["sky"]));
        Debug.Log(Convert.ToInt32(PhotonNetwork.room.customProperties["sky"]));
        //
        // Sets the cursor to border Lock
        MouseCursorController.instance.BorderLockState = true;
    }
    //
    // Main function for returning from active game to idle main menu
    //
    public void endGame_client()
    {
        if (PhotonNetwork.LeaveRoom())
        {
            setEnvironment_mainMenu();
            //Destroy all unnecessary GameObjects - Map, Players, etc...
            destroyAllChildGameObjects(UI_GameUI_Top);
            Destroy(GameMode.instantiatedMap.gameObject);
            //
            destroyAllChildGameObjects(UI_GameUI_ScoreBoard_Score);
            //
            MouseCursorController.instance.BorderLockState = false;
        }
        else
        {
            //Crit Error i need a way to do this.
            errorDisplay_open("ERROR while leaving the photon room.");
        }
    }
    // An error with spawning i believe
    private Vector3 GetRandomSpawnPoint()
    {
        return spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Count - 1)];
    }
    
    IEnumerator DisplayRoundText()
    {
        // Testing
        GameObject countdownObject = new GameObject();
        countdownObject.AddComponent<GUIText>();

        GUIText countdownText = countdownObject.GetComponent<GUIText>();
        Font pyrite = (Font)Resources.Load("Fonts/pyrite");
        countdownText.font = pyrite;
        countdownText.text = "FIGHT";
        countdownText.fontSize = 220;
        countdownText.color = new Color(0.7f, 0.25f, 0.25f);
        countdownText.alignment = TextAlignment.Center;
        countdownText.anchor = TextAnchor.MiddleCenter;

        countdownObject.transform.position = new Vector3(0.5f, 0.5f, 0f);

        AudioClip fight = new AudioClip();
        fight = (AudioClip)Resources.Load<AudioClip>("Sounds/fight");

        AudioSource.PlayClipAtPoint(fight, new Vector3(0f, 0f, 0f));

        while (countdownText.color.a > 0f)
        {
            yield return new WaitForSeconds(0.15f);

            countdownText.color = new Color(countdownText.color.r, countdownText.color.g, countdownText.color.b, countdownText.color.a - 0.1f);
        }
        
    }
    ////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////
    // Load the Game UI elements for running game
    public void gameUI_loadElements()
    {
        //Destroy childreen if any.
        destroyAllChildGameObjects(UI_GameUI_Top);
        destroyAllChildGameObjects(UI_GameUI_ScoreBoard_Score);
        // Load the needed ui for TOP and ScoreBoard, relation to players
        //StartCoroutine("DisplayRoundText");
        foreach (PhotonPlayer pl in PhotonNetwork.playerList)
        {
            if (pl != null)
            {
                GameObject temp_PlayerIconTop = Instantiate(this.gamePlayerIcon, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                temp_PlayerIconTop.transform.SetParent(UI_GameUI_Top.transform);
                GameObject temp_scorePlayer = Instantiate(this.score_PlayerWrap, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                temp_scorePlayer.transform.SetParent(UI_GameUI_ScoreBoard_Score.transform);
            }
        }
        // Load the ability names
        foreach (GameObject hr in HeroInformation.instance.allHeroes)
        {
            if (hr.GetComponent<PlayerController>().heroCode == PhotonNetwork.player.customProperties["h"].ToString())
            {
                UI_GameUI_Bottom_Center_FirstAbilityName.GetComponent<Text>().text = hr.GetComponent<PlayerController>().Ability.gameObject.transform.GetComponent<Ability>().visualName;
                UI_GameUI_Bottom_Center_SecondAbilityName.GetComponent<Text>().text = hr.GetComponent<PlayerController>().Ability2.gameObject.transform.GetComponent<Ability>().visualName;
            }
        }
    }
    // Switching main camera point of view
    public void rotateDeathCamera()
    {
        // Find the target
        int activeCount = activePlayerHeroes.Length;
        if (activeCount != 0)
        {
            int getActiveCameraTarget = 0;
            //Debug.Log(getActiveCameraTarget);
            int x = 1;
            foreach (GameObject pl in activePlayerHeroes)
            {
                if (mainCamera.GetComponent<SmoothCameraFollow>().targetHero == pl)
                {
                    getActiveCameraTarget = x;
                    Debug.Log("PrimaryFocus: " + getActiveCameraTarget.ToString());
                }
                x++;
            }
            if (getActiveCameraTarget == activeCount)
                getActiveCameraTarget = 1;
            else getActiveCameraTarget++;

            Debug.Log("NewFocus: " + getActiveCameraTarget.ToString());
            //
            x = 1;
            foreach (GameObject pl in activePlayerHeroes)
            {
                if (getActiveCameraTarget == x)
                {
                    mainCamera.GetComponent<SmoothCameraFollow>().targetHero = pl;
                }
                x++;
            }
            //
        }
        else 
        {
            this.errorDisplay_open("ActiveCount of players: " + activeCount.ToString());
        }

        //Debug.Log(getActiveCameraTarget);
        // 
        //mainCamera.GetComponent<SmoothCameraFollow>().target;
        
        
    }
    // Spawn the player hero
    public void spawnPlayerHero(string playerHeroName = "", bool forceSpawn = false)
    {
        // Check if forceSpawn is true, deletes the old spawn first
        if (forceSpawn)
        {
            if (activeLocalHero != null)
            {
                PhotonNetwork.Destroy(activeLocalHero.gameObject);
                Destroy(activeLocalHero.gameObject);
            }
        }
        // Set playerHeroStatus to alive /a
        //Debug.Log("You have launched the spawn method!");
        ExitGames.Client.Photon.Hashtable chInfo = new ExitGames.Client.Photon.Hashtable();
        chInfo = PhotonNetwork.player.customProperties;
        //Debug.Log("HeroStatus: " + chInfo["hs"].ToString());
        if (chInfo["hs"].ToString() != "a" || forceSpawn == true)
        {
            //Debug.Log("You The character is indeed dead !");
            //
            chInfo["hs"] = "a";
            PhotonNetwork.player.SetCustomProperties(chInfo);
            // Set keyInput
            InputKeys.instance.InputType = "Game";
            // Check if parameter playerHeroName is set if not, get hero name value for resource load
            if (playerHeroName == "")
            {
                ExitGames.Client.Photon.Hashtable plInfo = PhotonNetwork.player.customProperties;
                playerHeroName = HeroInformation.instance.return_HeroName_OnCode(plInfo["h"].ToString());
            }
            // Create player on location and set camera
            this.activeLocalHero = PhotonNetwork.Instantiate("Heroes/"+playerHeroName, GetRandomSpawnPoint(), Quaternion.identity, 0);
            mainCamera.GetComponent<SmoothCameraFollow>().targetHero = this.activeLocalHero;
        }
        //else Debug.Log("No spawn Hero Alive");
    }
    // Kill the player hero resource and Check if GameEnd is true, else continue with GameMode Design for spawning
    public void destroyPlayerHero()
    {

        ExitGames.Client.Photon.Hashtable chInfo = new ExitGames.Client.Photon.Hashtable();
        chInfo.Add("hs", "d");
        PhotonNetwork.player.SetCustomProperties(chInfo);
        //
        InputKeys.instance.InputType = "GameDead";
        //
        PhotonNetwork.Destroy(this.activeLocalHero.gameObject);
        Destroy(this.activeLocalHero.gameObject);
        // Depending on the GameMode this will be changed Spawning or well staying dead
        // Before checking for GameMode and what to do, check if game end condition has been met ?
        // GameMode_RoundMatch_GameEnd()
        bool temp_endConditionMet = false;
        foreach(PhotonPlayer pl in PhotonNetwork.playerList)
        {
            // Possible error with DRAW depending since it checks on player death and not with adding kill straight away
            if (GameMode.Mode == "RoundMatch")
            {
                ExitGames.Client.Photon.Hashtable sc = pl.customProperties;
                if (Convert.ToInt32(sc["k"]) >= GameMode.ScoreCondition) temp_endConditionMet = true;
            }
            if (GameMode.Mode == "CaptureTheFlag")
            {
                // Just in case we might need something else than scoreCondition for kills.
            }
        }
        if (temp_endConditionMet)
        {
            this.photonView.RPC("GameMode_RoundMatch_GameEnd", PhotonTargets.All);
        }
        else
        {
            if (GameMode.Mode == "RoundMatch")
            {
                ExitGames.Client.Photon.Hashtable roomCusInfo = PhotonNetwork.room.customProperties;
                int x = Convert.ToInt32(roomCusInfo["rk"].ToString());
                x++;
                if ((GameMode.PlayerCount - 1) <= x)
                {
                    // Start new Round
                    Debug.Log("New round has started");
                    // Call RPC
                    this.photonView.RPC("GameMode_RoundMatch_RoundEnd", PhotonTargets.All);
                    // Set Round deaths to 0
                    roomCusInfo["rk"] = "0";
                }
                else
                {
                    // Add the round kill to room properties
                    roomCusInfo["rk"] = x.ToString();
                }
                // Transfer only one parameter
                ExitGames.Client.Photon.Hashtable sInfoToTransfer = new ExitGames.Client.Photon.Hashtable();
                sInfoToTransfer.Add("rk", roomCusInfo["rk"].ToString());
                PhotonNetwork.room.SetCustomProperties(sInfoToTransfer);
            }
        }
    }
    // Close Main Menu and return to game
    public void returnToGame_fromMainMenu()
    {
        changeActiveStatus(GameController.instance.UI_mainMenu, false);
        InputKeys.instance.InputType = "Game";
    }
    //
    public void setHpValues_toPlayerCustomProp(int currentHP, int maxHP = 0)
    {
        if (currentHP < 0) currentHP = 0;
        ExitGames.Client.Photon.Hashtable newHpVal = new ExitGames.Client.Photon.Hashtable();
        newHpVal.Add("hp", currentHP.ToString());
        if(maxHP != 0 ) newHpVal.Add("mhp", maxHP.ToString());
        PhotonNetwork.player.SetCustomProperties(newHpVal);
    }
    // Add KILL point to local client
    public void addKillPoint(int playerId)
    {
        ExitGames.Client.Photon.Hashtable customPropertiesTable = new ExitGames.Client.Photon.Hashtable();

        foreach(PhotonPlayer networkPlayer in PhotonNetwork.otherPlayers)
        {
            if (networkPlayer.ID == playerId)
            {
                // Current players ID matches with the ID of the client who killed another client

                customPropertiesTable = networkPlayer.customProperties;

                // "k" = Ernils way of saying "amount of kills" // Leave Ernils way out of this :D!
                customPropertiesTable["k"] = (Convert.ToInt32(customPropertiesTable["k"]) + 1).ToString();
                networkPlayer.SetCustomProperties(customPropertiesTable);
            }
        }
    }

    // Add DEATH point to local client
    public void addDeathPoint()
    {
        ExitGames.Client.Photon.Hashtable getKill = new ExitGames.Client.Photon.Hashtable();
        getKill = PhotonNetwork.player.customProperties;
        getKill["d"] = (Convert.ToInt32(getKill["d"]) + 1).ToString();
        PhotonNetwork.player.SetCustomProperties(getKill);
    }
    /// <summary>
    /// Sets the skybox for the mainCamera
    /// </summary>
    private void selectSkyBox()
    {
        mainCamera.GetComponent<Skybox>().material = skyBox[selectSkyBox_randomReturn()];
    }
    private void selectSkyBox(int selectedSky)
    {
        mainCamera.GetComponent<Skybox>().material = skyBox[selectedSky];
    }
    public int selectSkyBox_randomReturn()
    {
        return UnityEngine.Random.Range(0, skyBox.Length);
    }
    /// <summary>
    /// Removes the skybox from the mainCamera
    /// </summary>
    private void removeSkyBox()
    {
        mainCamera.GetComponent<Skybox>().material = null;
    }
    //Application quit
    public void gameQuit()
    {
        Application.Quit();
    }
    // Testing method linked to Testing Button
}
