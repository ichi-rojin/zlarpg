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

    private int
        _rowLength,
        _colLength
    ; //マップの縦横サイズ

    private float _tileSize; //プレファブのサイズ
    private Vector2 _mapCenterPos;

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
        PlaceTiles();//プレファブを並べる処理
    }

    private void PlaceTiles()
    {
        _tileSize = _groundPrefab.GetComponent<Renderer>().bounds.size.x; //タイルサイズ取得
        _mapCenterPos = new Vector2(_colLength / 2 * _tileSize, (_rowLength / 2 + 1) * _tileSize);
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

                Instantiate(_groundPrefab, pos, Quaternion.Euler(0, 0, 0f), _mapBlocks);

                switch (tileType)
                {
                    case 'w':
                        Instantiate(_wallPrefab, pos, Quaternion.Euler(0, 0, 0f), _mapBlocks);
                        break;

                    case 's':
                        Instantiate(_sectionLinePrefab, pos, Quaternion.Euler(0, 0, 0f), _mapBlocks);
                        break;
                }
            }
        }
    }

    //座標を取得するメソッド
    public Vector2 GetWorldPositionFromTile(int x, int y)
    {
        return new Vector2(x * _tileSize, (_rowLength - y) * _tileSize) - _mapCenterPos;
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
}