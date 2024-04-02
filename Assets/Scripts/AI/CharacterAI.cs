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
    List<Vector2Int> _route;

    private GameObject _managers;
    private MapManager _mapManager;

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        _managers = GameObject.Find("Managers");
        _mapManager = _managers.GetComponent<MapManager>();

        _player = this.gameObject.transform;
        char[,] map = _mapManager.map;

        yield return new WaitForSeconds(0.1f);

        {
            Vector2Int startPos = _pos;
            Vector2Int endPos = _mapManager.GetRandomCoord();
            Debug.Log(startPos);
            Debug.Log(endPos);
            AStar aStar = new AStar();
            _route = aStar.Serch(startPos, endPos, map);

            yield return new WaitForSeconds(0.01f);
        }
    }

    public void setPos(Vector2Int pos)
    {
        _pos = pos;
    }
}