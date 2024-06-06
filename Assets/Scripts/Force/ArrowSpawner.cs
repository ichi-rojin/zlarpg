using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSpawner : BaseForceSpawner
{
    public void Action(Vector2 coord, Character target)
    {
        //if (_forces.Count >= _stats.SpawnCount) return;
        Arrow force = (Arrow)CreateForce(coord, target, _forcesParent.transform);
        force.target = target;
    }
}