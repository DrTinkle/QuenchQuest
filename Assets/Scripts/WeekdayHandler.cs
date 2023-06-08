using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeekdayHandler : MonoBehaviour
{
    [SerializeField] GameData gameData;

    readonly List<string> daysOfWeek = new()
    {
    "Monday",
    "Tuesday",
    "Wednesday",
    "Thursday",
    "Friday",
    "Saturday",
    "Sunday"
    };


    public void SetNextDay()
    {
        gameData.currentDayIndex = (gameData.currentDayIndex + 1) % daysOfWeek.Count;
    }

    public string GetWeekDay()
    { 
        return daysOfWeek[gameData.currentDayIndex];
    }
}
