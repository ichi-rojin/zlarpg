using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class MapManager : MonoBehaviour
{
    public TextAsset textFile;

    [SerializeField]
    private GameObject
        _groundPrefab,
        _wallPrefab,
        _sectionLinePrefab
    ; //各種プレファブ

    [SerializeField]
    private Transform _mapBlocks; //マップのゲームオブジェクト

    private string[] _textData;
    private char[,] _map; //マップデータ

    // プロパティ
    public char[,] map
    {
        get { return _map; }
    }

    private GameObject[,] _mapTips; //マップチップ格納

    // プロパティ
    public GameObject[,] mapTips
    {
        get { return _mapTips; }
    }

    private int
        _rowLength,
        _colLength
    ; //マップの縦横サイズ

    private float _tileSize; //プレファブのサイズ

    public float tileSize
    {
        get { return _tileSize; }
    }

    private Vector2 _mapCenterPos;

    //原点を左上とする
    private Vector2 _origin;

    private void Start()
    {
        string textLines = textFile.text; // テキストの全体データの代入

        // 改行でデータを分割して配列に代入
        _textData = textLines.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        // 行数と列数の取得
        _colLength = _textData[0].ToCharArray().Length;
        _rowLength = _textData.Length;

        // ２次元配列の定義
        _map = new char[_colLength, _rowLength];//マップ作成

        _mapTips = new GameObject[_colLength, _rowLength];//マップチップ格納
        PlaceTiles();//プレファブを並べる処理
    }

    private void PlaceTiles()
    {
        _tileSize = _groundPrefab.GetComponent<Renderer>().bounds.size.x; //タイルサイズ取得
        _mapCenterPos = new Vector2(_colLength / 2 * _tileSize, (_rowLength / 2 + 1) * _tileSize);
        _origin = new Vector2(_colLength * _tileSize / 2, _rowLength * _tileSize / 2);
        if (_colLength % 2 == 0) _mapCenterPos.x -= _tileSize / 2;
        if (_rowLength % 2 == 0) _mapCenterPos.y -= _tileSize / 2;

        for (int y = _rowLength - 1; y >= 0; y--)//反転して読み込む
        {
            char[] tempWords = _textData[y].ToCharArray();

            for (int x = 0; x < _colLength; x++)
            {
                char tileType = tempWords[x]; //マップの種類取得
                Vector2 pos = GetWorldPositionFromTile(x, y);//座標を計算

                _map[x, y] = tileType;

                GameObject mapTip;

                mapTip = Instantiate(_groundPrefab, pos, Quaternion.Euler(0, 0, 0f), _mapBlocks);

                switch (tileType)
                {
                    case 'w':
                        mapTip = Instantiate(_wallPrefab, pos, Quaternion.Euler(0, 0, 0f), _mapBlocks);
                        break;

                    case 's':
                        mapTip = Instantiate(_sectionLinePrefab, pos, Quaternion.Euler(0, 0, 0f), _mapBlocks);
                        break;
                }
                var map = mapTip.GetComponent<Map>();
                map.SetPos(x, y);
                _mapTips[x, y] = mapTip;
            }
        }
    }

    //座標を取得するメソッド
    public Vector2 GetWorldPositionFromTile(int x, int y)
    {
        return new Vector2(x * _tileSize, (_rowLength - y) * _tileSize) - _mapCenterPos;
    }

    public Vector2Int GetPosByCoord(float x, float y)
    {
        var posX = (x + _origin.x) / tileSize;
        var posY = (-y + _origin.y) / tileSize;
        return new Vector2Int(
            (int)posX,
            (int)posY
        );
    }

    public Vector2Int GetPosByCoord(Vector2 pos)
    {
        return GetPosByCoord(pos.x, pos.y);
    }

    public Vector2Int GetTilePosFromWorldPosition(float x, float y)
    {
        return new Vector2Int(
            (int)(x / _tileSize),
            (int)(_rowLength - (y / _tileSize))
        );
    }

    public Vector2 GetNormalizeWorldPosition(float x, float y)
    {
        Vector2Int p = GetTilePosFromWorldPosition(x, y);
        return GetWorldPositionFromTile(p.x, p.y);
    }

    //スポーン座標を取得
    public Vector2Int GetRandomCoord(char type = 'g')
    {
        var value = _map.Cast<char>()//1次元配列に戻す
            .Select((d, i) => new
            {
                X = i / _rowLength,
                Y = i % _rowLength,
                Value = d
            }) //index付きSelectでx,yと値取得
            .Where(t => t.Value == type)   //{type}のみ抽出
            .OrderBy(_ => Guid.NewGuid())   //ランダムな値でソート
            .First();   //最初の一つ(匿名クラス{X, Y, Value}型です。)

        return new Vector2Int(value.X, value.Y);
    }

    public Vector2Int GetNormalizePosition(Vector2Int cur, int addX, int addY)
    {
        var current = new Vector2Int(cur.x, cur.y);
        current.x += addX;
        current.y += addY;

        var w = _map.GetLength(0);
        var h = _map.GetLength(1);

        if (0 > current.x || current.x >= w)
        {
            current.x = -1;
        }

        if (0 > current.y || current.y >= h)
        {
            current.y = -1;
        }
        return current;
    }

    public Map GetMapTip(Vector2Int p)
    {
        var mapTip = mapTips[p.x, p.y];
        var map = mapTip.GetComponent<Map>();
        return map;
    }

    public float CalcDurationBySpeed(int speed)
    {
        return _tileSize / 100 / 4 * (6.0f - (float)speed / 10);
    }

    //視線
    public List<Vector2Int> GetSightLines(
        Vector2Int start,
        Vector2Int end,
        int dist,
        bool horizontal,
        Func<Vector2Int, bool> stopCond
    )
    {
        List<Vector2Int> SightList = new List<Vector2Int>();
        int step = 1;
        Vector2 d = end - start;
        float t = d.x != 0 ? d.x / d.y : 0;
        if (horizontal)
        {
            t = d.y != 0 ? d.y / d.x : 0;
        }
        int i = 0;
        float nx = start.x;
        float ny = start.y;
        while (i < dist)
        {
            i += step;
            if (horizontal)
            {
                nx += step * Math.Sign(d.x);
                ny += t;
            }
            else
            {
                nx += t;
                ny += step * Math.Sign(d.y);
            }
            int nxi = (int)Math.Round(nx);
            int nyi = (int)Math.Round(ny);
            Vector2Int p = GetNormalizePosition(new Vector2Int(nxi, nyi), 0, 0);
            if (
                stopCond(p)
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
}