using UnityEngine;

//追加データ
public enum StatsType
{
    Hp,
    Sense,
    Strength,
    Speed,
    Jump,
    Throughput,
    Memory,
}

public static partial class StatsTypeExtend
{
    public static int GetUpRandomValue(this StatsType param)
    {
        int value = new int();
        int lessI = 1;//int型のRandom.Rangeはmaxを含まないため
        switch (param)
        {
            case StatsType.Hp:
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

            case StatsType.Throughput:
                value = Random.Range(0, 1 + lessI);
                break;

            case StatsType.Memory:
                value = Random.Range(0, 1 + lessI);
                break;
        }
        return value;
    }

    public static int GetInitialBonus(this StatsType param)
    {
        int value = new int();
        int lessI = 1;//int型のRandom.Rangeはmaxを含まないため
        switch (param)
        {
            case StatsType.Hp:
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

            case StatsType.Throughput:
                value = Random.Range(0, 50 + lessI);
                break;

            case StatsType.Memory:
                value = Random.Range(0, 50 + lessI);
                break;
        }
        return value;
    }

    public static int GetMaxValue(this StatsType param)
    {
        int value = new int();
        switch (param)
        {
            case StatsType.Hp:
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

            case StatsType.Throughput:
                value = 100;
                break;

            case StatsType.Memory:
                value = 100;
                break;
        }
        return value;
    }
}