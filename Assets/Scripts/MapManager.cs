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
    ; //�e��v���t�@�u

    [SerializeField]
    private Transform _mapBlocks; //�}�b�v�̃Q�[���I�u�W�F�N�g

    private string[] _textData;
    private char[,] _map; //�}�b�v�f�[�^

    // �v���p�e�B
    public char[,] map
    {
        get { return _map; }
    }

    private int
        _rowLength,
        _colLength
    ; //�}�b�v�̏c���T�C�Y

    private float _tileSize; //�v���t�@�u�̃T�C�Y
    private Vector2 _mapCenterPos;

    private void Start()
    {
        string textLines = textFile.text; // �e�L�X�g�̑S�̃f�[�^�̑��

        // ���s�Ńf�[�^�𕪊����Ĕz��ɑ��
        _textData = textLines.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);

        // �s���Ɨ񐔂̎擾
        _colLength = _textData[0].ToCharArray().Length;
        _rowLength = _textData.Length;

        // �Q�����z��̒�`
        _map = new char[_colLength, _rowLength];//�}�b�v�쐬
        PlaceTiles();//�v���t�@�u����ׂ鏈��
    }

    private void PlaceTiles()
    {
        _tileSize = _groundPrefab.GetComponent<Renderer>().bounds.size.x; //�^�C���T�C�Y�擾
        _mapCenterPos = new Vector2(_colLength / 2 * _tileSize, (_rowLength / 2 + 1) * _tileSize);
        if (_colLength % 2 == 0) _mapCenterPos.x -= _tileSize / 2;
        if (_rowLength % 2 == 0) _mapCenterPos.y -= _tileSize / 2;

        for (int y = _rowLength - 1; y >= 0; y--)//���]���ēǂݍ���
        {
            char[] tempWords = _textData[y].ToCharArray();

            for (int x = 0; x < _colLength; x++)
            {
                char tileType = tempWords[x]; //�}�b�v�̎�ގ擾
                Vector2 pos = GetWorldPositionFromTile(x, y);//���W���v�Z

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
                mapTip.GetComponent<Map>().SetPos(new Vector2Int(x, y));
            }
        }
    }

    //���W���擾���郁�\�b�h
    public Vector2 GetWorldPositionFromTile(int x, int y)
    {
        return new Vector2(x * _tileSize, (_rowLength - y) * _tileSize) - _mapCenterPos;
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

    //�X�|�[�����W���擾
    public Vector2Int GetRandomCoord(char type = 'g')
    {
        var value = _map.Cast<char>()//1�����z��ɖ߂�
            .Select((d, i) => new
            {
                X = i / _rowLength,
                Y = i % _rowLength,
                Value = d
            }) //index�t��Select��x,y�ƒl�擾
            .Where(t => t.Value == type)   //{type}�̂ݒ��o
            .OrderBy(_ => Guid.NewGuid())   //�����_���Ȓl�Ń\�[�g
            .First();   //�ŏ��̈��(�����N���X{X, Y, Value}�^�ł��B)

        return new Vector2Int(value.X, value.Y);
    }
}