using UnityEngine;

public class ForceSpawnerStats
{
    //Inspector�ŕ\�������^�C�g��
    public string Title;

    public int Id;
    public int Lv;
    public string Name;
    [TextArea] public string Description;
    public int Physical;
    public int Speed;

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

    public ForceSpawnerStats GetCopy()
    {
        return (ForceSpawnerStats)MemberwiseClone();
    }

    public void UpValue(ForceType type, int value)
    {
        this[type] += value;
    }
}