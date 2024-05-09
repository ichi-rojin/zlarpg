using System;
using UnityEngine;

public class Token : MonoBehaviour
{
    [SerializeField]
    [Header("���j�[�NID")]
    private string _uuid;

    public string uuid
    {
        get { return _uuid; }
    }

    // �ʒu
    [SerializeField]
    [Header("�ʒu���")]
    private Vector2Int _pos;

    public Vector2Int pos
    {
        get { return _pos; }
    }

    public void SetPos(Vector2Int pos)
    {
        _pos = pos;
    }

    private void Awake()
    {
        //�C���X�^���X��������uuid��ݒ肷��
        Guid guid = Guid.NewGuid();
        _uuid = guid.ToString();
    }

    public void Vanish()
    {
        Destroy(this.gameObject);
    }
}