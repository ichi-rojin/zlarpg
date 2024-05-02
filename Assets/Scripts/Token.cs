using System;
using UnityEngine;

public class Token : MonoBehaviour
{
    [SerializeField]
    [Header("ユニークID")]
    private string _uuid;

    // 位置
    [SerializeField]
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
        //インスタンス生成時にuuidを設定する
        Guid guid = Guid.NewGuid();
        _uuid = guid.ToString();
    }

    public void Vanish()
    {
        Destroy(this.gameObject);
    }
}