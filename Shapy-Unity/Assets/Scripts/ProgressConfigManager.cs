using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProgressConfigManager
{
    public bool bridgeState;
    public bool cableCar1State; 
    public bool cableCar2State;
    public bool boatState;
    public bool kitState;
    public PuzzleStatus puzzleStatusTracker = new PuzzleStatus();
    public Vector3 playerPosition;
    public Quaternion playerRotation;

    public void UpdateKitStatus(bool kitActivated)
    {
        kitState = kitActivated;
        GenerateJSONString();
    }
    public void UpdateProgress(bool bridgeDown = false, bool cablecar1 = false, bool cablecar2 = false, bool boat = false, List<PuzzlesStatesArray> puzzlesStates = null, Vector3 playerPos = new Vector3(), Quaternion playerRot = new Quaternion())
    {
        bridgeState = bridgeDown;
        puzzleStatusTracker.puzzlesStatesArrays = puzzlesStates;
        playerPosition = playerPos;
        playerRotation = playerRot;
        cableCar1State = cablecar1;
        cableCar2State = cablecar2;
        boatState = boat;

        GenerateJSONString();
    }

    public void GenerateJSONString()
    {
        string jsonString = JsonUtility.ToJson(this, true);
        ConfigSaveLoadSystem.Save(jsonString);
    }


}





[Serializable]
public class PuzzleStatus
{
   public List<PuzzlesStatesArray> puzzlesStatesArrays = new List<PuzzlesStatesArray>();
}

