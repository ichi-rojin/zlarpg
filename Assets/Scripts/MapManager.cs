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
        groundPrefab,
        wallPrefab,
        sectionLinePrefab
    ; //各種プレファブ

    [SerializeField]
    private Transform mapBlocks; //マップのゲームオブジェクト

    private string[] textData;
    private char[,] map; //マップデータ

    private int
        rowLength,
        colLength
    ; //マップの縦横サイズ

    private float tileSize; //プレファブのサイズ
    private Vector2 mapCenterPos;

    private void Start()
    {
        string textLines = textFile.text; // テキストの全体データの代入

        // 改行でデータを分割して配列に代入
        textData = textLines.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        // 行数と列数の取得
        colLength = textData[0].ToCharArray().Length;
        rowLength = textData.Length;

        // ２次元配列の定義
        map = new char[colLength, rowLength];//マップ作成
        PlaceTiles();//プレファブを並べる処理
    }

    private void PlaceTiles()
    {
        tileSize = groundPrefab.GetComponent<Renderer>().bounds.size.x; //タイルサイズ取得
        mapCenterPos = new Vector2(colLength / 2 * tileSize, (rowLength / 2 + 1) * tileSize);
        if (colLength % 2 == 0) mapCenterPos.x -= tileSize / 2;
        if (rowLength % 2 == 0) mapCenterPos.y -= tileSize / 2;

        for (int y = rowLength - 1; y >= 0; y--)//反転して読み込む
        {
            char[] tempWords = textData[y].ToCharArray();

            for (int x = 0; x < colLength; x++)
            {
                char tileType = tempWords[x]; //マップの種類取得
                Vector2 pos = GetWorldPositionFromTile(x, y);//座標を計算

                map[x, y] = tileType;

                Instantiate(groundPrefab, pos, Quaternion.Euler(0, 0, 0f), mapBlocks);

                switch (tileType)
                {
                    case 'w':
                        Instantiate(wallPrefab, pos, Quaternion.Euler(0, 0, 0f), mapBlocks);
                        break;

                    case 's':
                        Instantiate(sectionLinePrefab, pos, Quaternion.Euler(0, 0, 0f), mapBlocks);
                        break;
                }
            }
        }
    }

    //座標を取得するメソッド
    public Vector2 GetWorldPositionFromTile(int x, int y)
    {
        return new Vector2(x * tileSize, (rowLength - y) * tileSize) - mapCenterPos;
    }

    //スポーン座標を取得
    public Vector2 GetRandomPosition(char type = 'g')
    {
        var value = map.Cast<char>()//1次元配列に戻す

            .Select((d, i) => new
            {
                X = i / rowLength,
                Y = i % rowLength,
                Value = d
            }) //index付きSelectでx,yと値取得
            .Where(t => t.Value == type)   //{type}のみ抽出
            .OrderBy(_ => Guid.NewGuid())   //ランダムな値でソート
            .First();   //最初の一つ(匿名クラス{X, Y, Value}型です。)

        return GetWorldPositionFromTile(value.X, value.Y);
    }
}