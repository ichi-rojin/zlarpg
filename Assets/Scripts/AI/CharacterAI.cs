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

    private Character _character;
    private Transform _transform;

    private List<Vector2Int> _route;

    private GameObject _managers;
    private GameObject _itemsParent;//アイテムの親ゲームオブジェクト
    private MapManager _mapManager;
    private char[,] _map;
    private int[,] _costMap;

    public int[,] costMap
    {
        get { return _costMap; }
    }

    private int[,] _findMap;//発見物メモリ

    private List<Vector2Int> _sensed = new List<Vector2Int>();

    public List<Vector2Int> sensed
    {
        get { return _sensed; }
    }

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

        MakeCostMap();

        yield return new WaitForSeconds(0.1f);

        // プレイヤーを移動させる.
        SetRoute(GetDestination());
        _state = eState.Walk;
        StartCoroutine("Move");
    }

    private int GetMapTipCost(char mapType)
    {
        switch (mapType)
        {
            case 'g':
                return 1;

            case 'w':
                return -1;//壁はマイナス値を入れて、計算対象外とする

            case 's':
                return 5;
        }
        return 0;
    }

    private void MakeCostMap()
    {
        for (int i = 0, size_i = _map.GetLength(0); i < size_i; i++)
        {
            for (int j = 0, size_j = _map.GetLength(1); j < size_j; j++)
            {
                _costMap[i, j] = GetMapTipCost(_map[i, j]);
            }
        }
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

        _route = AStar.Serch(startPos, endPos, _costMap);
    }

    //視線
    private List<Vector2Int> GetSightLines(Vector2Int start, Vector2Int end, int dist, bool horizontal)
    {
        List<Vector2Int> SightList = new List<Vector2Int>();
        if (horizontal)
        {
            if (end.y < 0)
            {
                return SightList;
            }
        }
        else
        {
            if (end.x < 0)
            {
                return SightList;
            }
        }
        int step = 1;
        float dx = end.x - start.x;
        float dy = end.y - start.y;
        float t = dx != 0 ? dx / dy : 0;
        if (horizontal)
        {
            t = dy != 0 ? dy / dx : 0;
        }
        int i = 0;
        float nx = start.x;
        float ny = start.y;
        while (i < dist)
        {
            i += step;
            if (horizontal)
            {
                nx += step * Math.Sign(dx);
                ny += t;
            }
            else
            {
                nx += t;
                ny += step * Math.Sign(dy);
            }
            int nxi = (int)Math.Round(nx);
            int nyi = (int)Math.Round(ny);
            Vector2Int p = _mapManager.GetNormalizePosition(new Vector2Int(nxi, nyi), 0, 0);
            if (
                p.x < 0
                ||
                p.y < 0
                ||
                _costMap[p.x, p.y] < 0
            )
            {
                break;
            }
            if (SightList.Contains(p) == false)
            {
                SightList.Add(p);
            }
        }
        return SightList;
    }

    //視界
    private List<Vector2Int> GetPerceptionCoords(int sign1, int sign2, bool reverse)
    {
        List<Vector2Int> senses = new List<Vector2Int>();
        var p1 = _character.sense;
        for (int p2 = -1 * p1; p2 < (p1 - 1) + 2; p2++)
        {
            Vector2Int coord = new Vector2Int();
            if (reverse != false)
            {
                coord = new Vector2Int(
                    _character.pos.x + p1 * sign1,
                    _character.pos.y + p2 * sign2
                );
            }
            else
            {
                coord = new Vector2Int(
                    _character.pos.x + p2 * sign2
                    , _character.pos.y + p1 * sign1
                );
            }

            foreach (var sightCell in GetSightLines(_character.pos, coord, _character.sense, reverse))
            {
                senses.Add(sightCell);
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
            _sensed = GetPerceptionCoords(1, 1, true);
            senseNext1 = _mapManager.GetNormalizePosition(_character.pos, 0, -1);
            senseNext2 = _mapManager.GetNormalizePosition(_character.pos, 0, 1);
        }
        if (_character.orientation == Character.eOrientation.West)
        {
            _sensed = GetPerceptionCoords(-1, -1, true);
            senseNext1 = _mapManager.GetNormalizePosition(_character.pos, 0, -1);
            senseNext2 = _mapManager.GetNormalizePosition(_character.pos, 0, 1);
        }
        if (_character.orientation == Character.eOrientation.South)
        {
            _sensed = GetPerceptionCoords(1, 1, false);
            senseNext1 = _mapManager.GetNormalizePosition(_character.pos, -1, 0);
            senseNext2 = _mapManager.GetNormalizePosition(_character.pos, 1, 0);
        }
        if (_character.orientation == Character.eOrientation.North)
        {
            _sensed = GetPerceptionCoords(-1, -1, false);
            senseNext1 = _mapManager.GetNormalizePosition(_character.pos, -1, 0);
            senseNext2 = _mapManager.GetNormalizePosition(_character.pos, 1, 0);
        }
        //周囲を知覚範囲に含める
        if (_costMap[senseNext1.x, senseNext1.y] > 0) _sensed.Add(senseNext1);
        if (_costMap[senseNext2.x, senseNext2.y] > 0) _sensed.Add(senseNext2);
        _sensed.Add(_character.pos);
    }

    private void FindItems()
    {
        List<Item> items = new List<Item>();
        List<Item> findedItems = new List<Item>();
        _itemsParent.GetComponentsInChildren(items);
        foreach (var item in items)
        {
            if (_sensed.Contains(item.pos))
            {
                findedItems.Add(item);
            }
            else
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
        var findItems = _findMap.GetCoordListByValue(1);
        foreach (var item in findItems)
        {
            findedList.Add(new Vector2Int(item.X, item.Y));
        }
        foreach (var item in items)
        {
            if (_costMap[item.pos.x, item.pos.y] > _character.jump)
            {
                //コストが踏破力を上回れば無視する
                continue;
            }
            else
            {
                //コストが踏破力以下のときはマップコストを初期化する
                _costMap[item.pos.x, item.pos.y] = GetMapTipCost(_map[item.pos.x, item.pos.y]);
            }
            if (findedList.Contains(item.pos))
            {
                findedItems.Add(item);
            }
        }
        return findedItems;
    }

    private float CalcDurationBySpeed(int speed)
    {
        return _mapManager.tileSize / 100 / 4 * (3.0f - (float)speed / 10);
    }

    private IEnumerator Move()
    {
        float duration = CalcDurationBySpeed(_character.speed);
        var i = 0;
        foreach (var p in _route)
        {
            SetOrientation(_character.pos, p);
            CreateSenseArea();

            var findedMapTip = _mapManager.GetMapTip(p);
            var AdvancePermission = findedMapTip.GetAdvancePermission(_character);
            if (AdvancePermission == false)
            {
                var goal = _route.LastOrDefault();
                if (_findMap[goal.x, goal.y] > 0)
                {
                    //ルートの最終地点に発見物があれば、進行不可にする。
                    var unreachableRoute = _route.GetRange(i, _route.Count - i);
                    var urMax = _route.Max(el => _costMap[el.x, el.y]);
                    _costMap[goal.x, goal.y] = urMax;
                }
                Blocked();
                break;
            }

            _transform.DOMove(
                _mapManager.GetWorldPositionFromTile(p.x, p.y),
                duration
            ).SetEase(Ease.Linear);

            _character.SetPos(p);
            yield return new WaitForSeconds(duration);

            if (_target)
            {
                //射程内のターゲットに対してアクション
                if (_character.pos == _target.pos)
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
            i++;
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

    //再度思考
    public void Blocked()
    {
        _target = null;
        _route.Clear();
        _state = eState.Search;
    }
}