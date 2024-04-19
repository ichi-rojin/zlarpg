using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Reflection;
using System;
using System.Linq;

public class CharacterAI : MonoBehaviour
{
    // 状態.
    private enum eState
    {
        Search, // 検索中.
        Walk, // 移動中.
        Find, // 発見.
    }

    private eState _state = eState.Search;

    private Character _character; //座標
    private Transform _transform; //座標

    private List<Vector2Int> _route;

    private GameObject _managers;
    private GameObject _itemsParent; //マップのゲームオブジェクト
    private MapManager _mapManager;
    private char[,] _map;
    private int[,] _costMap;
    private int[,] _findMap;//発見物メモリ

    private List<Vector2Int> _sensed = new List<Vector2Int>();

    private Item _target = null;

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        _managers = GameObject.Find("Managers");
        _mapManager = _managers.GetComponent<MapManager>();
        _itemsParent = GameObject.Find("ItemBlocks");
        _character = this.gameObject.GetComponent<Character>();
        _transform = this.gameObject.transform;

        _map = _mapManager.map;
        _costMap = new int[_map.GetLength(0), _map.GetLength(1)];
        _findMap = new int[_map.GetLength(0), _map.GetLength(1)];

        yield return new WaitForSeconds(0.1f);

        // プレイヤーを移動させる.
        SetRoute(GetDestination());
        _state = eState.Walk;
        StartCoroutine("Move");
    }

    private Vector2Int GetDestination()
    {
        return _mapManager.GetRandomCoord();
    }

    private void SetOrientation(Vector2Int prev, Vector2Int next)
    {
        if (
            prev.x == next.x
            &&
            prev.y == next.y
        ) return;

        if (
            prev.y == next.y
        )
        {
            if (prev.x < next.x)
            {
                _character.SetOrientation(Character.eOrientation.East);
            }
            else
            {
                _character.SetOrientation(Character.eOrientation.West);
            }
            return;
        }

        if (
            prev.x == next.x
        )
        {
            if (prev.y < next.y)
            {
                _character.SetOrientation(Character.eOrientation.South);
            }
            else
            {
                _character.SetOrientation(Character.eOrientation.North);
            }
            return;
        }
    }

    private void SetRoute(Vector2Int destination)
    {
        Vector2Int startPos = _character.pos;
        Vector2Int endPos = destination;
        AStar aStar = new AStar();

        for (int i = 0, size_i = _map.GetLength(0); i < size_i; i++)
        {
            for (int j = 0, size_j = _map.GetLength(1); j < size_j; j++)
            {
                var num = 0;
                switch (_map[i, j])
                {
                    case 'g':
                        num = 1;
                        break;

                    case 'w':
                        num = 1000;
                        break;

                    case 's':
                        num = 5;
                        break;
                }
                _costMap[i, j] = num;
            }
        }

        _route = aStar.Serch(startPos, endPos, _costMap);
    }

    private List<Vector2Int> GetPerceptionCoords(int sign1, int sign2, bool reverse)
    {
        List<Vector2Int> senses = new List<Vector2Int>();
        for (int p1 = 1; p1 <= _character.sense; p1++)
        {
            for (int p2 = -1 * p1; p2 < (p1 - 1) + 2; p2++)
            {
                Vector2Int coord = new Vector2Int();
                if (reverse != false)
                {
                    coord = _character.GetNormalizePosition(p1 * sign1, p2 * sign2);
                }
                else
                {
                    coord = _character.GetNormalizePosition(p2 * sign2, p1 * sign1);
                }
                if (coord != null) senses.Add(coord);
            }
        }
        return senses;
    }

    private void CreateSenseArea()
    {
        _sensed.Clear();
        Vector2Int senseNext1 = new Vector2Int();
        Vector2Int senseNext2 = new Vector2Int();

        if (_character.orientation == Character.eOrientation.East)
        {
            _sensed = GetPerceptionCoords(1, 1, false);
            senseNext1 = _character.GetNormalizePosition(0, -1);
            senseNext2 = _character.GetNormalizePosition(0, 1);
        }
        if (_character.orientation == Character.eOrientation.West)
        {
            _sensed = GetPerceptionCoords(-1, -1, false);
            senseNext1 = _character.GetNormalizePosition(0, -1);
            senseNext2 = _character.GetNormalizePosition(0, 1);
        }
        if (_character.orientation == Character.eOrientation.South)
        {
            _sensed = GetPerceptionCoords(1, 1, false);
            senseNext1 = _character.GetNormalizePosition(-1, 0);
            senseNext2 = _character.GetNormalizePosition(1, 0);
        }
        if (_character.orientation == Character.eOrientation.North)
        {
            _sensed = GetPerceptionCoords(1, -1, false);
            senseNext1 = _character.GetNormalizePosition(-1, 0);
            senseNext2 = _character.GetNormalizePosition(1, 0);
        }
        //周囲を知覚範囲に含める
        if (senseNext1 != null) _sensed.Add(senseNext1);
        if (senseNext2 != null) _sensed.Add(senseNext2);
        _sensed.Add(_character.pos);
    }

    private void FindItems()
    {
        CreateSenseArea();
        List<Item> items = new List<Item>();
        List<Item> findedItems = new List<Item>();
        _itemsParent.GetComponentsInChildren(items);
        foreach (var item in items)
        {
            if (_sensed.Contains(item.pos))
            {
                findedItems.Add(item);
            } else
            {
                //視界内の発見物がすでになくなっていたら記憶から消す
                _findMap[item.pos.x, item.pos.y] = 0;

            }
        }
        foreach (var find in findedItems)
        {
            _findMap[find.pos.x, find.pos.y] = 1;
        }
    }

    private List<Item> GetFindItems()
    {
        List<Item> items = new List<Item>();
        List<Vector2Int> findedList = new List<Vector2Int>();
        List<Item> findedItems = new List<Item>();
        _itemsParent.GetComponentsInChildren(items);
        var findItems = _findMap.GetCoordByValue(1);
        foreach (var item in findItems)
        {
            findedList.Add(new Vector2Int(item.X, item.Y));
        }
        foreach (var item in items)
        {
            if (findedList.Contains(item.pos))
            {
                findedItems.Add(item);
            }
        }
        return findedItems;
    }

    private float calcDurationBySpeed(int speed)
    {
        return _mapManager.tileSize / 100 / 4 * (3.0f - (float)speed / 10);
    }

    private IEnumerator Move()
    {
        float duration = calcDurationBySpeed(_character.speed);
        foreach (var p in _route)
        {
            _transform.DOMove(
                _mapManager.GetWorldPositionFromTile(p.x, p.y),
                duration
            ).SetEase(Ease.Linear);

            SetOrientation(_character.pos, p);

            _character.SetPos(p);
            yield return new WaitForSeconds(duration);

            if (_target)
            {
                //射程内のターゲットに対してアクション
                if (
                    _character.pos == _target.pos
                    ||
                    _character.GetNormalizePosition(1, 0) == _target.pos
                    ||
                    _character.GetNormalizePosition(0, 1) == _target.pos
                    ||
                    _character.GetNormalizePosition(-1, 0) == _target.pos
                    ||
                    _character.GetNormalizePosition(0, -1) == _target.pos
                )
                {
                    Action(_target);
                    _state = eState.Search;
                    break;
                }
            }
            else
            {
                if (_state == eState.Find)
                {
                    //発見物がなくなっていればルート再建策
                    _state = eState.Search;
                    break;
                }
                //知覚エリア内のターゲットを検索
                FindItems();
                var findedItems = GetFindItems();

                if (findedItems.Count > 0)
                {
                    _target = findedItems.GetRandom();
                }
                if (_target)
                {
                    SetRoute(_target.pos);
                    _state = eState.Find;
                    break;
                }
            }
        }

        yield return new WaitForSeconds(0.01f);
        if (_state != eState.Find)
        {
            SetRoute(GetDestination());
        }
        _state = eState.Walk;
        StartCoroutine("Move");
    }

    private void Action(Item target)
    {
        Dictionary<string, int> provides = target.Provide();
        target.Vanish();
        _findMap[target.pos.x, target.pos.y] = 0;//記憶から削除
        foreach (var provide in provides)
        {
            Type t = _character.GetType();
            MethodInfo mi = t.GetMethod(provide.Key);
            object o = mi.Invoke(_character, new object[] { provide.Value });
        }
    }
}