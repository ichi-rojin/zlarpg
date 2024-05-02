using UnityEngine;

//追加データ
public enum StatsType
{
    Hp,
    MaxHp,
    Sense,
    Strength,
    Speed,
    Jump,
}

public static partial class EnumExtend
{
    public static string GetUpMethod(this StatsType param)
    {
        return "Up" + param;
    }

    public static int GetUpRandomValue(this StatsType param)
    {
        int value = new int();
        int lessI = 1;//int型のRandom.Rangeはmaxを含まないため
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

public class BaseStats
{
    //Inspectorで表示されるタイトル
    public string Title;

    public int Id;
    public int Lv;
    public string Name;
    [TextArea] public string Description;
    public int Hp;
    public int MaxHp;
    public int Sense;
    public int Strength;
    public int Speed;
    public int Jump;

    // StatsTypeとの紐づけ　インデクサ
    public int this[StatsType key]
    {
        get
        {
            switch (key)
            {
                case StatsType.Hp:
                    return Hp;

                case StatsType.MaxHp:
                    return MaxHp;

                case StatsType.Sense:
                    return Sense;

                case StatsType.Strength:
                    return Strength;

                case StatsType.Speed:
                    return Speed;

                case StatsType.Jump:
                    return Jump;

                default:
                    return 0;
            }
        }

        set
        {
            switch (key)
            {
                case StatsType.Hp:
                    Hp = value;
                    break;

                case StatsType.MaxHp:
                    MaxHp = value;
                    break;

                case StatsType.Sense:
                    Sense = value;
                    break;

                case StatsType.Strength:
                    Strength = value;
                    break;

                case StatsType.Speed:
                    Speed = value;
                    break;

                case StatsType.Jump:
                    Jump = value;
                    break;
            }
        }
    }

    public BaseStats GetCopy()
    {
        return (BaseStats)MemberwiseClone();
    }
}