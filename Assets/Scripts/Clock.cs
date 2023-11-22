using System;
using UnityEngine;

public class Clock : MonoBehaviour
{
    const float hoursToDegrees = -30f;
    const float minutesToDegrees = -6f;
    const float secondsToDegrees = -6f;
    const float secondsToMinuteDegrees = -0.1f;
    const float secondsToHourDegrees = -0.001666f;
    const float minutesToHourDegrees = -0.5f;



    [SerializeField]
    Transform hoursPivot, minutesPivot, secondsPivot;

    private void Awake()
    {
        UpdateArms();
    }

    private void UpdateArms()
    {
        var currentTime = DateTime.Now;
        int hour = currentTime.Hour;
        int minute = currentTime.Minute;
        int second = currentTime.Second;

        float hoursAngle = hoursToDegrees * hour + minutesToHourDegrees * minute + secondsToHourDegrees * second;
        float minutesAngle = minutesToDegrees * minute + secondsToMinuteDegrees * second;
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
