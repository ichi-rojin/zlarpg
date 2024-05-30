using System.Collections;
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

    public ForceSpawnerStats Get(int id)
    {
        return (ForceSpawnerStats)datas.Find(item => item.Id == id).GetCopy();
    }
}