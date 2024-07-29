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

    private GameObject[,] _mapTips; //�}�b�v�`�b�v�i�[

    // �v���p�e�B
    public GameObject[,] mapTips
    {
        get { return _mapTips; }
    }

    private int
        _rowLength,
        _colLength
    ; //�}�b�v�̏c���T�C�Y

    private float _tileSize; //�v���t�@�u�̃T�C�Y

    public float tileSize
    {
        get { return _tileSize; }
    }

    private Vector2 _mapCenterPos;

    //���_������Ƃ���
    private Vector2 _origin;

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

        _mapTips = new GameObject[_colLength, _rowLength];//�}�b�v�`�b�v�i�[
        PlaceTiles();//�v���t�@�u����ׂ鏈��
    }

    private void PlaceTiles()
    {
        _tileSize = _groundPrefab.GetComponent<Renderer>().bounds.size.x; //�^�C���T�C�Y�擾
        _mapCenterPos = new Vector2(_colLength / 2 * _tileSize, (_rowLength / 2 + 1) * _tileSize);
        _origin = new Vector2(_colLength * _tileSize / 2, _rowLength * _tileSize / 2);
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
                var map = mapTip.GetComponent<Map>();
                map.SetPos(x, y);
                _mapTips[x, y] = mapTip;
            }
        }
    }

    //���W���擾���郁�\�b�h
    public Vector2 GetWorldPositionFromTile(int x, int y)
    {
        return new Vector2(x * _tileSize, (_rowLength - y) * _tileSize) - _mapCenterPos;
    }

    public Vector2Int GetPosByCoord(float x, float y)
    {
        var posX = (x + _origin.x) / tileSize;
        var posY = (-y + _origin.y) / tileSize;
        return new Vector2Int(
            (int)posX,
            (int)posY
        );
    }

    public Vector2Int GetPosByCoord(Vector2 pos)
    {
        return GetPosByCoord(pos.x, pos.y);
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

    public Vector2Int GetNormalizePosition(Vector2Int cur, int addX, int addY)
    {
        var current = new Vector2Int(cur.x, cur.y);
        current.x += addX;
        current.y += addY;

        var w = _map.GetLength(0);
        var h = _map.GetLength(1);

        if (0 > current.x || current.x >= w)
        {
            current.x = -1;
        }

        if (0 > current.y || current.y >= h)
        {
            current.y = -1;
        }
        return current;
    }

    public Map GetMapTip(Vector2Int p)
    {
        var mapTip = mapTips[p.x, p.y];
        var map = mapTip.GetComponent<Map>();
        return map;
    }

    public float CalcDurationBySpeed(int speed)
    {
        return _tileSize / 100 / 4 * (6.0f - (float)speed / 10);
    }

    //����
    public List<Vector2Int> GetSightLines(
        Vector2Int start,
        Vector2Int end,
        int dist,
        bool horizontal,
        Func<Vector2Int, bool> stopCond
    )
    {
        List<Vector2Int> SightList = new List<Vector2Int>();
        int step = 1;
        Vector2 d = end - start;
        float t = d.x != 0 ? d.x / d.y : 0;
        if (horizontal)
        {
            t = d.y != 0 ? d.y / d.x : 0;
        }
        int i = 0;
        float nx = start.x;
        float ny = start.y;
        while (i < dist)
        {
            i += step;
            if (horizontal)
            {
                nx += step * Math.Sign(d.x);
                ny += t;
            }
            else
            {
                nx += t;
                ny += step * Math.Sign(d.y);
            }
            int nxi = (int)Math.Round(nx);
            int nyi = (int)Math.Round(ny);
            Vector2Int p = GetNormalizePosition(new Vector2Int(nxi, nyi), 0, 0);
            if (
                stopCond(p)
            )
            {
                break;
            }
            if (SightList.Contains(p) == false)
            {
                SightList.Add(p);
            }
        }
        return SightList;
    }
}