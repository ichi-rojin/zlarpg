using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Token
{
    [SerializeField]
    [Header("�ő�HP(����HP)")]
    private int _hp; //HitPoint

    public int hp
    {
        get { return _hp; }
    }

    [SerializeField]
    [Header("�m�o�\��")]
    private int _sense;

    public int sense
    {
        get { return _sense; }
    }

    [SerializeField]
    [Header("�͂̋���")]
    private int _strength;

    public int strength
    {
        get { return _strength; }
    }

    [SerializeField]
    [Header("�k���X�s�[�h")]
    private int _speed;

    public int speed
    {
        get { return _speed; }
    }

    // ���.
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

    // Start is called before the first frame update
    private void Start()
    {
        _hp = StatusType.Hp.GetUpInitialValue();
        _sense = StatusType.Sense.GetUpInitialValue();
        _strength = StatusType.Strength.GetUpInitialValue();
        _speed = StatusType.Speed.GetUpInitialValue();
    }

    public void SetOrientation(eOrientation orientation)
    {
        _orientation = orientation;
    }

    public void UpHp(int hp)
    {
        int max = StatusType.Hp.GetMaxValue();
        if (_hp + hp > max)
        {
            _hp = max;
            return;
        }
        _hp += hp;
    }

    public void UpSense(int sense)
    {
        int max = StatusType.Sense.GetMaxValue();
        if (_sense + sense > max)
        {
            _sense = max;
            return;
        }
        _sense += sense;
    }

    public void UpStrength(int strength)
    {
        int max = StatusType.Strength.GetMaxValue();
        if (_strength + strength > max)
        {
            _strength = max;
            return;
        }
        _strength += strength;
    }

    public void UpSpeed(int speed)
    {
        int max = StatusType.Speed.GetMaxValue();
        if (_speed + speed > max)
        {
            _speed = max;
            return;
        }
        _speed += speed;
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