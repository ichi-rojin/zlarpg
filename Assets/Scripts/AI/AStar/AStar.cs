using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AStar
{
    public struct GridNode
    {
        public Vector2Int position { get; set; }//このグリッドのポジション
        public Vector2Int parentIndex { get; set; }//このグリッドのリンク先　Vector2Int(x , y)
        public float score { get; set; }// 移動に必要なコスト　＋　現在の位置とゴール地点の距離　Heu + Cost
        public float Heu { get; set; } // 現在の位置とゴール地点の距離　Vector2Int.Distance(grid_position, goal)
        public float cost { get; set; } // 移動に必要なコスト　cell_size * n
        public char[,] layer { get; set; }//レイヤー番号

        public override string ToString()
        {
            return "now:" + position + "  parent:" + parentIndex + "  Heu:" + Heu + "  score:" + score + "  cost:" + cost + "  layer:" + layer;
        }
    }

    //探索する
    public List<Vector2Int> Serch(Vector2Int startPos, Vector2Int endPos, char[,] layer)
    {
        Vector2Int goalIndex = new Vector2Int(endPos.x, endPos.y);
        //経路完成後のList作成に使う
        Vector2Int finalFlag;
        //ノード管理用変数
        Vector2Int nodeIndex;
        Dictionary<Vector2Int, GridNode> openList;
        openList = new Dictionary<Vector2Int, GridNode>();
        Dictionary<Vector2Int, GridNode> closeList;
        closeList = new Dictionary<Vector2Int, GridNode>();

        //スタートノードを生成＆諸々初期化
        GridNode node = new GridNode();
        node.position = startPos;
        node.Heu = Vector2Int.Distance(
            new Vector2Int(startPos.x, startPos.y),
            new Vector2Int(endPos.x, endPos.y)
        );
        node.score = node.Heu;
        node.cost = 0;
        node.layer = layer;
        //Helper.dumpCharArray(node.layer);

        finalFlag = node.parentIndex;
        nodeIndex = (Vector2Int)node.position;
        openList.Add(nodeIndex, node);

        int i = 1;//何回回ったか見る為のデバック変数

        while (openList?.Count > 0) //オープンリストが空でない
        {
            //Debug.Log("スタート:" + i + "回目;" + node.ToString());
            i++;

            //Dictionaryをソートしてノードを更新する
            var sortList = openList.OrderBy((score) => score.Value.score);
            node = sortList.ElementAt(0).Value;
            nodeIndex = sortList.ElementAt(0).Key;

            //経路が完成
            if (nodeIndex == goalIndex)
            {
                List<Vector2Int> successList;
                successList = new List<Vector2Int>();
                //ゴールからスタートまでの最短経路を抽出する
                while (nodeIndex != finalFlag)
                {
                    successList.Add(nodeIndex);
                    node = closeList[node.parentIndex];
                    //Debug.DrawLine(nodeIndex, node.parentIndex, Color.red, 3.5f);
                    nodeIndex = node.parentIndex;
                }
                successList.Add(nodeIndex);

                Debug.Log(successList.Count + "マスでゴール");
                successList.Reverse();//反転して結果を返す
                return successList;
            }
            else
            {
                //現在のノードをクローズに移す
                openList.Remove(nodeIndex);
                closeList.Add(nodeIndex, node);
                //現在のノードに隣接する8方向ノードを調べる
                for (int w = -1; w < 2; w++)
                {
                    for (int h = -1; h < 2; h++)
                    {
                        if (!(w == 0 && h == 0))//自分自身でない
                        {
                            //クローズリストに含まれてなければ
                            if (!closeList.ContainsKey(new Vector2Int(nodeIndex.x + w, nodeIndex.y + h)))
                            {
                                //オープンリストに含まれてなければ
                                if (!openList.ContainsKey(new Vector2Int(nodeIndex.x + w, nodeIndex.y + h)))
                                {
                                    GridNode nextNode = new GridNode();
                                    nextNode.position = node.position + new Vector2Int(w, h);
                                    nextNode.parentIndex = (Vector2Int)node.position + new Vector2Int(0, 0);
                                    nextNode.Heu = Vector2Int.Distance(nextNode.position, endPos);
                                    nextNode.cost = node.cost;
                                    nextNode.score = nextNode.Heu + nextNode.cost;
                                    nextNode.layer = node.layer;

                                    openList.Add((Vector2Int)nextNode.position + new Vector2Int(0, 0), nextNode);
                                    //Debug.Log(nextNode.ToString());
                                }
                            }
                        }
                    }
                }
            }
        }
        //ゴールに辿りつけなかった場合
        return new List<Vector2Int>();
    }
}