using System;
using UnityEngine;
using DG.Tweening;

public class Token : MonoBehaviour
{
    private Transform _transform;
    private GameObject _managers;
    private MapManager _mapManager;

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

    public void SetPos(Vector2Int pos)
    {
        _pos = pos;
    }

    private void Awake()
    {
        //インスタンス生成時にuuidを設定する
        Guid guid = Guid.NewGuid();
        _uuid = guid.ToString();

        _managers = GameObject.Find("Managers");
        _mapManager = _managers.GetComponent<MapManager>();
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

    public void Vanish()
    {
        Destroy(this.gameObject);
    }
}