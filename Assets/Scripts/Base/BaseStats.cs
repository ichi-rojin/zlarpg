using UnityEngine;

public class BaseStats
{
    //Inspectorで表示されるタイトル
    public string Title;

    public int Id;
    public int Lv;
    public string Name;
    [TextArea] public string Description;
    public int Hp;
    public int Sense;
    public int Strength;
    public int Speed;
    public int Jump;
    public int Throughput;
    public int Memory;

    // StatsTypeとの紐づけ　インデクサ
    public int this[StatsType key]
    {
        get
        {
            switch (key)
            {
                case StatsType.Hp:
                    return Hp;

                case StatsType.Sense:
                    return Sense;

                case StatsType.Strength:
                    return Strength;

                case StatsType.Speed:
                    return Speed;

                case StatsType.Jump:
                    return Jump;

                case StatsType.Throughput:
                    return Throughput;

                case StatsType.Memory:
                    return Memory;

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

                case StatsType.Throughput:
                    Throughput = value;
                    break;

                case StatsType.Memory:
                    Memory = value;
                    break;
            }
        }
    }

    public BaseStats GetCopy()
    {
        return (BaseStats)MemberwiseClone();
    }

    public void UpValue(StatsType type, int value)
    {
        this[type] += value;
    }
}