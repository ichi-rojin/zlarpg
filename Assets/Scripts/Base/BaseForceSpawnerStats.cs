using UnityEngine;

public class BaseForceSpawnerStats
{
    //Inspectorで表示されるタイトル
    public string Title;

    public int Id;
    public int Lv;
    public string Name;
    [TextArea] public string Description;
    public int Physical;
    public int Speed;
    public int Range;
    public int SpawnCount;
    public float AliveTime;

    // ForceTypeとの紐づけ　インデクサ
    public int this[ForceType key]
    {
        get
        {
            switch (key)
            {
                case ForceType.Physical:
                    return Physical;

                case ForceType.Speed:
                    return Speed;

                case ForceType.Range:
                    return Range;

                case ForceType.SpawnCount:
                    return SpawnCount;

                default:
                    return 0;
            }
        }

        set
        {
            switch (key)
            {
                case ForceType.Physical:
                    Physical = value;
                    break;

                case ForceType.Speed:
                    Speed = value;
                    break;

                case ForceType.Range:
                    Range = value;
                    break;

                case ForceType.SpawnCount:
                    SpawnCount = value;
                    break;
            }
        }
    }

    public BaseForceSpawnerStats GetCopy()
    {
        return (BaseForceSpawnerStats)MemberwiseClone();
    }

    public void UpValue(ForceType type, int value)
    {
        this[type] += value;
    }
}