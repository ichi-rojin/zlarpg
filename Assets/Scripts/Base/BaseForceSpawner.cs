using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseForceSpawner : MonoBehaviour
{
    public GameObject PrefabForce;
    public ForceSpawnerStats _stats;

    // 生成タイマー
    protected float _spawnTimer;

    protected List<BaseForce> _forces;

    public void Init(ForceSpawnerStats stats)
    {
        _forces = new List<BaseForce>();
        _stats = stats;
    }

    protected BaseForce CreateForce(Vector2 coord, Vector2Int forward, Character target = null, Transform parent = null)
    {
        GameObject obj = Instantiate(PrefabForce, coord, PrefabForce.transform.rotation, parent);
        BaseForce force = obj.GetComponent<BaseForce>();
        force.Init(this, forward);
        _forces.Add(force);

        return force;
    }

    protected BaseForce CreateForce(Vector2 coord, Character target = null, Transform parent = null)
    {
        return CreateForce(coord, Vector2Int.zero, target, parent);
    }
}