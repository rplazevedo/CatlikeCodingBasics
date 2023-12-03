using UnityEngine;
using static FunctionLibrary;
using UnityEngine.UIElements;
using static UnityEngine.Mathf;
public static class FunctionLibrary
{
    public delegate float Function(float x, float t);

    public static Function GetFunction (int index)
    {
        if (index == 0)
        {
            return Wave;
        }
        else if (index == 1)
        {
            return MultiWave;
        }
        else
        {
            return Ripple;
        }
    }
    public static float Wave (float x, float t)
    {
        return Sin(PI * (x + t));
    }

    public static float MultiWave(float x, float t)
    {
        float y = Sin(PI * (x + 0.5f * t));
        y +=  0.5f * Sin(2f * PI * (x + t));
        return y * ( 2f / 3f );
    }

    public static float Ripple(float x, float t)
    {
        float d = Abs(x);
        float y = Sin(PI * ( 4f * d - t));
        return y / (1f + 10f * d);
    }
}
