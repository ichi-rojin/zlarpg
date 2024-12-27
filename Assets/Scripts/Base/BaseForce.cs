using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BaseForce : Token
{
    protected BaseForceSpawner _spawner;
    protected ForceSpawnerStats _stats;
    protected Vector2Int _forward;

    // 生存時間
    public float _alivetime;

    // ターゲット
    public Character _target;

    [SerializeField]
    protected float _angle;
    protected float _duration;
    protected Tweener _tweener;

    public void Init(BaseForceSpawner spawner, Vector2Int forward)
    {
        _spawner = spawner;
        _stats = (ForceSpawnerStats)spawner.stats.GetCopy();
        _forward = forward;
    }

    protected void AttackTarget(int attack, Character target)
    {
        if (!target.TryGetComponent<Character>(out var character)) return;
        target.Damage(attack, target);
    }

    protected void CountUpAliveTime()
    {
        _alivetime += Time.fixedDeltaTime;
    }

    protected bool CheckLifespan()
    {
        return _alivetime >= _stats.AliveTime;
    }

    protected void SetDuration()
    {
        _duration = _mapManager.CalcDurationBySpeed(_stats[ForceType.Speed]);
    }

    protected Quaternion GetRotate()
    {
        //進行方向
        Vector2 forward = _target.pos - pos;
        //角度に変換
        float degree = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0, 0, -1 * degree - 90);
    }

    protected void SetRotate()
    {
        transform.rotation = GetRotate();
    }

    protected float GetAngle()
    {
        Vector2 forward = _target.pos - pos;
        float degree = Mathf.Atan2(forward.x, forward.y) * Mathf.Rad2Deg;
        return degree;
    }

    protected void CheckObstruction()
    {
        if (_spawner._mapManager.map[pos.x, pos.y].ToString() == "g") return;
        Vanish();
    }

    protected void SetAngle()
    {
        _angle = GetAngle();
    }
}