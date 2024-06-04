using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseForce : Token
{
    protected BaseForceSpawner _spawner;
    protected ForceSpawnerStats _stats;
    protected Vector2Int _forward;

    public void Init(BaseForceSpawner spawner, Vector2Int forward)
    {
        _spawner = spawner;
        _stats = (ForceSpawnerStats)spawner._stats.GetCopy();
        _forward = forward;
    }

    protected void AttackTarget(int attack, Character target)
    {
        if (!target.TryGetComponent<Character>(out var character)) return;
        target.Damage(attack, target);
    }
}