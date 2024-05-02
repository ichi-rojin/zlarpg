using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSettings", menuName = "ScriptableObjects/CharacterSettings")]
public class CharacterSettings : ScriptableObject
{
    //キャラクターデータ
    public List<CharacterStats> datas;

    private static CharacterSettings instance;

    public static CharacterSettings Instance
    {
        get
        {
            if (!instance)
            {
                instance = Resources.Load<CharacterSettings>(nameof(CharacterSettings));
            }
            return instance;
        }
    }

    public CharacterStats Get(int id)
    {
        return (CharacterStats)datas.Find(item => item.Id == id).GetCopy();
    }
}

[Serializable]
public class CharacterStats : BaseStats
{
    public GameObject Prefabs;
}