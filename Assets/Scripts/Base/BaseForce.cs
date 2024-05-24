using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseForce : MonoBehaviour
{
    protected BaseForceSpawner spawner;
    protected ForceSpawnerStats stats;
    protected Vector2Int forward;

    public void Init(BaseForceSpawner spawner, Vector2Int forward)
    {
        this.spawner = spawner;
        this.stats = (ForceSpawnerStats)spawner.Stats.GetCopy();
        this.forward = forward;
    }

    protected void AttackTarget(int attack, Character target)
    {
        if (!target.TryGetComponent<Character>(out var character)) return;
        target.Damage(attack, target);
    }
}