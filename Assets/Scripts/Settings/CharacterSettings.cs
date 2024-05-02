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

    // キャラ生成
    public GameObject CreateCharacter(int id, Vector2 pos, Transform parent)
    {
        // ステータス取得
        CharacterStats stats = Instance.Get(id);
        GameObject charaGameObject = Instantiate(stats.Prefab, pos, Quaternion.Euler(0, 0, 0f), parent);
        Character character = charaGameObject.GetComponent<Character>();
        character.Init(stats);
        return charaGameObject;
    }
}

[Serializable]
public class CharacterStats : BaseStats
{
    public GameObject Prefab;
}