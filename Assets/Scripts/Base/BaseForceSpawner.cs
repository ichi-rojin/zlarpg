using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseForceSpawner : MonoBehaviour
{
    public GameObject PrefabForce;
    public ForceSpawnerStats Stats;
    private List<BaseForce> _forces;

    public void Init()
    {
        _forces = new List<BaseForce>();
    }

    protected BaseForce CreateForce(Vector2 pos, Vector2Int forward, Transform parent)
    {
        GameObject obj = Instantiate(PrefabForce, pos, PrefabForce.transform.rotation);
        BaseForce force = obj.GetComponent<BaseForce>();
        force.Init(this, forward);
        _forces.Add(force);

        return force;
    }

    protected BaseForce CreateForce(Vector2Int pos, Transform parent = null)
    {
        return CreateForce(pos, Vector2Int.zero, parent);
    }
}