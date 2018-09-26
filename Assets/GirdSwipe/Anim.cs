using UnityEngine;

public class Anim
{
    public static float Liner(float startValue, float endValue, float time, float duration)
    {
        float differenceValue = endValue - startValue;
        time = Mathf.Clamp(time, 0f, duration);
        time /= duration;

        if (time == 0f)
            return startValue;
        if (time == 1f)
            return endValue;

        return differenceValue * time + startValue;
    }
}