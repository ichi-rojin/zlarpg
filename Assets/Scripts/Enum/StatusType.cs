using UnityEngine;

public enum StatusType
{
    Hp,
    Sense,
    Strength,
    Speed,
    Jump,
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
        int lessI = 1;//intŒ^‚ÌRandom.Range‚Ímax‚ðŠÜ‚Ü‚È‚¢‚½‚ß
        switch (param)
        {
            case StatusType.Hp:
                value = Random.Range(0, 10 + lessI);
                break;

            case StatusType.Sense:
                value = Random.Range(0, 1 + lessI);
                break;

            case StatusType.Strength:
                value = Random.Range(0, 5 + lessI);
                break;

            case StatusType.Speed:
                value = Random.Range(0, 1 + lessI);
                break;

            case StatusType.Jump:
                value = Random.Range(0, 1 + lessI);
                break;
        }
        return value;
    }

    public static int GetUpInitialValue(this StatusType param, CharacterStats stats)
    {
        int value = new int();
        int lessI = 1;//intŒ^‚ÌRandom.Range‚Ímax‚ðŠÜ‚Ü‚È‚¢‚½‚ß
        switch (param)
        {
            case StatusType.Hp:
                value = Random.Range(stats.Hp, 300 + lessI);
                break;

            case StatusType.Sense:
                value = Random.Range(stats.Sense, 10 + lessI);
                break;

            case StatusType.Strength:
                value = Random.Range(stats.Strength, 10 + lessI);
                break;

            case StatusType.Speed:
                value = Random.Range(stats.Speed, 5 + lessI);
                break;

            case StatusType.Jump:
                value = Random.Range(stats.Jump, 5 + lessI);
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
                value = 15;
                break;

            case StatusType.Strength:
                value = 15;
                break;

            case StatusType.Speed:
                value = 15;
                break;

            case StatusType.Jump:
                value = 15;
                break;
        }
        return value;
    }
}