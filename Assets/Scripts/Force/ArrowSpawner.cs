using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSpawner : BaseForceSpawner
{
    public void Action(Vector2 coord, Character target)
    {
        Arrow force = (Arrow)CreateForce(coord, target, this.gameObject.transform);
        force.target = target;
    }
}