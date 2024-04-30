using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Token
{
    [SerializeField]
    [Header("ユニークID")]
    private string _id;

    [SerializeField]
    [Header("最大HP(初期HP)")]
    private int _hp;

    public int hp
    {
        get { return _hp; }
    }

    [SerializeField]
    [Header("知覚能力")]
    private int _sense;

    public int sense
    {
        get { return _sense; }
    }

    [SerializeField]
    [Header("力の強さ")]
    private int _strength;

    public int strength
    {
        get { return _strength; }
    }

    [SerializeField]
    [Header("徒歩スピード")]
    private int _speed;

    public int speed
    {
        get { return _speed; }
    }

    [SerializeField]
    [Header("ジャンプ力")]
    private int _jump;

    public int jump
    {
        get { return _jump; }
    }

    // 状態.
    public enum eOrientation
    {
        East,
        West,
        South,
        North,
    }

    private eOrientation _orientation = eOrientation.South;

    public eOrientation orientation
    {
        get { return _orientation; }
    }

    private Animator animator;

    private void Awake()
    {
        //インスタンス生成時にuuidを設定する
        Guid guid = Guid.NewGuid();
        _id = guid.ToString();
    }

    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();

        _hp = StatusType.Hp.GetUpInitialValue();
        _sense = StatusType.Sense.GetUpInitialValue();
        _strength = StatusType.Strength.GetUpInitialValue();
        _speed = StatusType.Speed.GetUpInitialValue();
        _jump = StatusType.Jump.GetUpInitialValue();
    }

    public void SetOrientation(eOrientation orientation)
    {
        if (_orientation == orientation) return;
        _orientation = orientation;
        switch (_orientation)
        {
            case eOrientation.East:
                animator.SetTrigger("isRight");
                break;

            case eOrientation.West:
                animator.SetTrigger("isLeft");
                break;

            case eOrientation.South:
                animator.SetTrigger("isDown");
                break;

            case eOrientation.North:
                animator.SetTrigger("isUp");
                break;

            default:
                animator.SetTrigger("isUp");
                break;
        }
    }

    public void UpHp(int hp)
    {
        int max = StatusType.Hp.GetMaxValue();
        _hp = Math.Clamp(_hp + hp, _hp, max);
    }

    public void UpSense(int sense)
    {
        int max = StatusType.Sense.GetMaxValue();
        _sense = Math.Clamp(_sense + sense, _sense, max);
    }

    public void UpStrength(int strength)
    {
        int max = StatusType.Strength.GetMaxValue();
        _strength = Math.Clamp(_strength + strength, _strength, max);
    }

    public void UpSpeed(int speed)
    {
        int max = StatusType.Speed.GetMaxValue();
        _speed = Math.Clamp(_speed + speed, _speed, max);
    }

    public void UpJump(int jump)
    {
        int max = StatusType.Jump.GetMaxValue();
        _jump = Math.Clamp(_jump + jump, _jump, max);
    }

    public void Damage(int attack, Character aggressor)
    {
        _hp -= attack;
        if (_hp <= 0)
        {
            Vanish();
        }
    }
}