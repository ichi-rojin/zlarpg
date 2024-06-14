using System.Collections;
using System.Collections.Generic;
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

    protected float _angle;
    protected float _duration;

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
        float rad = Mathf.Atan2(forward.y, forward.x);
        var degree = rad * Mathf.Rad2Deg;
        return Quaternion.Euler(0, 0, degree);
    }

    protected void SetRotate()
    {
        transform.rotation = GetRotate();
    }

    protected float GetAngle()
    {
        Vector2 forward = _target.pos - pos;
        float rad = Mathf.Atan2(forward.x, forward.y);
        float degree = rad * Mathf.Rad2Deg;

        if (degree < 0)
        {
            degree += 360;
        }

        return degree;
    }

    protected void SetAngle()
    {
        _angle = GetAngle();
    }
}