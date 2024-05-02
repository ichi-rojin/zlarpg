using UnityEngine;

//�ǉ��f�[�^
public enum StatsType
{
    Hp,
    MaxHp,
    Sense,
    Strength,
    Speed,
    Jump,
}

public class BaseStats
{
    //Inspector�ŕ\�������^�C�g��
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

    // StatsType�Ƃ̕R�Â��@�C���f�N�T
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