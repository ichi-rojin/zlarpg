using UnityEngine;

//í«â¡ÉfÅ[É^
public enum StatsType
{
    Hp,
    MaxHp,
    Sense,
    Strength,
    Speed,
    Jump,
}

public static partial class StatsTypeExtend
{
    public static int GetUpRandomValue(this StatsType param)
    {
        int value = new int();
        int lessI = 1;//intå^ÇÃRandom.RangeÇÕmaxÇä‹Ç‹Ç»Ç¢ÇΩÇﬂ
        switch (param)
        {
            case StatsType.MaxHp:
                value = Random.Range(0, 10 + lessI);
                break;

            case StatsType.Sense:
                value = Random.Range(0, 1 + lessI);
                break;

            case StatsType.Strength:
                value = Random.Range(0, 5 + lessI);
                break;

            case StatsType.Speed:
                value = Random.Range(0, 1 + lessI);
                break;

            case StatsType.Jump:
                value = Random.Range(0, 1 + lessI);
                break;
        }
        return value;
    }

    public static int GetInitialBonus(this StatsType param)
    {
        int value = new int();
        int lessI = 1;//intå^ÇÃRandom.RangeÇÕmaxÇä‹Ç‹Ç»Ç¢ÇΩÇﬂ
        switch (param)
        {
            case StatsType.MaxHp:
                value = Random.Range(0, 200 + lessI);
                break;

            case StatsType.Sense:
                value = Random.Range(0, 3 + lessI);
                break;

            case StatsType.Strength:
                value = Random.Range(0, 3 + lessI);
                break;

            case StatsType.Speed:
                value = Random.Range(0, 3 + lessI);
                break;

            case StatsType.Jump:
                value = Random.Range(0, 3 + lessI);
                break;
        }
        return value;
    }

    public static int GetMaxValue(this StatsType param)
    {
        int value = new int();
        switch (param)
        {
            case StatsType.MaxHp:
                value = 1000;
                break;

            case StatsType.Sense:
                value = 15;
                break;

            case StatsType.Strength:
                value = 15;
                break;

            case StatsType.Speed:
                value = 20;
                break;

            case StatsType.Jump:
                value = 10;
                break;
        }
        return value;
    }
}