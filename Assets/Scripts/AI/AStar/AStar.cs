using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AStar
{
    /// A-starノード.
    private class ANode
    {
        private enum eStatus
        {
            None,
            Open,
            Closed,
        }

        /// ステータス
        private eStatus _status = eStatus.None;

        /// 実コスト
        private int _cost = 0;

        /// ヒューリスティック・コスト
        private int _heuristic = 0;

        /// 親ノード
        private ANode _parent = null;

        /// 座標
        private int _x = 0;

        private int _y = 0;

        public int X
        {
            get { return _x; }
        }

        public int Y
        {
            get { return _y; }
        }

        public int Cost
        {
            get { return _cost; }
        }

        /// コンストラクタ.
        public ANode(int x, int y)
        {
            _x = x;
            _y = y;
        }

        /// スコアを計算する.
        public int GetScore()
        {
            return _cost + _heuristic;
        }

        /// ヒューリスティック・コストの計算.
        public void CalcHeuristic(bool allowdiag, int xgoal, int ygoal)
        {
            if (allowdiag)
            {
                // 斜め移動あり
                var dx = (int)Mathf.Abs(xgoal - X);
                var dy = (int)Mathf.Abs(ygoal - Y);
                // 大きい方をコストにする
                _heuristic = dx > dy ? dx : dy;
            }
            else
            {
                // 縦横移動のみ
                var dx = Mathf.Abs(xgoal - X);
                var dy = Mathf.Abs(ygoal - Y);
                _heuristic = (int)(dx + dy);
            }
            //Dump();
        }

        /// ステータスがNoneかどうか.
        public bool IsNone()
        {
            return _status == eStatus.None;
        }

        /// ステータスをOpenにする.
        public void Open(ANode parent, int cost)
        {
            //Debug.Log(string.Format("Open: ({0},{1})", X, Y));
            _status = eStatus.Open;
            _cost = cost;
            _parent = parent;
        }

        /// ステータスをClosedにする.
        public void Close()
        {
            //Debug.Log(string.Format("Closed: ({0},{1})", X, Y));
            _status = eStatus.Closed;
        }

        /// パスを取得する
        public void GetPath(List<Vector2Int> pList)
        {
            pList.Add(new Vector2Int(X, Y));
            if (_parent != null)
            {
                _parent.GetPath(pList);
            }
        }

        public void Dump()
        {
            Debug.Log(string.Format("({0},{1})[{2}] cost={3} heuris={4} score={5}", X, Y, _status, _cost, _heuristic, GetScore()));
        }

        public void DumpRecursive()
        {
            Dump();
            if (_parent != null)
            {
                // 再帰的にダンプする.
                _parent.DumpRecursive();
            }
        }
    }

    /// A-starノード管理.
    private class ANodeMgr
    {
        /// 地形レイヤー.
        private int[,] _layer;

        /// ノードインスタンス管理.
        private ANode[,] _pool;

        /// 斜め移動を許可するかどうか.
        private bool _allowdiag = true;

        /// オープンリスト.
        private List<ANode> _openList = null;

        /// ゴール座標.
        private int _xgoal = 0;

        private int _ygoal = 0;

        public ANodeMgr(int[,] layer, int xgoal, int ygoal, bool allowdiag = true)
        {
            _layer = layer;
            _pool = new ANode[_layer.GetLength(0), _layer.GetLength(1)];
            _allowdiag = allowdiag;
            _openList = new List<ANode>();
            _xgoal = xgoal;
            _ygoal = ygoal;
        }

        /// ノード生成する.
        public ANode GetNode(int x, int y)
        {
            if (_pool[x, y] != null)
            {
                // 既に存在しているのでプーリングから取得.
                return _pool[x, y];
            }

            // poolになければ新規作成.
            var node = new ANode(x, y);
            _pool[x, y] = node;

            // ヒューリスティック・コストを計算する.
            node.CalcHeuristic(_allowdiag, _xgoal, _ygoal);
            return node;
        }

        /// ノードをオープンリストに追加する.
        public void AddOpenList(ANode node)
        {
            _openList.Add(node);
        }

        /// ノードをオープンリストから削除する.
        public void RemoveOpenList(ANode node)
        {
            _openList.Remove(node);
        }

        /// 指定の座標にあるノードをオープンする.
        public ANode OpenNode(int x, int y, int cost, ANode parent)
        {
            // ノードを取得する.
            var node = GetNode(x, y);
            if (node.IsNone() == false)
            {
                // 既にOpenしているので何もしない
                return null;
            }
            int _cost = (int)_layer[x, y] + (int)cost;//マップの移動コストと合算
            // Openする.
            node.Open(parent, _cost);
            AddOpenList(node);

            return node;
        }

        /// 周りをOpenする.
        public void OpenAround(ANode parent)
        {
            var xbase = parent.X; // 基準座標(X).
            var ybase = parent.Y; // 基準座標(Y).
            var cost = parent.Cost; // コスト.
            cost += 1; // 一歩進むので+1する.
            if (_allowdiag)
            {
                // 8方向を開く.
                for (int j = 0; j < 3; j++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        var x = xbase + i - 1; // -1～1
                        var y = ybase + j - 1; // -1～1
                        OpenNode(x, y, cost, parent);
                    }
                }
            }
            else
            {
                // 4方向を開く.
                var x = xbase;
                var y = ybase;
                OpenNode(x - 1, y, cost, parent); // 右.
                OpenNode(x, y - 1, cost, parent); // 上.
                OpenNode(x + 1, y, cost, parent); // 左.
                OpenNode(x, y + 1, cost, parent); // 下.
            }
        }

        /// 最小スコアのノードを取得する.
        public ANode SearchMinScoreNodeFromOpenList()
        {
            // 最小スコア
            int min = 9999;
            // 最小実コスト
            int minCost = 9999;
            ANode minNode = null;
            foreach (ANode node in _openList)
            {
                int score = node.GetScore();
                if (score > min)
                {
                    // スコアが大きい
                    continue;
                }
                if (score == min && node.Cost >= minCost)
                {
                    // スコアが同じときは実コストも比較する
                    continue;
                }

                // 最小値更新.
                min = score;
                minCost = node.Cost;
                minNode = node;
            }
            return minNode;
        }
    }

    //探索する
    public List<Vector2Int> Serch(Vector2Int startPos, Vector2Int endPos, int[,] layer)
    {
        List<Vector2Int> pList = new List<Vector2Int>();
        // A-star実行.
        {
            // 斜め移動を許可
            var allowdiag = false;
            var mgr = new ANodeMgr(layer, endPos.x, endPos.y, allowdiag);
            // スタート地点のノード取得
            // スタート地点なのでコストは「0」
            ANode node = mgr.OpenNode(startPos.x, startPos.y, 0, null);
            mgr.AddOpenList(node);

            // 試行回数。100回超えたら強制中断
            int cnt = 0;
            while (cnt < 100)
            {
                mgr.RemoveOpenList(node);
                // 周囲を開く
                mgr.OpenAround(node);
                // 最小スコアのノードを探す.
                node = mgr.SearchMinScoreNodeFromOpenList();
                if (node == null)
                {
                    // 袋小路なのでおしまい.
                    Debug.Log("Not found path.");
                    break;
                }

                if (node.X == endPos.x && node.Y == endPos.y)
                {
                    // ゴールにたどり着いた.
                    //Debug.Log("Success.");
                    mgr.RemoveOpenList(node);
                    //node.DumpRecursive();
                    // パスを取得する
                    node.GetPath(pList);
                    // 反転する
                    pList.Reverse();
                    break;
                }
            }
        }

        return pList;
    }
}