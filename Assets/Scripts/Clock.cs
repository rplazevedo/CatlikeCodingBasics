using System;
using UnityEngine;

public class Clock : MonoBehaviour
{
    const float hoursToDegrees = -30f;
    const float minutesToDegrees = -6f;
    const float secondsToDegrees = -6f;



    [SerializeField]
    Transform hoursPivot, minutesPivot, secondsPivot;

    private void Awake()
    {
        UpdateArms();
    }

    private void UpdateArms()
    {
        TimeSpan currentTime = DateTime.Now.TimeOfDay;

        float hour = (float)currentTime.TotalHours;
        float minute = (float)currentTime.TotalMinutes;
        float second = (float)currentTime.Seconds;

        float hoursAngle = hoursToDegrees * hour;
        float minutesAngle = minutesToDegrees * minute;
        float secondsAngle = secondsToDegrees * second;

        hoursPivot.localRotation = RotateClockArm(hoursAngle);
        minutesPivot.localRotation = RotateClockArm(minutesAngle);
        secondsPivot.localRotation = RotateClockArm(secondsAngle);
    }

    private void FixedUpdate()
    {
        UpdateArms();
    }

    private Quaternion RotateClockArm(float angle)
    {
        return Quaternion.Euler(0f, 0f, angle);
    }
}
