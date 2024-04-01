using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AStar
{
    public struct GridNode
    {
        public Vector2Int position { get; set; }//���̃O���b�h�̃|�W�V����
        public Vector2Int parentIndex { get; set; }//���̃O���b�h�̃����N��@Vector2Int(x , y)
        public float score { get; set; }// �ړ��ɕK�v�ȃR�X�g�@�{�@���݂̈ʒu�ƃS�[���n�_�̋����@Heu + Cost
        public float Heu { get; set; } // ���݂̈ʒu�ƃS�[���n�_�̋����@Vector2Int.Distance(grid_position, goal)
        public float cost { get; set; } // �ړ��ɕK�v�ȃR�X�g�@cell_size * n
        public char[,] layer { get; set; }//���C���[�ԍ�

        public override string ToString()
        {
            return "now:" + position + "  parent:" + parentIndex + "  Heu:" + Heu + "  score:" + score + "  cost:" + cost + "  layer:" + layer;
        }
    }

    //�T������
    public List<Vector2Int> Serch(Vector2Int startPos, Vector2Int endPos, char[,] layer)
    {
        Vector2Int goalIndex = new Vector2Int(endPos.x, endPos.y);
        //�o�H�������List�쐬�Ɏg��
        Vector2Int finalFlag;
        //�m�[�h�Ǘ��p�ϐ�
        Vector2Int nodeIndex;
        Dictionary<Vector2Int, GridNode> openList;
        openList = new Dictionary<Vector2Int, GridNode>();
        Dictionary<Vector2Int, GridNode> closeList;
        closeList = new Dictionary<Vector2Int, GridNode>();

        //�X�^�[�g�m�[�h�𐶐������X������
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

        int i = 1;//��������������ׂ̃f�o�b�N�ϐ�

        while (openList?.Count > 0) //�I�[�v�����X�g����łȂ�
        {
            //Debug.Log("�X�^�[�g:" + i + "���;" + node.ToString());
            i++;

            //Dictionary���\�[�g���ăm�[�h���X�V����
            var sortList = openList.OrderBy((score) => score.Value.score);
            node = sortList.ElementAt(0).Value;
            nodeIndex = sortList.ElementAt(0).Key;

            //�o�H������
            if (nodeIndex == goalIndex)
            {
                List<Vector2Int> successList;
                successList = new List<Vector2Int>();
                //�S�[������X�^�[�g�܂ł̍ŒZ�o�H�𒊏o����
                while (nodeIndex != finalFlag)
                {
                    successList.Add(nodeIndex);
                    node = closeList[node.parentIndex];
                    //Debug.DrawLine(nodeIndex, node.parentIndex, Color.red, 3.5f);
                    nodeIndex = node.parentIndex;
                }
                successList.Add(nodeIndex);

                Debug.Log(successList.Count + "�}�X�ŃS�[��");
                successList.Reverse();//���]���Č��ʂ�Ԃ�
                return successList;
            }
            else
            {
                //���݂̃m�[�h���N���[�Y�Ɉڂ�
                openList.Remove(nodeIndex);
                closeList.Add(nodeIndex, node);
                //���݂̃m�[�h�ɗאڂ���8�����m�[�h�𒲂ׂ�
                for (int w = -1; w < 2; w++)
                {
                    for (int h = -1; h < 2; h++)
                    {
                        if (!(w == 0 && h == 0))//�������g�łȂ�
                        {
                            //�N���[�Y���X�g�Ɋ܂܂�ĂȂ����
                            if (!closeList.ContainsKey(new Vector2Int(nodeIndex.x + w, nodeIndex.y + h)))
                            {
                                //�I�[�v�����X�g�Ɋ܂܂�ĂȂ����
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
        //�S�[���ɒH����Ȃ������ꍇ
        return new List<Vector2Int>();
    }
}