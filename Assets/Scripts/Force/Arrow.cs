using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : BaseForce
{
    // ターゲット
    public Character target;

    private float _duration;
    private float _angle;

    // Start is called before the first frame update
    private void Start()
    {
        //進行方向
        Vector2 forward = target.pos - pos;
        //角度に変換
        _angle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, _angle);

        _duration = _mapManager.CalcDurationBySpeed(_stats[ForceType.Speed]);
        StartCoroutine("Exec");
    }

    // Update is called once per frame
    private IEnumerator Exec()
    {
        var a = _angle < 0 ? Mathf.Abs(_angle) : 360 - _angle;
        var x = Mathf.Cos(a * Mathf.PI / 180f);
        var y = -1 * Mathf.Sin(a * Mathf.PI / 180f);
        Debug.Log("pos " + pos + " a " + a + " angle " + _angle + " x " + x + " y " + y);
        MovePosition(target.pos, _duration);
        yield return new WaitForSeconds(_duration);
        yield break;
    }
}