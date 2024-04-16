using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Reflection;
using System;

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

    private void CreateSenseArea()
    {
        _sensed.Clear();
        for (int p1 = 0; p1 <= _character.sense; p1++)
        {
            for (int p2 = -1 * p1; p2 <= (p1 - 1) * 2 + 1; p2++)
            {
                if (_character.orientation == Character.eOrientation.East)
                {
                    _sensed.Add(new Vector2Int(_character.pos.x + p1, _character.pos.y + p2));
                }
                if (_character.orientation == Character.eOrientation.West)
                {
                    _sensed.Add(new Vector2Int(_character.pos.x - p1, _character.pos.y - p2));
                }
                if (_character.orientation == Character.eOrientation.South)
                {
                    _sensed.Add(new Vector2Int(_character.pos.x + p2, _character.pos.y + p1));
                }
                if (_character.orientation == Character.eOrientation.North)
                {
                    _sensed.Add(new Vector2Int(_character.pos.x + p2, _character.pos.y - p1));
                }
            }
        }
    }

    private Item FindItem()
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
            }
        }
        if (findedItems.Count > 0)
        {
            var random = new System.Random();
            var value = findedItems.GetRandom(random);
            return value;
        }
        return null;
    }

    private IEnumerator Move()
    {
        foreach (var p in _route)
        {
            _transform.DOMove(
                _mapManager.GetWorldPositionFromTile(p.x, p.y),
                0.2f
            ).SetEase(Ease.Linear);

            SetOrientation(_character.pos, p);

            _character.SetPos(p);
            yield return new WaitForSeconds(0.2f);

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
                _target = FindItem();
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
        foreach (var provide in provides)
        {
            Type t = _character.GetType();
            MethodInfo mi = t.GetMethod(provide.Key);
            object o = mi.Invoke(_character, new object[] { provide.Value });
        }
    }
}