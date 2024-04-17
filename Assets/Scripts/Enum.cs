using UnityEngine;

public enum StatusType
{
    Hp,
    Sense,
    Strength,
    Speed,
}

public static partial class EnumExtend
{
    public static string GetUpMethod(this StatusType param)
    {
        return "Up" + param;
    }

    public static int GetUpRandomValue(this StatusType param)
    {
        int value = new int();
        switch (param)
        {
            case StatusType.Hp:
                value = Random.Range(0, 10);
                break;

            case StatusType.Sense:
                value = Random.Range(0, 1);
                break;

            case StatusType.Strength:
                value = Random.Range(0, 5);
                break;

            case StatusType.Speed:
                value = Random.Range(0, 1);
                break;
        }
        return value;
    }

    public static int GetUpInitialValue(this StatusType param)
    {
        int value = new int();
        switch (param)
        {
            case StatusType.Hp:
                value = Random.Range(100, 300);
                break;

            case StatusType.Sense:
                value = Random.Range(2, 10);
                break;

            case StatusType.Strength:
                value = Random.Range(5, 10);
                break;

            case StatusType.Speed:
                value = Random.Range(1, 5);
                break;
        }
        return value;
    }

    public static int GetMaxValue(this StatusType param)
    {
        int value = new int();
        switch (param)
        {
            case StatusType.Hp:
                value = 1000;
                break;

            case StatusType.Sense:
                value = 20;
                break;

            case StatusType.Strength:
                value = 20;
                break;

            case StatusType.Speed:
                value = 20;
                break;
        }
        return value;
    }
}