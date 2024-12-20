using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using static UnityEditor.PlayerSettings;

public class CharacterAI : MonoBehaviour
{
    // 状態.
    private enum eState
    {
        Search, // 検索中.
        Walk, // 移動中.
        Find, // 発見.
        Battle, // 戦闘中.
        BattleMove, // 戦闘中.
        Escape, // 逃走中は敵を検索せずに脅威マップに従って逃走する。
    }

    [SerializeField]
    private eState _state = eState.Search;

    private Character _character;

    private List<Vector2Int> _route;

    public List<Vector2Int> route
    {
        get { return _route; }
    }

    private GameObject _managers;
    private GameObject _itemsParent;//アイテムの親ゲームオブジェクト
    private GameObject _charasParent;//キャラクターの親ゲームオブジェクト
    private MapManager _mapManager;
    private char[,] _map;
    private int[,] _costMap;

    public int[,] costMap
    {
        get { return _costMap; }
    }

    private int[,] _findMap;//発見物メモリ

    private float[,] _influenceMap;

    public float[,] influenceMap
    {
        get { return _influenceMap; }
    }

    private List<Vector2Int> _sensed;

    public List<Vector2Int> sensed
    {
        get { return _sensed; }
    }

    [SerializeField]
    [Header("戦意")]
    private int _spirit = 0;

    [SerializeField]
    private List<Character> _enemies;

    private Item _target = null;

    private Character _targetEnemy = null;

    [SerializeField]
    private BaseForceSpawner _selectedForceSpawner = null;

    private IEnumerator TacticsEra = null;
    private IEnumerator BattleEra = null;
    private IEnumerator BattleMoveEra = null;

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        _managers = GameObject.Find("Managers");
        _mapManager = _managers.GetComponent<MapManager>();
        _itemsParent = GameObject.Find("ItemBlocks");
        _charasParent = GameObject.Find("Characters");
        _character = this.gameObject.GetComponent<Character>();

        _map = _mapManager.map;
        _costMap = new int[_map.GetLength(0), _map.GetLength(1)];
        _findMap = new int[_map.GetLength(0), _map.GetLength(1)];
        _influenceMap = new float[_map.GetLength(0), _map.GetLength(1)];

        _sensed = new List<Vector2Int>();
        _enemies = new List<Character>();

        MakeCostMap();
        _influenceMap.Fill(0);//0で埋める

        yield return new WaitForSeconds(0.1f);

        // プレイヤーを移動させる.
        SetRoute(GetDestination(), _costMap);
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

    private void SetOrientation(Vector2Int forward)
    {
        if (forward == Vector2Int.left)
        {
            _character.SetOrientation(Character.eOrientation.East);
        }
        if (forward == Vector2Int.right)
        {
            _character.SetOrientation(Character.eOrientation.West);
        }
        if (forward == Vector2Int.up)
        {
            _character.SetOrientation(Character.eOrientation.North);
        }
        if (forward == Vector2Int.down)
        {
            _character.SetOrientation(Character.eOrientation.South);
        }
    }

    private void SetOrientation(Vector2 forward)
    {
        if (Math.Abs(forward.x) >= Math.Abs(forward.y))
        {
            if (forward.x >= 0)
            {
                SetOrientation(Vector2Int.right);
            }
            else
            {
                SetOrientation(Vector2Int.left);
            }
        }
        else
        {
            if (forward.y >= 0)
            {
                SetOrientation(Vector2Int.up);
            }
            else
            {
                SetOrientation(Vector2Int.down);
            }
        }
    }

    private void SetOrientation(Vector2Int prev, Vector2Int next)
    {
        Vector2Int forward = prev - next;
        SetOrientation(forward);
    }

    private void SetRoute(Vector2Int destination, int[,] map)
    {
        Vector2Int startPos = _character.pos;
        Vector2Int endPos = destination;

        _route = AStar.Serch(startPos, endPos, map);
    }

    //視界
    private List<Vector2Int> GetPerceptionCoords(
        int sign1,
        int sign2,
        bool reverse,
        Vector2Int origin,
        int sense
    )
    {
        List<Vector2Int> senses = new List<Vector2Int>();
        var p1 = sense;
        for (int p2 = -1 * p1; p2 < (p1 - 1) + 2; p2++)
        {
            Vector2Int coord = new Vector2Int();
            if (reverse != false)
            {
                coord = new Vector2Int(
                    origin.x + p1 * sign1,
                    origin.y + p2 * sign2
                );
            }
            else
            {
                coord = new Vector2Int(
                    origin.x + p2 * sign2,
                    origin.y + p1 * sign1
                );
            }

            foreach (var sightCell in _mapManager.GetSightLines(origin, coord, p1, reverse, (p) =>
            {
                if (
                    p.x < 0
                    ||
                    p.y < 0
                    ||
                    _costMap[p.x, p.y] < 0
                )
                {
                    return true;
                }
                return false;
            }))
            {
                senses.Add(sightCell);
            }
        }
        return senses;
    }

    private List<Vector2Int> GetSenseArea(
        Character.eOrientation orientation,
        Vector2Int origin,
        int sense
    )
    {
        var sensedArea = new List<Vector2Int>();
        Vector2Int senseNext1 = new Vector2Int();
        Vector2Int senseNext2 = new Vector2Int();

        if (orientation == Character.eOrientation.East)
        {
            sensedArea = GetPerceptionCoords(1, 1, true, origin, sense);
            senseNext1 = _mapManager.GetNormalizePosition(origin, 0, -1);
            senseNext2 = _mapManager.GetNormalizePosition(origin, 0, 1);
        }
        if (orientation == Character.eOrientation.West)
        {
            sensedArea = GetPerceptionCoords(-1, -1, true, origin, sense);
            senseNext1 = _mapManager.GetNormalizePosition(origin, 0, -1);
            senseNext2 = _mapManager.GetNormalizePosition(origin, 0, 1);
        }
        if (orientation == Character.eOrientation.South)
        {
            sensedArea = GetPerceptionCoords(1, 1, false, origin, sense);
            senseNext1 = _mapManager.GetNormalizePosition(origin, -1, 0);
            senseNext2 = _mapManager.GetNormalizePosition(origin, 1, 0);
        }
        if (orientation == Character.eOrientation.North)
        {
            sensedArea = GetPerceptionCoords(-1, -1, false, origin, sense);
            senseNext1 = _mapManager.GetNormalizePosition(origin, -1, 0);
            senseNext2 = _mapManager.GetNormalizePosition(origin, 1, 0);
        }
        //周囲を知覚範囲に含める
        if (_costMap[senseNext1.x, senseNext1.y] > 0) sensedArea.Add(senseNext1);
        if (_costMap[senseNext2.x, senseNext2.y] > 0) sensedArea.Add(senseNext2);
        sensedArea.Add(origin);
        return sensedArea;
    }

    private void CreateSenseArea()
    {
        _sensed = GetSenseArea(
            _character.orientation,
            _character.pos,
            _character.stats[StatsType.Sense]
        );
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

    private void FindCharas()
    {
        _enemies.Clear();

        List<Character> charas = new List<Character>();
        _charasParent.GetComponentsInChildren(charas);
        foreach (var chara in charas)
        {
            if (_character == chara)
            {
                continue;
            }
            if (_sensed.Contains(chara.pos))
            {
                _enemies.Add(chara.GetComponent<Character>());
            }
        }
        if (_state != eState.Escape)
        {
            if (_enemies.Count > 0)
            {
                _state = eState.Battle;
            }
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
            if (_costMap[item.pos.x, item.pos.y] > _character.stats[StatsType.Jump])
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

    private void TransitionStatusFromBattle()
    {
        _targetEnemy = null;
        var findedItems = GetFindItems();

        if (findedItems.Count > 0)
        {
            _target = findedItems.GetNearestItem(_character.pos);
        }
        if (_target)
        {
            SetRoute(_target.pos, _costMap);
            _state = eState.Find;
        }
        else
        {
            SetRoute(GetDestination(), _costMap);
            _state = eState.Walk;
        }
    }

    private float CalcDurationByThroughput()
    {
        float throughput = _character.stats[StatsType.Throughput];
        float max = StatsType.Throughput.GetMaxValue();
        float min = 10;
        float normalize = 1 - ((throughput - min) / (max - min));
        return 0.5f + normalize * 4;
    }

    private void SetTactics()
    {
        //【TODO】戦術的に_targetEnemyを選ぶ
        _targetEnemy = _enemies.GetRandom();
        if (_targetEnemy == null) return;

        var distant = Vector2.Distance(_character.pos, _targetEnemy.pos);
        var maxRange = 0;
        // distantに一番近いforceSpawnerを取得
        foreach (BaseForceSpawner spawner in _character.forceSpawners)
        {
            var range = spawner.stats.Range;
            if (distant > range) continue;// 射程外
            if (range > maxRange)
            {
                maxRange = range;
                _selectedForceSpawner = spawner;
            }
        }

        //【TODO】distant以上ターゲットと離れている場合の移動処理
        //【TODO】相手より射程が優っている場合の回避処理
    }

    //攻撃
    public void Attack()
    {
        if (_selectedForceSpawner == null) return;
        var pos = _character.pos;
        var coord = _mapManager.GetWorldPositionFromTile(pos.x, pos.y);
        _selectedForceSpawner.Action(coord, _targetEnemy);
    }

    //戦闘移動
    private IEnumerator BattleMove()
    {
        float duration = _mapManager.CalcDurationBySpeed(_character.stats[StatsType.Speed]);

        var combedInflCostMaps = GetCombedInflCostMaps();
        var pos = _character.pos;
        var movableList = new List<Vector2Int> {
            pos,
            pos + Vector2Int.left,
            pos + Vector2Int.up,
            pos + Vector2Int.right,
            pos + Vector2Int.down,
        }.FindAll(el => combedInflCostMaps[el.x, el.y] > 0);
        //射程付近に移動する
        Vector2Int nextPos;
        float range = 0;
        if (_selectedForceSpawner)
        {
            range = _selectedForceSpawner.stats.Range;
        } else
        {
            if (_character.forceSpawners != null)
            {
                range = _character.forceSpawners.OrderBy(v =>
                {
                    return v.stats.Range;
                }).Take(1).First().stats.Range;
            }
        }
        if (_targetEnemy)
        {
            var nearestList = movableList.OrderBy(v =>
            {
                var distant = Vector2.Distance(v, _targetEnemy.pos);
                return Mathf.Abs(range - distant);
            }).Take(3);
            nextPos = nearestList.GetRandom();
        } else
        {
            nextPos = movableList.GetRandom();
        }

        Vector2Int dir;
        if (_targetEnemy == null)
        {
            dir = nextPos;
        }
        else
        {
            dir = _targetEnemy.pos;
        }
        Vector2 forward = _character.pos - dir;
        SetOrientation(forward);
        CreateSenseArea();

        _character.MovePosition(nextPos, duration);
        yield return new WaitForSeconds(duration);
    }

    private IEnumerator Tactics()
    {
        // 敵存在チェック
        if (_enemies.Count < 1)
        {
            StopCoroutine(TacticsEra);
            yield break;
        }
        //戦術決定
        SetTactics();

        //行動
        Attack();

        float duration = CalcDurationByThroughput();
        yield return new WaitForSeconds(duration);
        TacticsEra = Tactics();
        StartCoroutine(TacticsEra);
    }

    private IEnumerator Battle()
    {
        //知覚エリア内のキャラクターを検索
        FindCharas();
        // 敵存在チェック
        if (_enemies.Count < 1)
        {
            TransitionStatusFromBattle();

            _selectedForceSpawner = null;
            StopCoroutine(BattleEra);
            BattleEra = null;
            StopCoroutine(TacticsEra);
            TacticsEra = null;

            StartCoroutine("Move");
            yield break;
        }

        _spirit -= 1;
        if (_spirit <= 0)
        {
            _state = eState.Escape;

            _selectedForceSpawner = null;
            StopCoroutine(BattleEra);
            BattleEra = null;
            StopCoroutine(TacticsEra);
            TacticsEra = null;

            Escape();
            yield break;
        }

        //移動
        if (
            _selectedForceSpawner == null
            ||
            new System.Random().Next(6) <= 5
        )
        {
            _state = eState.BattleMove;
            BattleMoveEra = BattleMove();
            yield return StartCoroutine(BattleMoveEra);
        }

        BattleEra = Battle();
        StartCoroutine(BattleEra);
    }

    private int GetForecastSense(Character chara)
    {
        //【TODO】自分のSenseを基に予想、知能が高いほど相手のSenseに近い数字を返す。
        return (_character.stats[StatsType.Sense] + chara.stats.Sense) / 2;
    }

    private int[,] GetCombedInflCostMaps()
    {
        var map = new int[_map.GetLength(0), _map.GetLength(1)];

        for (var d1 = 0; d1 < map.GetLength(0); d1++)
        {
            for (var d2 = 0; d2 < map.GetLength(1); d2++)
            {
                map[d1, d2] = _costMap[d1, d2] + (int)(_influenceMap[d1, d2] * 100);
            }
        }
        return map;
    }

    private void Escape()
    {
        _influenceMap.Fill(0);//脅威マップ初期化
        foreach (var chara in _enemies)
        {
            var sense = GetForecastSense(chara);
            var sensed = GetSenseArea(chara.orientation, chara.pos, sense);
            var maxCost = sense + sense;
            foreach (var p in sensed)
            {
                _influenceMap[p.x, p.y] = maxCost - (
                    Math.Abs(chara.pos.x - p.x)
                    + Math.Abs(chara.pos.y - p.y)
                );
            }
        }
        //0から1で正規化
        _influenceMap.MinMaxNormalization();
        var combedInflCostMaps = GetCombedInflCostMaps();
        int cnt = 0;
        while (cnt < 10)
        {
            var destination = GetDestination();
            if (combedInflCostMaps[destination.x, destination.y] <= 2)
            {
                SetRoute(destination, combedInflCostMaps);
                break;
            }
            cnt++;
        }
        StartCoroutine("Move");
    }

    private IEnumerator Move()
    {
        float duration = _mapManager.CalcDurationBySpeed(_character.stats[StatsType.Speed]);
        var i = 0;
        foreach (var p in _route)
        {
            SetOrientation(_character.pos, p);
            CreateSenseArea();

            var findedMapTip = _mapManager.GetMapTip(p);
            var AdvancePermission = findedMapTip.GetAdvancePermission(_character);
            var goal = _route.LastOrDefault();
            if (AdvancePermission == false)
            {
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

            _character.MovePosition(p, duration);
            yield return new WaitForSeconds(duration);

            //目的地に着いたら脅威をリセット
            if (
                _state != eState.Escape
                &&
                goal == p
            )
            {
                ResetThreat();
            }

            if (_target)
            {
                //射程内のターゲットに対してアクション
                if (_character.pos == _target.pos)
                {
                    Gain(_target);
                    _state = eState.Search;
                    break;
                }
            }
            else
            {
                if (_state == eState.Find)
                {
                    //発見物がなくなっていればルート再検索
                    _state = eState.Search;
                    break;
                }
                //知覚エリア内のキャラクターを検索
                FindCharas();
                if (_state == eState.Battle)
                {
                    _spirit = 200;
                    BattleEra = Battle();
                    TacticsEra = Tactics();
                    StartCoroutine(BattleEra);
                    StartCoroutine(TacticsEra);
                    yield break;
                }
                //知覚エリア内のターゲットを検索
                FindItems();
                var findedItems = GetFindItems();

                if (findedItems.Count > 0)
                {
                    _target = findedItems.GetNearestItem(_character.pos);
                }
                if (_target)
                {
                    SetRoute(_target.pos, _costMap);
                    _state = eState.Find;
                    break;
                }
            }
            i++;
        }

        yield return new WaitForSeconds(0.01f);

        if (_state != eState.Find)
        {
            SetRoute(GetDestination(), _costMap);
        }
        _state = eState.Walk;
        StartCoroutine("Move");
        yield break;
    }

    private void ResetThreat()
    {
        _influenceMap.Fill(0);//脅威マップ初期化
    }

    private void Gain(Item target)
    {
        Dictionary<StatsType, int> provides = target.Provide();
        foreach (var provide in provides)
        {
            _character.maxStats.UpValue(provide.Key, provide.Value);
            _character.stats[provide.Key] = _character.maxStats[provide.Key];
        }
        target.Vanish();
        _findMap[target.pos.x, target.pos.y] = 0;//記憶から削除
    }

    //再度思考
    public void Blocked()
    {
        _target = null;
        _route.Clear();
        if (_state != eState.Escape)
        {
            _state = eState.Search;
        }
    }
}