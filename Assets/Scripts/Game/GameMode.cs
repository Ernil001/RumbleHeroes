//using UnityEngine;
using System.Collections;
using System;

public static class GameMode 
{
    private static int playerCount;
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
    private static string mode = "";
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
}
