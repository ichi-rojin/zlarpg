using UnityEngine;

public class BaseForceSpawnerStats
{
    //Inspector�ŕ\�������^�C�g��
    public string Title;

    public int Id;
    public int Lv;
    public string Name;
    [TextArea] public string Description;
    public int Physical;
    public int Speed;
    public int Range;
    public int SpawnCount;

    // ForceType�Ƃ̕R�Â��@�C���f�N�T
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
                    return Speed;

                case ForceType.SpawnCount:
                    return Speed;

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