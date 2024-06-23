using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProgressConfigManager
{
    public bool bridgeState = false;
    public PuzzleStatus puzzleStatusTracker = new PuzzleStatus();
    public Vector3 playerPosition;
    public Quaternion playerRotation;

    public void UpdateProgress(bool bridgeDown = false, List<PuzzlesStatesArray> puzzlesStates = null, Vector3 playerPos = new Vector3(), Quaternion playerRot = new Quaternion())
    {
        bridgeState = bridgeDown;
        puzzleStatusTracker.puzzlesStatesArrays = puzzlesStates;
        playerPosition = playerPos;
        playerRotation = playerRot;

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

