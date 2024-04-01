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
    ; //�e��v���t�@�u

    [SerializeField]
    private Transform mapBlocks; //�}�b�v�̃Q�[���I�u�W�F�N�g

    private string[] textData;
    private char[,] map; //�}�b�v�f�[�^

    private int
        rowLength,
        colLength
    ; //�}�b�v�̏c���T�C�Y

    private float tileSize; //�v���t�@�u�̃T�C�Y
    private Vector2 mapCenterPos;

    private void Start()
    {
        string textLines = textFile.text; // �e�L�X�g�̑S�̃f�[�^�̑��

        // ���s�Ńf�[�^�𕪊����Ĕz��ɑ��
        textData = textLines.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        // �s���Ɨ񐔂̎擾
        colLength = textData[0].ToCharArray().Length;
        rowLength = textData.Length;

        // �Q�����z��̒�`
        map = new char[colLength, rowLength];//�}�b�v�쐬
        PlaceTiles();//�v���t�@�u����ׂ鏈��
    }

    private void PlaceTiles()
    {
        tileSize = groundPrefab.GetComponent<Renderer>().bounds.size.x; //�^�C���T�C�Y�擾
        mapCenterPos = new Vector2(colLength / 2 * tileSize, (rowLength / 2 + 1) * tileSize);
        if (colLength % 2 == 0) mapCenterPos.x -= tileSize / 2;
        if (rowLength % 2 == 0) mapCenterPos.y -= tileSize / 2;

        for (int y = rowLength - 1; y >= 0; y--)//���]���ēǂݍ���
        {
            char[] tempWords = textData[y].ToCharArray();

            for (int x = 0; x < colLength; x++)
            {
                char tileType = tempWords[x]; //�}�b�v�̎�ގ擾
                Vector2 pos = GetWorldPositionFromTile(x, y);//���W���v�Z

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

    //���W���擾���郁�\�b�h
    public Vector2 GetWorldPositionFromTile(int x, int y)
    {
        return new Vector2(x * tileSize, (rowLength - y) * tileSize) - mapCenterPos;
    }

    //�X�|�[�����W���擾
    public Vector2 GetRandomPosition(char type = 'g')
    {
        var value = map.Cast<char>()//1�����z��ɖ߂�

            .Select((d, i) => new
            {
                X = i / rowLength,
                Y = i % rowLength,
                Value = d
            }) //index�t��Select��x,y�ƒl�擾
            .Where(t => t.Value == type)   //{type}�̂ݒ��o
            .OrderBy(_ => Guid.NewGuid())   //�����_���Ȓl�Ń\�[�g
            .First();   //�ŏ��̈��(�����N���X{X, Y, Value}�^�ł��B)

        return GetWorldPositionFromTile(value.X, value.Y);
    }
}