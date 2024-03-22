using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] GameObject
        groundPrefab,
        wallPrefab,
        sectionLinePrefab
    ; //�e��v���t�@�u

    [SerializeField]
    private TextAsset textFile;

    private string[] textData;
    string[,] map; //�}�b�v�f�[�^

    private int
        rowLength,
        colLength
    ; //�}�b�v�̏c���T�C�Y
    
    float tileSize; //�v���t�@�u�̃T�C�Y
    Vector2 mapCenterPos; //�}�b�v�̃Z���^�[�ʒu

    private GameObject parent;

    void Start()
    {
        parent = GameObject.Find("Map");
        string textLines = textFile.text; // �e�L�X�g�̑S�̃f�[�^�̑��

        // ���s�Ńf�[�^�𕪊����Ĕz��ɑ��
        textData = textLines.Split('\n');

        // �s���Ɨ񐔂̎擾
        colLength = textData[0].Split(',').Length;
        rowLength = textData.Length;

        // �Q�����z��̒�`
        map = new string[colLength, rowLength];//�}�b�v�쐬
        PlaceTiles();//�v���t�@�u����ׂ鏈��
    }

    void PlaceTiles()
    {
        tileSize = groundPrefab.GetComponent<Renderer>().bounds.size.x; //�^�C���T�C�Y�擾
        mapCenterPos = new Vector2(colLength * tileSize / 2, rowLength * tileSize / 2); //���S���W�擾
        for (int i = 0; i < rowLength; i++)
        {
            string[] tempWords = textData[i].Split(',');

            for (int j = 0; j < colLength; j++)
            {
                string tileType = tempWords[j]; //�}�b�v�̎�ގ擾
                Vector2 pos = GetWorldPositionFromTile(j, i);//���W���v�Z

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
    //���W���擾���郁�\�b�h
    Vector2 GetWorldPositionFromTile(int x, int y)
    {
        return new Vector2(x * tileSize, (rowLength - y) * tileSize) - mapCenterPos;
    }
}