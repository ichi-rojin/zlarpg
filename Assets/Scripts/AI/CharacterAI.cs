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

    public int[,] costMap
    {
        get { return _costMap; }
    }

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

        makeCostMap();

        yield return new WaitForSeconds(0.1f);

        // プレイヤーを移動させる.
        SetRoute(GetDestination());
        _state = eState.Walk;
        StartCoroutine("Move");
    }

    private int getMapTipCost(char mapType)
    {
        switch (mapType)
        {
            case 'g':
                return 1;
                break;

            case 'w':
                return -1;//壁はマイナス値を入れて、計算対象外とする
                break;

            case 's':
                return 5;
                break;
        }
        return 0;
    }

    private void makeCostMap()
    {
        for (int i = 0, size_i = _map.GetLength(0); i < size_i; i++)
        {
            for (int j = 0, size_j = _map.GetLength(1); j < size_j; j++)
            {
                _costMap[i, j] = getMapTipCost(_map[i, j]);
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
            _sensed = GetPerceptionCoords(1, 1, true);
            senseNext1 = _character.GetNormalizePosition(0, -1);
            senseNext2 = _character.GetNormalizePosition(0, 1);
        }
        if (_character.orientation == Character.eOrientation.West)
        {
            _sensed = GetPerceptionCoords(-1, -1, true);
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
            _sensed = GetPerceptionCoords(-1, -1, false);
            senseNext1 = _character.GetNormalizePosition(-1, 0);
            senseNext2 = _character.GetNormalizePosition(1, 0);
        }
        //周囲を知覚範囲に含める
        if (senseNext1 != null) _sensed.Add(senseNext1);
        if (senseNext2 != null) _sensed.Add(senseNext2);
        _sensed.Add(_character.pos);

        ShowSensedArea();
    }

    private void ShowSensedArea()
    {
        GameObject[,] mapTips = _mapManager.mapTips;
        for (var d1 = 0; d1 < mapTips.GetLength(0); d1++)
        {
            for (var d2 = 0; d2 < mapTips.GetLength(1); d2++)
            {
                var p = new Vector2Int(d1, d2);
                var findedMapTip = GetMapTip(p);
                var mapSpRdr = findedMapTip.GetComponent<SpriteRenderer>();
                if (_sensed.Contains(p))
                {
                    mapSpRdr.color = new Color(0f, 255f, 255f);
                }
                else
                {
                    mapSpRdr.color = new Color(255f, 255f, 255f);
                }
            }
        }
    }

    private Map GetMapTip(Vector2Int p)
    {
        GameObject[,] mapTips = _mapManager.mapTips;
        var mapTip = mapTips[p.x, p.y];
        var map = mapTip.GetComponent<Map>();
        return map;
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
        var findItems = _findMap.GetCoordByValue(1);
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
                _costMap[item.pos.x, item.pos.y] = getMapTipCost(_map[item.pos.x, item.pos.y]);
            }
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
        var i = 0;
        foreach (var p in _route)
        {
            var findedMapTip = GetMapTip(p);
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

            SetOrientation(_character.pos, p);
            CreateSenseArea();

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