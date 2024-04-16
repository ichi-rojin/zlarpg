using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Token
{
    [SerializeField]
    private int _hp; //HitPoint

    public int hp
    {
        get { return _hp; }
    }

    [SerializeField]
    private int _sense; //�m�o�\��

    public int sense
    {
        get { return _sense; }
    }

    [SerializeField]
    private int _attack; //�U���\��

    public int attack
    {
        get { return _attack; }
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
        _hp = Random.Range(100, 300);
        _sense = Random.Range(2, 10);
        _attack = Random.Range(5, 10);
    }

    public void SetOrientation(eOrientation orientation)
    {
        _orientation = orientation;
    }

    public void UpAttack(int attack)
    {
        _attack += attack;
    }

    public void UpHp(int hp)
    {
        _hp += hp;
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