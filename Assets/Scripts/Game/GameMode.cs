using UnityEngine;
using System.Collections;
using System;

public static class GameMode 
{
    //
    private static int playerCount;
    private static int scoreCondition = 15; // Each gamemode is limited by an objective. This is that objective.
    // Options for mode
    //  - RoundMatch - A player must get a certain amount of kills before the game ends, When 1 player survives each round the round restarts.
    private static string mode;
    private static string modeDescription;
    public static GameObject map;
    public static GameObject instantiatedMap;
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
            if (mode == "RoundMatch")
                ModeDescription = " Round based game \n- Each round is played until 1 player is left alive \n- Game ends when a player reaches the score " + scoreCondition.ToString();
        }
        get 
        {
            return mode;
        }
    }
    //
    public static int ScoreCondition
    {
        set
        {
            scoreCondition = value;
        }
        get
        {
            return scoreCondition;
        }
    }
    //
    public static string ModeDescription
    {
        set
        {
            modeDescription = value;
        }
        get
        {
            return modeDescription;
        }
    }
}
