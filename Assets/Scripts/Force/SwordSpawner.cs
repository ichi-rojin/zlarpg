using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSpawner : BaseForceSpawner
{
    public override void Init(ForceSpawnerStats stats)
    {
        base.Init(stats);
    }

    public override void Action(Vector2 coord, Character target)
    {
        //if (_forces.Count >= _stats.SpawnCount) return;
        if (_coolTime > 0) return;
        _coolTime = 10;
        var duration = _mapManager.CalcDurationBySpeed(_stats[ForceType.Speed]);
        _stats.AliveTime = _stats[ForceType.Range] * duration;
        Sword force = (Sword)CreateForce(coord, target, _forcesParent.transform);
        force._target = target;
    }
}