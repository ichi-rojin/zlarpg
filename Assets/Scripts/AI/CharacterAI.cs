using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterAI : MonoBehaviour
{
    // 状態.
    private enum eState
    {
        Exec, // 実行中.
        Walk, // 移動中.
        End,  // おしまい.
    }

    private eState _state = eState.Exec;

    private Character _character; //座標
    private Transform _transform; //座標

    private List<Vector2Int> _route;

    private GameObject _managers;
    private MapManager _mapManager;
    private char[,] _map;
    private int[,] _costMap;

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        _managers = GameObject.Find("Managers");
        _mapManager = _managers.GetComponent<MapManager>();
        _character = this.gameObject.GetComponent<Character>();
        _transform = this.gameObject.transform;

        _map = _mapManager.map;
        _costMap = new int[_map.GetLength(0), _map.GetLength(1)];

        yield return new WaitForSeconds(0.1f);

        setRoute();
        Debug.Log(_route.Dump());
        yield return new WaitForSeconds(0.1f);
        // プレイヤーを移動させる.
        StartCoroutine("Move");
    }

    private void setRoute()
    {
        Vector2Int startPos = _character.pos;
        Vector2Int endPos = _mapManager.GetRandomCoord();
        Debug.Log(startPos);
        Debug.Log(endPos);
        AStar aStar = new AStar();

        for (int i = 0, size_i = _map.GetLength(0); i < size_i; i++)
        {
            for (int j = 0, size_j = _map.GetLength(1); j < size_j; j++)
            {
                var num = 0;
                switch (_map[i, j])
                {
                    case 'g':
                        num = 1;
                        break;

                    case 'w':
                        num = 1000;
                        break;

                    case 's':
                        num = 5;
                        break;
                }
                _costMap[i, j] = num;
            }
        }

        _route = aStar.Serch(startPos, endPos, _costMap);
    }

    private IEnumerator Move()
    {
        foreach (var p in _route)
        {
            _transform.DOMove(
                _mapManager.GetWorldPositionFromTile(p.x, p.y),
                0.2f
            ).SetEase(Ease.Linear);
            _character.setPos(p);
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(0.01f);

        setRoute();
        Debug.Log(_route.Dump());
        yield return new WaitForSeconds(0.01f);
        StartCoroutine("Move");
    }
}