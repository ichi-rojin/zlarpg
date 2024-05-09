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

    public void Damage(int attack, Character aggressor)
    {
        _stats[StatsType.Hp] -= attack;
        if (_stats[StatsType.Hp] <= 0)
        {
            Vanish();
        }
    }
}