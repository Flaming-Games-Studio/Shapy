using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsMethods : MonoBehaviour
{
    GlobalTimer timer;
    private void Start()
    {
        timer = new GlobalTimer();
        timer.GetCurrentTimestamp(out timer.startTime);
    }

    [System.Serializable]
    public class StringListWrapper
    {
        public List<string> stringList;
        public StringListWrapper(List<string> list)
        {
            stringList = list;
        }
    }

    #region Switches
    public void SwitchesPuzzleFinished(bool completed)
    {
        AnalyticsService.Instance.CustomData("SwitchesPuzzlePlayed", new Dictionary<string, object>
        {
            { "playedPuzzle", completed },
        });
    }
    public void SwitchesPuzzleTimesAndOrder(string winCombo, string totalTimeInPuzzle, List<DateTime> realTimeOfInteractions, List<string> listOrderOfInteraction, List<string> listTimeOfInteraction)
    {
        //splitting string to remove number infront
        string[] partsOfString = totalTimeInPuzzle.Split('.');
        string totalTimeSpent = partsOfString[1];
        //creating list and setting first entry as null value
        List<string> timeBetweenInteractions = new();
        // calculating time difference between interactions and saving them to list
        for (int i = 0; i < realTimeOfInteractions.Count; i++)
        {
            if (i == 0)
            {
                timeBetweenInteractions.Add("0:0:0:0");
            }
            else
            {
                TimeSpan diff = realTimeOfInteractions[i] - realTimeOfInteractions[i - 1];
                string lastPress = diff.Hours + ":" + diff.Minutes + ":" + diff.Seconds + ":" + diff.Milliseconds;
                timeBetweenInteractions.Add(lastPress);
            }
        }
        //converting stringlists to jsonstring format
        string jsonStringTimeBetweenInteractions = JsonUtility.ToJson(new StringListWrapper(timeBetweenInteractions));
        string jsonStringOrderOfInteraction = JsonUtility.ToJson(new StringListWrapper(listOrderOfInteraction));
        string jsonStringTimeOfInteraction = JsonUtility.ToJson(new StringListWrapper(listTimeOfInteraction));
        //sending event to unity analytics
        AnalyticsService.Instance.CustomData("SwitchesPuzzleTimes", new Dictionary<string, object>
        {
            { "winningCombination", winCombo },
            { "totalTimeSpentUntilComplete", totalTimeSpent },
            { "timeBetweenInteractions", jsonStringTimeBetweenInteractions },
            { "orderOfInteraction", jsonStringOrderOfInteraction },
            { "timeOfInteraction", jsonStringTimeOfInteraction },
        });
    }
    #endregion

    #region Wires
    public void WiresPuzzleFinished(bool completed)
    {
        AnalyticsService.Instance.CustomData("WiresPuzzlePlayed", new Dictionary<string, object>
        {
            { "playedPuzzle", completed },
        });
    }
    public void WiresPuzzleTimesAndOrder(int correctMoves,int incorrectMoves,string totalTimeInPuzzle, List<DateTime> realTimeOfInteractions,List<string> listTimeOfInteraction)
    {
        //splitting string to remove number infront
        string[] partsOfString = totalTimeInPuzzle.Split('.');
        string totalTimeSpent = partsOfString[1];
        //creating list and setting first entry as null value
        List<string> timeBetweenInteractions = new();
        // calculating time difference between interactions and saving them to list
        for (int i = 0; i < realTimeOfInteractions.Count; i++)
        {
            if (i == 0)
            {
                timeBetweenInteractions.Add("0:0:0:0");
            }
            else
            {
                TimeSpan diff = realTimeOfInteractions[i] - realTimeOfInteractions[i - 1];
                string lastPress = diff.Hours + ":" + diff.Minutes + ":" + diff.Seconds + ":" + diff.Milliseconds;
                timeBetweenInteractions.Add(lastPress);
            }
        }
        //converting stringlists to jsonstring format
        string jsonStringTimeBetweenInteractions = JsonUtility.ToJson(new StringListWrapper(timeBetweenInteractions));
        string jsonStringTimeOfInteraction = JsonUtility.ToJson(new StringListWrapper(listTimeOfInteraction));
        //sending event to unity analytics
        AnalyticsService.Instance.CustomData("WiresPuzzleTimes", new Dictionary<string, object>
        {
            { "correctMoves", correctMoves},
            { "incorrectMoves", incorrectMoves },
            { "totalTimeSpentUntilComplete", totalTimeSpent },
            { "timeBetweenInteractions", jsonStringTimeBetweenInteractions },
            { "timeOfInteraction", jsonStringTimeOfInteraction },
        });
    }
    #endregion

    #region Gears
    public void GearsPuzzleFinished(bool completed)
    {
        AnalyticsService.Instance.CustomData("GearsPuzzlePlayed", new Dictionary<string, object>
        {
            { "playedPuzzle", completed },
        });
    }
    public void GearsPuzzleTimesAndOrder(string totalTimeInPuzzle, List<DateTime> realTimeOfInteractions, List<string> listOrderOfInteraction, List<string> listTimeOfInteraction)
    {
        //splitting string to remove number infront
        string[] partsOfString = totalTimeInPuzzle.Split('.');
        string totalTimeSpent = partsOfString[1];
        //creating list and setting first entry as null value
        List<string> timeBetweenInteractions = new();
        // calculating time difference between interactions and saving them to list
        for (int i = 0; i < realTimeOfInteractions.Count; i++)
        {
            if (i == 0)
            {
                timeBetweenInteractions.Add("0:0:0:0");
            }
            else
            {
                TimeSpan diff = realTimeOfInteractions[i] - realTimeOfInteractions[i - 1];
                string lastPress = diff.Hours + ":" + diff.Minutes + ":" + diff.Seconds + ":" + diff.Milliseconds;
                timeBetweenInteractions.Add(lastPress);
            }
        }
        //converting stringlists to jsonstring format
        string jsonStringTimeBetweenInteractions = JsonUtility.ToJson(new StringListWrapper(timeBetweenInteractions));
        string jsonStringOrderOfInteraction = JsonUtility.ToJson(new StringListWrapper(listOrderOfInteraction));
        string jsonStringTimeOfInteraction = JsonUtility.ToJson(new StringListWrapper(listTimeOfInteraction));
        //sending event to unity analytics
        AnalyticsService.Instance.CustomData("GearsPuzzleTimes", new Dictionary<string, object>
        {
            { "totalTimeSpentUntilComplete", totalTimeSpent },
            { "timeBetweenInteractions", jsonStringTimeBetweenInteractions },
            { "orderOfInteraction", jsonStringOrderOfInteraction },
            { "timeOfInteraction", jsonStringTimeOfInteraction },
        });
    }
    #endregion

    #region LeverPulling
    public void LeverPullingPuzzleFinished(bool completed)
    {
        AnalyticsService.Instance.CustomData("LeverPullingPuzzlePlayed", new Dictionary<string, object>
        {
            { "playedPuzzle", completed },
        });
    }
    public void LeverPullingPuzzleTimesAndOrder(string totalTimeInPuzzle, List<DateTime> realTimeOfInteractions, List<string> listOrderOfInteraction, List<string> listTimeOfInteraction)
    {
        //splitting string to remove number infront
        string[] partsOfString = totalTimeInPuzzle.Split('.');
        string totalTimeSpent = partsOfString[1];
        //creating list and setting first entry as null value
        List<string> timeBetweenInteractions = new();
        // calculating time difference between interactions and saving them to list
        for (int i = 0; i < realTimeOfInteractions.Count; i++)
        {
            if (i == 0)
            {
                timeBetweenInteractions.Add("0:0:0:0");
            }
            else
            {
                TimeSpan diff = realTimeOfInteractions[i] - realTimeOfInteractions[i - 1];
                string lastPress = diff.Hours + ":" + diff.Minutes + ":" + diff.Seconds + ":" + diff.Milliseconds;
                timeBetweenInteractions.Add(lastPress);
            }
        }
        //converting stringlists to jsonstring format
        string jsonStringTimeBetweenInteractions = JsonUtility.ToJson(new StringListWrapper(timeBetweenInteractions));
        string jsonStringOrderOfInteraction = JsonUtility.ToJson(new StringListWrapper(listOrderOfInteraction));
        string jsonStringTimeOfInteraction = JsonUtility.ToJson(new StringListWrapper(listTimeOfInteraction));
        //sending event to unity analytics
        AnalyticsService.Instance.CustomData("LeverPullingPuzzleTimes", new Dictionary<string, object>
        {
            { "totalTimeSpentUntilComplete", totalTimeSpent },
            { "timeBetweenInteractions", jsonStringTimeBetweenInteractions },
            { "orderOfInteraction", jsonStringOrderOfInteraction },
            { "timeOfInteraction", jsonStringTimeOfInteraction },
        });
    }
    #endregion

    #region ResourceDispatcher
    public void ResourceDispatcherPuzzleFinished(bool completed)
    {
        AnalyticsService.Instance.CustomData("ResourceDispatcherPuzzlePlayed", new Dictionary<string, object>
        {
            { "playedPuzzle", completed },
        });
    }
    public void ResourceDispatcherPuzzleTimesAndOrder(string totalTimeInPuzzle, List<string> batteryColumnsMax, List<DateTime> realTimeOfInteractions, List<string> listOrderOfInteraction, List<string> listTimeOfInteraction)
    {
        //splitting string to remove number infront
        string[] partsOfString = totalTimeInPuzzle.Split('.');
        string totalTimeSpent = partsOfString[1];
        //creating list and setting first entry as null value
        List<string> timeBetweenInteractions = new();
        // calculating time difference between interactions and saving them to list
        for (int i = 0; i < realTimeOfInteractions.Count; i++)
        {
            if (i == 0)
            {
                timeBetweenInteractions.Add("0:0:0:0");
            }
            else
            {
                TimeSpan diff = realTimeOfInteractions[i] - realTimeOfInteractions[i - 1];
                string lastPress = diff.Hours + ":" + diff.Minutes + ":" + diff.Seconds + ":" + diff.Milliseconds;
                timeBetweenInteractions.Add(lastPress);
            }
        }
        //converting stringlists to jsonstring format
        string jsonStringTimeBetweenInteractions = JsonUtility.ToJson(new StringListWrapper(timeBetweenInteractions));
        string jsonStringOrderOfInteraction = JsonUtility.ToJson(new StringListWrapper(listOrderOfInteraction));
        string jsonStringTimeOfInteraction = JsonUtility.ToJson(new StringListWrapper(listTimeOfInteraction));
        string jsonStringBatteryColumnsMax = JsonUtility.ToJson(new StringListWrapper(batteryColumnsMax));
        //sending event to unity analytics
        AnalyticsService.Instance.CustomData("ResourceDispatcherPuzzleTimes", new Dictionary<string, object>
        {
            { "totalTimeSpentUntilComplete", totalTimeSpent },
            { "timeBetweenInteractions", jsonStringTimeBetweenInteractions },
            { "orderOfInteraction", jsonStringOrderOfInteraction },
            { "timeOfInteraction", jsonStringTimeOfInteraction },
            { "batteryMaxCharges", jsonStringBatteryColumnsMax },
        });
    }
    #endregion

    #region SimonSays
    public void SimonSaysPuzzleFinished(bool completed)
    {
        AnalyticsService.Instance.CustomData("SimonSaysPuzzlePlayed", new Dictionary<string, object>
        {
            { "playedPuzzle", completed },
        });
    }
    public void SimonSaysPuzzleTimesAndOrder(string totalTimeInPuzzle, int levelReached, List<DateTime> realTimeOfInteractions, List<string> listOrderOfInteraction, List<string> listTimeOfInteraction)
    {
        //splitting string to remove number infront
        string[] partsOfString = totalTimeInPuzzle.Split('.');
        string totalTimeSpent = partsOfString[1];
        //creating list and setting first entry as null value
        List<string> timeBetweenInteractions = new();
        // calculating time difference between interactions and saving them to list
        for (int i = 0; i < realTimeOfInteractions.Count; i++)
        {
            if (i == 0)
            {
                timeBetweenInteractions.Add("0:0:0:0");
            }
            else
            {
                TimeSpan diff = realTimeOfInteractions[i] - realTimeOfInteractions[i - 1];
                string lastPress = diff.Hours + ":" + diff.Minutes + ":" + diff.Seconds + ":" + diff.Milliseconds;
                timeBetweenInteractions.Add(lastPress);
            }
        }
        //converting stringlists to jsonstring format
        string jsonStringTimeBetweenInteractions = JsonUtility.ToJson(new StringListWrapper(timeBetweenInteractions));
        string jsonStringOrderOfInteraction = JsonUtility.ToJson(new StringListWrapper(listOrderOfInteraction));
        string jsonStringTimeOfInteraction = JsonUtility.ToJson(new StringListWrapper(listTimeOfInteraction));
        //sending event to unity analytics
        AnalyticsService.Instance.CustomData("SimonSaysPuzzleTimes", new Dictionary<string, object>
        {
            { "totalTimeSpentUntilComplete", totalTimeSpent },
            { "timeBetweenInteractions", jsonStringTimeBetweenInteractions },
            { "orderOfInteraction", jsonStringOrderOfInteraction },
            { "timeOfInteraction", jsonStringTimeOfInteraction },
            { "levelReached", levelReached },
        });
    }


    #endregion

    #region ReactionTimePuzzle
    public void TrackButtonPress(DateTime lastButtonPressTime, DateTime currentTime)
    {
        TimeSpan t, u;
        //time since last press calculations
        timer.CalculateTimeDifference(lastButtonPressTime, currentTime, out t);
        string lastPress = t.Hours + ":" + t.Minutes + ":" + t.Seconds + ":" + t.Milliseconds;

        //time when button is pressed calculations
        timer.CalculateTimeDifference(timer.startTime, currentTime, out u);
        string currentPressTime = u.Hours + ":" + u.Minutes + ":" + u.Seconds + ":" + u.Milliseconds;

        AnalyticsService.Instance.CustomData("ReactionTimeTest", new Dictionary<string, object>
            {
                { "timeOfPress", currentPressTime },
                { "timeLastPress", lastPress}
            });
    }

    public void ReactionTimePuzzleFinished(bool completed)
    {
        AnalyticsService.Instance.CustomData("ReactionTimePuzzleCompleted", new Dictionary<string, object>
            {
                { "completedPuzzle", completed },
            });
    }

    public void ReactionTimePuzzleFailed(bool completed)
    {
        AnalyticsService.Instance.CustomData("ReactionTimePuzzleFailed", new Dictionary<string, object>
            {
                { "failedPuzzle", completed },
            });
    }

    #endregion

    #region CodeCrackingPuzzle
    public void CodeCrackingPuzzleFinished(bool completed)
    {
        AnalyticsService.Instance.CustomData("CodeCrackingPuzzleCompleted", new Dictionary<string, object>
        {
            { "completedPuzzle", completed },
        });
    }
    public void CodeCrackingPuzzleRotationNavigation(int outerLeft, int outerRight, int innerLeft, int innerRight)
    {
        AnalyticsService.Instance.CustomData("CodeCrackingPuzzleRotationNavigation", new Dictionary<string, object>
        {
            { "rotateInnerDialLeft", innerLeft },
            { "rotateInnerDialRight", innerRight },
            { "rotateOuterDialLeft", outerLeft },
            { "rotateOuterDialRight", outerRight },
        });
    }

    public void CodeCrackingPuzzleTimesAndOrder(string totalTimeInPuzzle, List<DateTime> realTimeOfInteractions, List<string> listOrderOfInteraction, List<string> listTimeOfInteraction)
    {
        //splitting string to remove number infront
        string[] partsOfString = totalTimeInPuzzle.Split('.');
        string totalTimeSpent = partsOfString[1];
        //creating list and setting first entry as null value
        List<string> timeBetweenInteractions = new();
        // calculating time difference between interactions and saving them to list
        for (int i = 0; i < realTimeOfInteractions.Count; i++)
        {
            if (i == 0)
            {
                timeBetweenInteractions.Add("0:0:0:0");
            }
            else
            {
                TimeSpan diff = realTimeOfInteractions[i] - realTimeOfInteractions[i - 1];
                string lastPress = diff.Hours + ":" + diff.Minutes + ":" + diff.Seconds + ":" + diff.Milliseconds;
                timeBetweenInteractions.Add(lastPress);
            }
        }
        //converting stringlists to jsonstring format
        string jsonStringTimeBetweenInteractions = JsonUtility.ToJson(new StringListWrapper(timeBetweenInteractions));
        string jsonStringOrderOfInteraction = JsonUtility.ToJson(new StringListWrapper(listOrderOfInteraction));
        string jsonStringTimeOfInteraction = JsonUtility.ToJson(new StringListWrapper(listTimeOfInteraction));
        //sending event to unity analytics
        AnalyticsService.Instance.CustomData("CodeCrackingPuzzleTimes", new Dictionary<string, object>
        {
            { "totalTimeSpentUntilComplete", totalTimeSpent },
            { "timeBetweenInteractions", jsonStringTimeBetweenInteractions },
            { "orderOfInteraction", jsonStringOrderOfInteraction },
            { "timeOfInteraction", jsonStringTimeOfInteraction },
        });
    }
    #endregion

    #region PatternPuzzle
    public void PatternPuzzleFinished(bool completed)
    {
        AnalyticsService.Instance.CustomData("PatternPuzzleCompleted", new Dictionary<string, object>
        {
            { "completedPuzzle", completed },
        });
    }
    public void PatternPuzzleBtnPresses(int BtnRow1L, int BtnRow1R, int BtnRow2L, int BtnRow2R, int BtnRow3L, int BtnRow3R, int BtnRow4L, int BtnRow4R)
    {
        AnalyticsService.Instance.CustomData("PatternPuzzleBtnPresses", new Dictionary<string, object>
        {
            { "numberOfBtnPressRow1L", BtnRow1L },
            { "numberOfBtnPressRow1R", BtnRow1R},
            { "numberOfBtnPressRow2L", BtnRow2L },
            { "numberOfBtnPressRow2R", BtnRow2R },
            { "numberOfBtnPressRow3L", BtnRow3L },
            { "numberOfBtnPressRow3R", BtnRow3R },
            { "numberOfBtnPressRow4L", BtnRow4L },
            { "numberOfBtnPressRow4R", BtnRow4R },
        });
    }
    public void PatternPuzzleTimesAndOrder(string totalTimeInPuzzle, List<DateTime> realTimeOfInteractions, List<string> listOrderOfInteraction, List<string> listTimeOfInteraction)
    {
        //splitting string to remove number infront
        string[] partsOfString = totalTimeInPuzzle.Split('.');
        string totalTimeSpent = partsOfString[1];
        //creating list and setting first entry as null value
        List<string> timeBetweenInteractions = new();
        // calculating time difference between interactions and saving them to list
        for (int i = 0; i < realTimeOfInteractions.Count; i++)
        {
            if (i == 0)
            {
                timeBetweenInteractions.Add("0:0:0:0");
            }
            else
            {
                TimeSpan diff = realTimeOfInteractions[i] - realTimeOfInteractions[i - 1];
                string lastPress = diff.Hours + ":" + diff.Minutes + ":" + diff.Seconds + ":" + diff.Milliseconds;
                timeBetweenInteractions.Add(lastPress);
            }
        }
        //converting stringlists to jsonstring format
        string jsonStringTimeBetweenInteractions = JsonUtility.ToJson(new StringListWrapper(timeBetweenInteractions));
        string jsonStringOrderOfInteraction = JsonUtility.ToJson(new StringListWrapper(listOrderOfInteraction));
        string jsonStringTimeOfInteraction = JsonUtility.ToJson(new StringListWrapper(listTimeOfInteraction));
        //sending event to unity analytics
        AnalyticsService.Instance.CustomData("PatternPuzzleTimes", new Dictionary<string, object>
        {
            { "totalTimeSpentUntilComplete", totalTimeSpent },
            { "timeBetweenInteractions", jsonStringTimeBetweenInteractions },
            { "orderOfInteraction", jsonStringOrderOfInteraction },
            { "timeOfInteraction", jsonStringTimeOfInteraction },
        });
    }
    #endregion

    #region islandTimes


    //later used for basic
    //public void timeToInteractionsAndDistanceBetween(string timeToInteractionsFromStart, float distanceTraveledBetweenInteractions)
    //{
    //    AnalyticsService.Instance.CustomData("timeToInteractionsAndDistanceBetween", new Dictionary<string, object>
    //        {
    //            { "timeToInteractionsFromStart", timeToInteractionsFromStart },         
    //            { "distanceTraveledBetweenInteractions", distanceTraveledBetweenInteractions },
    //        });
    //}
    //public void timeToExitBeach(string timeToExitBeach)
    //{
    //    AnalyticsService.Instance.CustomData("timeToExitBeach", new Dictionary<string, object>
    //        {

    //            { "timeToExitBeach", timeToExitBeach },      
    //        });
    //}
    //public void timeToWindmill(string timeToWindmill)
    //{
    //    AnalyticsService.Instance.CustomData("timeToWindmill", new Dictionary<string, object>
    //        {
    //            { "timeToWindmill", timeToWindmill },             
    //        });
    //}
    //public void timeToHouse(string timeToHouse)
    //{
    //    AnalyticsService.Instance.CustomData("timeToHouse", new Dictionary<string, object>
    //        { 
    //            { "timeToHouse", timeToHouse },        
    //        });
    //}

    #endregion

}
