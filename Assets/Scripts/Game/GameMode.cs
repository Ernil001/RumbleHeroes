using UnityEngine;
using System.Collections;
using System;

public static class GameMode 
{
    //
    private static int playerCount;
    private static int winKillCondition;
    private static string mode;
    public static GameObject map;
    //
    public static int PlayerCount
    {
        set
        {
            playerCount = value;
        }
        get
        {
            return playerCount;
        }
    }
    //
    public static string Mode 
    {
        set
        {
            mode = value;
        }
        get 
        {
            return mode;
        }
    }
    //
    public static int WinKillCondition
    {
        set
        {
            winKillCondition = value;
        }
        get
        {
            return winKillCondition;
        }
    }
    //
}
