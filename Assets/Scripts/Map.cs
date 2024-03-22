using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] GameObject
        groundPrefab,
        wallPrefab,
        sectionLinePrefab
    ; //各種プレファブ

    [SerializeField]
    private TextAsset textFile;

    private string[] textData;
    string[,] map; //マップデータ

    private int
        rowLength,
        colLength
    ; //マップの縦横サイズ
    
    float tileSize; //プレファブのサイズ
    Vector2 mapCenterPos; //マップのセンター位置

    private GameObject parent; //マップのゲームオブジェクト

    void Start()
    {
        parent = GameObject.Find("Map");
        string textLines = textFile.text; // テキストの全体データの代入

        // 改行でデータを分割して配列に代入
        textData = textLines.Split('\n');

        // 行数と列数の取得
        colLength = textData[0].Split(',').Length;
        rowLength = textData.Length;

        // ２次元配列の定義
        map = new string[colLength, rowLength + 1];//マップ作成
        PlaceTiles();//プレファブを並べる処理
        for (int y = 0; y < rowLength; y++)
        {
            for (int x = 0; x < colLength; x++)
            {
                print(map[x, y]);
            }
        }
    }

    void PlaceTiles()
    {
        tileSize = groundPrefab.GetComponent<Renderer>().bounds.size.x; //タイルサイズ取得
        mapCenterPos = new Vector2(colLength * tileSize / 2, rowLength * tileSize / 2); //中心座標取得
        for (int y = 0; y < rowLength; y++)
        {
            string[] tempWords = textData[y].Split(',');

            for (int x = 0; x < colLength; x++)
            {
                string tileType = tempWords[x]; //マップの種類取得
                Vector2 pos = GetWorldPositionFromTile(x, y);//座標を計算

                map[x, y] = tileType;

                if (tileType != null)
                {
                    switch (tileType)
                    {
                        case "w":
                            Instantiate(wallPrefab, pos, Quaternion.Euler(0, 0, 0f), parent.transform);
                            break;

                        case "g":
                            Instantiate(groundPrefab, pos, Quaternion.Euler(0, 0, 0f), parent.transform);
                            break;

                        case "s":
                            Instantiate(sectionLinePrefab, pos, Quaternion.Euler(0, 0, 0f), parent.transform);
                            break;
                    }
                }
            }
        }
    }
    //座標を取得するメソッド
    Vector2 GetWorldPositionFromTile(int x, int y)
    {
        return new Vector2(x * tileSize, (rowLength - y) * tileSize) - mapCenterPos;
    }
}