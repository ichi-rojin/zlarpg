using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AStar
{
    /// A-star�m�[�h.
    private class ANode
    {
        private enum eStatus
        {
            None,
            Open,
            Closed,
        }

        /// �X�e�[�^�X
        private eStatus _status = eStatus.None;

        /// ���R�X�g
        private int _cost = 0;

        /// �q���[���X�e�B�b�N�E�R�X�g
        private int _heuristic = 0;

        /// �e�m�[�h
        private ANode _parent = null;

        /// ���W
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

        /// �R���X�g���N�^.
        public ANode(int x, int y)
        {
            _x = x;
            _y = y;
        }

        /// �X�R�A���v�Z����.
        public int GetScore()
        {
            return _cost + _heuristic;
        }

        /// �q���[���X�e�B�b�N�E�R�X�g�̌v�Z.
        public void CalcHeuristic(bool allowdiag, int xgoal, int ygoal)
        {
            if (allowdiag)
            {
                // �΂߈ړ�����
                var dx = (int)Mathf.Abs(xgoal - X);
                var dy = (int)Mathf.Abs(ygoal - Y);
                // �傫�������R�X�g�ɂ���
                _heuristic = dx > dy ? dx : dy;
            }
            else
            {
                // �c���ړ��̂�
                var dx = Mathf.Abs(xgoal - X);
                var dy = Mathf.Abs(ygoal - Y);
                _heuristic = (int)(dx + dy);
            }
            //Dump();
        }

        /// �X�e�[�^�X��None���ǂ���.
        public bool IsNone()
        {
            return _status == eStatus.None;
        }

        /// �X�e�[�^�X��Open�ɂ���.
        public void Open(ANode parent, int cost)
        {
            //Debug.Log(string.Format("Open: ({0},{1})", X, Y));
            _status = eStatus.Open;
            _cost = cost;
            _parent = parent;
        }

        /// �X�e�[�^�X��Closed�ɂ���.
        public void Close()
        {
            //Debug.Log(string.Format("Closed: ({0},{1})", X, Y));
            _status = eStatus.Closed;
        }

        /// �p�X���擾����
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
                // �ċA�I�Ƀ_���v����.
                _parent.DumpRecursive();
            }
        }
    }

    /// A-star�m�[�h�Ǘ�.
    private class ANodeMgr
    {
        /// �n�`���C���[.
        private int[,] _layer;

        /// �m�[�h�C���X�^���X�Ǘ�.
        private ANode[,] _pool;

        /// �΂߈ړ��������邩�ǂ���.
        private bool _allowdiag = true;

        /// �I�[�v�����X�g.
        private List<ANode> _openList = null;

        /// �S�[�����W.
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

        /// �m�[�h��������.
        public ANode GetNode(int x, int y)
        {
            if (_pool[x, y] != null)
            {
                // ���ɑ��݂��Ă���̂Ńv�[�����O����擾.
                return _pool[x, y];
            }

            // pool�ɂȂ���ΐV�K�쐬.
            var node = new ANode(x, y);
            _pool[x, y] = node;

            // �q���[���X�e�B�b�N�E�R�X�g���v�Z����.
            node.CalcHeuristic(_allowdiag, _xgoal, _ygoal);
            return node;
        }

        /// �m�[�h���I�[�v�����X�g�ɒǉ�����.
        public void AddOpenList(ANode node)
        {
            _openList.Add(node);
        }

        /// �m�[�h���I�[�v�����X�g����폜����.
        public void RemoveOpenList(ANode node)
        {
            _openList.Remove(node);
        }

        /// �w��̍��W�ɂ���m�[�h���I�[�v������.
        public ANode OpenNode(int x, int y, int cost, ANode parent)
        {
            // �m�[�h���擾����.
            var node = GetNode(x, y);
            if (node.IsNone() == false)
            {
                // ����Open���Ă���̂ŉ������Ȃ�
                return null;
            }
            int _cost = (int)_layer[x, y] + (int)cost;//�}�b�v�̈ړ��R�X�g�ƍ��Z
            // Open����.
            node.Open(parent, _cost);
            AddOpenList(node);

            return node;
        }

        /// �����Open����.
        public void OpenAround(ANode parent)
        {
            var xbase = parent.X; // ����W(X).
            var ybase = parent.Y; // ����W(Y).
            var cost = parent.Cost; // �R�X�g.
            cost += 1; // ����i�ނ̂�+1����.
            if (_allowdiag)
            {
                // 8�������J��.
                for (int j = 0; j < 3; j++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        var x = xbase + i - 1; // -1�`1
                        var y = ybase + j - 1; // -1�`1
                        OpenNode(x, y, cost, parent);
                    }
                }
            }
            else
            {
                // 4�������J��.
                var x = xbase;
                var y = ybase;
                OpenNode(x - 1, y, cost, parent); // �E.
                OpenNode(x, y - 1, cost, parent); // ��.
                OpenNode(x + 1, y, cost, parent); // ��.
                OpenNode(x, y + 1, cost, parent); // ��.
            }
        }

        /// �ŏ��X�R�A�̃m�[�h���擾����.
        public ANode SearchMinScoreNodeFromOpenList()
        {
            // �ŏ��X�R�A
            int min = 9999;
            // �ŏ����R�X�g
            int minCost = 9999;
            ANode minNode = null;
            foreach (ANode node in _openList)
            {
                int score = node.GetScore();
                if (score > min)
                {
                    // �X�R�A���傫��
                    continue;
                }
                if (score == min && node.Cost >= minCost)
                {
                    // �X�R�A�������Ƃ��͎��R�X�g����r����
                    continue;
                }

                // �ŏ��l�X�V.
                min = score;
                minCost = node.Cost;
                minNode = node;
            }
            return minNode;
        }
    }

    //�T������
    public List<Vector2Int> Serch(Vector2Int startPos, Vector2Int endPos, int[,] layer)
    {
        List<Vector2Int> pList = new List<Vector2Int>();
        // A-star���s.
        {
            // �΂߈ړ�������
            var allowdiag = false;
            var mgr = new ANodeMgr(layer, endPos.x, endPos.y, allowdiag);
            // �X�^�[�g�n�_�̃m�[�h�擾
            // �X�^�[�g�n�_�Ȃ̂ŃR�X�g�́u0�v
            ANode node = mgr.OpenNode(startPos.x, startPos.y, 0, null);
            mgr.AddOpenList(node);

            // ���s�񐔁B100�񒴂����狭�����f
            int cnt = 0;
            while (cnt < 100)
            {
                mgr.RemoveOpenList(node);
                // ���͂��J��
                mgr.OpenAround(node);
                // �ŏ��X�R�A�̃m�[�h��T��.
                node = mgr.SearchMinScoreNodeFromOpenList();
                if (node == null)
                {
                    // �܏��H�Ȃ̂ł����܂�.
                    Debug.Log("Not found path.");
                    break;
                }

                if (node.X == endPos.x && node.Y == endPos.y)
                {
                    // �S�[���ɂ��ǂ蒅����.
                    //Debug.Log("Success.");
                    mgr.RemoveOpenList(node);
                    //node.DumpRecursive();
                    // �p�X���擾����
                    node.GetPath(pList);
                    // ���]����
                    pList.Reverse();
                    break;
                }
            }
        }

        return pList;
    }
}