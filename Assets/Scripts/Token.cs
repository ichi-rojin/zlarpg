using System;
using UnityEngine;
using DG.Tweening;

public class Token : MonoBehaviour
{
    internal Transform _transform;
    internal GameObject _managers;
    internal MapManager _mapManager;
    internal float _tileSize; //タイルサイズ

    [SerializeField]
    [Header("ユニークID")]
    private string _uuid;

    public string uuid
    {
        get { return _uuid; }
    }

    // 位置
    [SerializeField]
    [Header("位置情報")]
    private Vector2Int _pos;

    public Vector2Int pos
    {
        get { return _pos; }
    }

    public void SetPos(int x, int y)
    {
        _pos.x = x;
        _pos.y = y;
    }

    public void SetPos(Vector2Int pos)
    {
        SetPos(pos.x, pos.y);
    }

    private void Awake()
    {
        //インスタンス生成時にuuidを設定する
        Guid guid = Guid.NewGuid();
        _uuid = guid.ToString();

        _managers = GameObject.Find("Managers");
        _mapManager = _managers.GetComponent<MapManager>();
        _tileSize = _mapManager.tileSize;
        _transform = this.gameObject.transform;
    }

    private void MovePosition(Vector2 coord, Vector2Int p, float duration)
    {
        _transform.DOMove(
            coord,
            duration
        ).SetEase(Ease.Linear);

        SetPos(p);
    }

    public void MovePosition(Vector2Int p, float duration)
    {
        var coord = _mapManager.GetWorldPositionFromTile(p.x, p.y);
        MovePosition(coord, p, duration);
    }

    public void Vanish(float sec)
    {
        Destroy(this.gameObject, sec);
    }

    public void Vanish()
    {
        Destroy(this.gameObject);
    }
}