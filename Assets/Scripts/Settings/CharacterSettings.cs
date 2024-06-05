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
        return datas.Find(item => item.Id == id).GetCopy<CharacterStats>();
    }

    // キャラ生成
    public GameObject CreateCharacter(int id, MapManager mapManager, Vector2Int pos, Transform parent)
    {
        // ステータス取得
        CharacterStats stats = Instance.Get(id);
        Vector2 coord = mapManager.GetWorldPositionFromTile(pos.x, pos.y); // 基点の座標を元に変数posを宣言
        GameObject charaGameObject = Instantiate(stats.Prefab, coord, Quaternion.Euler(0, 0, 0f), parent);
        Character character = charaGameObject.GetComponent<Character>();
        character.SetPos(pos);
        character.Init(stats, mapManager);
        return charaGameObject;
    }
}

[Serializable]
public class CharacterStats : BaseStats
{
    public GameObject Prefab;

    // 初期装備フォースID
    public List<int> DefaultForceIds;

    // 装備可能フォースID
    public List<int> UsableForceIds;

    // 装備可能数
    public int UsableForceMax;
}