using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ForceSpawnerSettings", menuName = "ScriptableObjects/ForceSpawnerSettings")]
public class ForceSpawnerSettings : ScriptableObject
{
    //キャラクターデータ
    public List<ForceSpawnerStats> datas;

    private static ForceSpawnerSettings instance;

    public static ForceSpawnerSettings Instance
    {
        get
        {
            if (!instance)
            {
                instance = Resources.Load<ForceSpawnerSettings>(nameof(ForceSpawnerSettings));
            }
            return instance;
        }
    }

    public ForceSpawnerStats Get(int id, int lv)
    {
        //指定されたレベルのデータがなければ一番高いレベルのデータを返す
        ForceSpawnerStats ret = null;

        foreach (var force in datas)
        {
            if (id != force.Id) continue;
            //指定レベルと一致
            if (lv == force.Lv)
            {
                return (ForceSpawnerStats)force.GetCopy();
            }

            //仮のデータがセットされていないか、それを超えるレベルがあったら入れ替える
            if (ret == null)
            {
                ret = force;
            }
            else if (force.Lv < lv && ret.Lv < force.Lv)
            {
                ret = force;
            }
        }
        return (ForceSpawnerStats)ret.GetCopy();
    }

    // フォース生成
    public BaseForceSpawner CreateForceSpawner(int id, MapManager mapManager, Vector2Int pos, Transform parent = null)
    {
        // データ取得
        ForceSpawnerStats stats = Instance.Get(id, 1);
        Vector2 coord = mapManager.GetWorldPositionFromTile(pos.x, pos.y); // 基点の座標を元に変数posを宣言
        GameObject forceGameObject = Instantiate(stats.Prefab, coord, Quaternion.Euler(0, 0, 0f), parent);
        BaseForceSpawner spawner = forceGameObject.GetComponent<BaseForceSpawner>();
        spawner.Init(stats);
        return spawner;
    }
}

[Serializable]
public class ForceSpawnerStats : BaseForceSpawnerStats
{
    public GameObject Prefab;
}