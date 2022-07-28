using System;
using UnityEngine;

[Serializable]
public struct StageAvailability
{
    public DayOfWeek day;
    [Range(0, 23)]
    public int startTimeHour;
    [Range(0, 59)]
    public int startTimeMinute;
    [Range(0, 23)]
    public int durationHour;
    [Range(0, 59)]
    public int durationMinute;

    /// <summary>
    /// This function to adjust event duration to not over 1 day
    /// </summary>
    /// <returns></returns>
    public StageAvailability ValidateSetting(out bool hasChanges)
    {
        hasChanges = false;
        TimeSpan time = new TimeSpan(startTimeHour, startTimeMinute, 0);
        time = time.Add(new TimeSpan(durationHour, durationMinute, 0));
        if (time.TotalHours - 24 > 0)
        {
            durationHour -= (int)time.TotalHours - 24;
            time = time.Subtract(TimeSpan.FromHours((int)time.TotalHours - 24));
            hasChanges = true;
        }
        if (time.TotalMinutes - (60 * 24) > 0)
        {
            durationMinute -= (int)(time.TotalMinutes - (60 * 24));
            hasChanges = true;
        }
        return this;
    }

    public string ToJson()
    {
        return "{" +
            "\"day\":" + (byte)day + "," +
            "\"startTimeHour\":" + startTimeHour + "," +
            "\"startTimeMinute\":" + startTimeMinute + "," +
            "\"durationHour\":" + durationHour + "," +
            "\"durationMinute\":" + durationMinute + "}";
    }
}