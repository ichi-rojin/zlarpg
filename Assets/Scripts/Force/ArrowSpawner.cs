using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSpawner : BaseForceSpawner
{
    public override void Init(ForceSpawnerStats stats)
    {
        base.Init(stats);
        var duration = _mapManager.CalcDurationBySpeed(_stats[ForceType.Speed]);
        _stats.Range = (int)(_stats.AliveTime / duration);
    }
    public void Action(Vector2 coord, Character target)
    {
        //if (_forces.Count >= _stats.SpawnCount) return;
        Arrow force = (Arrow)CreateForce(coord, target, _forcesParent.transform);
        force._target = target;
    }
}