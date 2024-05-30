using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : BaseForce
{
    // �^�[�Q�b�g
    public Character target;

    // Start is called before the first frame update
    private void Start()
    {
        //�i�s����
        Vector2Int forward = target.pos - pos;
        //�p�x�ɕϊ�
        float angle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    // Update is called once per frame
    private void Update()
    {
    }
}