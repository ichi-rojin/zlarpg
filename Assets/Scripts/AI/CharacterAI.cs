using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    private Transform _player; //Prefab

    private Vector2Int _pos;
    private List<Vector2Int> _route;

    private GameObject _managers;
    private MapManager _mapManager;

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        _managers = GameObject.Find("Managers");
        _mapManager = _managers.GetComponent<MapManager>();

        _player = this.gameObject.transform;
        char[,] map = _mapManager.map;
        int[,] costMap = new int[map.GetLength(0), map.GetLength(1)];

        yield return new WaitForSeconds(0.1f);

        {
            Vector2Int startPos = _pos;
            Vector2Int endPos = _mapManager.GetRandomCoord();
            Debug.Log(startPos);
            Debug.Log(endPos);
            AStar aStar = new AStar();

            for (int i = 0, size_i = map.GetLength(0); i < size_i; i++)
            {
                for (int j = 0, size_j = map.GetLength(1); j < size_j; j++)
                {
                    var num = 0;
                    switch (map[i, j])
                    {
                        case 'g':
                            num = 1;
                            break;

                        case 'w':
                            num = 100;
                            break;

                        case 's':
                            num = 10;
                            break;
                    }
                    costMap[i, j] = num;
                }
            }

            _route = aStar.Serch(startPos, endPos, costMap);
            yield return new WaitForSeconds(1f);
            Debug.Log(_route.Dump());
            // プレイヤーを移動させる.
            foreach (var p in _route)
            {
                _player.position = _mapManager.GetWorldPositionFromTile(p.x, p.y);
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    public void setPos(Vector2Int pos)
    {
        _pos = pos;
    }
}