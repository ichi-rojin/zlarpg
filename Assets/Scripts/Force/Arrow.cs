using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;
using DG.Tweening;

public class Arrow : BaseForce
{
    // Start is called before the first frame update
    private void Start()
    {
        SetRotate();
        SetAngle();
        SetDuration();
        StartCoroutine("Exec");
    }

    protected void FixedUpdate()
    {
        CountUpAliveTime();
    }

    // Update is called once per frame
    private IEnumerator Exec()
    {
        if (_transform == false)
        {
            yield break;
        }
        var n = Quaternion.Euler(0, 0, _angle) * new Vector2(0, _tileSize);
        var coord = transform.position - n;
        _transform.DOMove(
            coord,
            _duration
        )
            .SetEase(Ease.Linear)
            .SetLink(this.gameObject);
        yield return new WaitForSeconds(_duration);

        SetPos(_mapManager.GetPosByCoord(_transform.position));
        CheckAttack();

        if (CheckLifespan())
        {
            Vanish();
            yield break;
        }
        StartCoroutine("Exec");
        yield break;
    }

    private int GetAtk()
    {
        var pow = _stats[ForceType.Physical];
        var spd = _stats[ForceType.Speed] / 10;
        return pow * spd;
    }

    private void CheckAttack()
    {
        List<Character> charas = new List<Character>();
        _characters.GetComponentsInChildren(charas);
        foreach (var chara in charas)
        {
            if (chara.pos == pos)
            {
                var atk = GetAtk();
                AttackTarget(atk, chara);
                Vanish();
            }
        }
    }
}