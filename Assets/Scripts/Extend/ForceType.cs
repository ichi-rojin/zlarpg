using UnityEngine;

//í«â¡ÉfÅ[É^
public enum ForceType
{
    Physical,
    Speed,
}

public static partial class ForceTypeExtend
{
    public static int GetUpRandomValue(this ForceType param)
    {
        int value = new int();
        int lessI = 1;//intå^ÇÃRandom.RangeÇÕmaxÇä‹Ç‹Ç»Ç¢ÇΩÇﬂ
        switch (param)
        {
            case ForceType.Physical:
                value = Random.Range(0, 5 + lessI);
                break;

            case ForceType.Speed:
                value = Random.Range(0, 1 + lessI);
                break;
        }
        return value;
    }

    public static int GetInitialBonus(this ForceType param)
    {
        int value = new int();
        int lessI = 1;//intå^ÇÃRandom.RangeÇÕmaxÇä‹Ç‹Ç»Ç¢ÇΩÇﬂ
        switch (param)
        {
            case ForceType.Physical:
                value = Random.Range(0, 3 + lessI);
                break;

            case ForceType.Speed:
                value = Random.Range(0, 3 + lessI);
                break;
        }
        return value;
    }

    public static int GetMaxValue(this ForceType param)
    {
        int value = new int();
        switch (param)
        {
            case ForceType.Physical:
                value = 15;
                break;

            case ForceType.Speed:
                value = 20;
                break;
        }
        return value;
    }
}