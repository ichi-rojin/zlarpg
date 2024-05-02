using System;
using UnityEngine;

public class Character : Token
{
    [SerializeField]
    [Header("ステータス")]
    private CharacterStats _stats;

    public CharacterStats stats
    {
        get { return _stats; }
    }

    // 状態.
    public enum eOrientation
    {
        East,
        West,
        South,
        North,
    }

    [SerializeField]
    [Header("体の向き")]
    private eOrientation _orientation = eOrientation.South;

    public eOrientation orientation
    {
        get { return _orientation; }
    }

    private Animator _animator;

    // Start is called before the first frame update

    public void Init(CharacterStats stats)
    {
        _animator = GetComponent<Animator>();
        _stats = stats;
    }

    public void SetOrientation(eOrientation orientation)
    {
        if (_orientation == orientation) return;
        _orientation = orientation;
        switch (_orientation)
        {
            case eOrientation.East:
                _animator.SetTrigger("isRight");
                break;

            case eOrientation.West:
                _animator.SetTrigger("isLeft");
                break;

            case eOrientation.South:
                _animator.SetTrigger("isDown");
                break;

            case eOrientation.North:
                _animator.SetTrigger("isUp");
                break;

            default:
                _animator.SetTrigger("isUp");
                break;
        }
    }

    public void UpHp(int hp)
    {
        int max = StatsType.MaxHp.GetMaxValue();
        _stats[StatsType.Hp] = Math.Clamp(_stats[StatsType.Hp] + hp, _stats[StatsType.Hp], max);
    }

    public void UpMaxHp(int maxHp)
    {
        int max = StatsType.MaxHp.GetMaxValue();
        _stats[StatsType.MaxHp] = Math.Clamp(_stats[StatsType.MaxHp] + maxHp, _stats[StatsType.MaxHp], max);
        if (_stats[StatsType.Hp] > _stats[StatsType.MaxHp]) _stats[StatsType.Hp] = _stats[StatsType.MaxHp];
    }

    public void UpSense(int sense)
    {
        int max = StatsType.Sense.GetMaxValue();
        _stats[StatsType.Sense] = Math.Clamp(_stats[StatsType.Sense] + sense, _stats[StatsType.Sense], max);
    }

    public void UpStrength(int strength)
    {
        int max = StatsType.Strength.GetMaxValue();
        _stats[StatsType.Strength] = Math.Clamp(_stats[StatsType.Strength] + strength, _stats[StatsType.Strength], max);
    }

    public void UpSpeed(int speed)
    {
        int max = StatsType.Speed.GetMaxValue();
        _stats[StatsType.Speed] = Math.Clamp(_stats[StatsType.Speed] + speed, _stats[StatsType.Speed], max);
    }

    public void UpJump(int jump)
    {
        int max = StatsType.Jump.GetMaxValue();
        _stats[StatsType.Jump] = Math.Clamp(_stats[StatsType.Jump] + jump, _stats[StatsType.Jump], max);
    }

    public void Damage(int attack, Character aggressor)
    {
        _stats[StatsType.Hp] -= attack;
        if (_stats[StatsType.Hp] <= 0)
        {
            Vanish();
        }
    }
}