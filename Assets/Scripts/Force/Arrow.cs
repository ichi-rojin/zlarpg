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
        ).SetEase(Ease.Linear);
        yield return new WaitForSeconds(_duration);

        if (CheckLifespan())
        {
            Vanish();
            yield break;
        }
        StartCoroutine("Exec");
        yield break;
    }
}