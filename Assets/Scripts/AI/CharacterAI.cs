using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterAI : MonoBehaviour
{
    // èÛë‘.
    private enum eState
    {
        Exec, // é¿çsíÜ.
        Walk, // à⁄ìÆíÜ.
        End,  // Ç®ÇµÇ‹Ç¢.
    }

    private eState _state = eState.Exec;

    private Transform player; //Prefab

    private Vector2Int _pos;

    private GameObject managers;
    private MapManager mapManager;

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        managers = GameObject.Find("Managers");
        mapManager = managers.GetComponent<MapManager>();

        player = this.gameObject.transform;
        char[,] map = mapManager.map;

        yield return new WaitForSeconds(0.1f);

        {
            Vector2Int startPos = _pos;
            Vector2Int endPos = mapManager.GetRandomCoord();
            Debug.Log(startPos);
            Debug.Log(endPos);
            AStar aStar = new AStar();
            List<Vector2Int> route = aStar.Serch(startPos, endPos, map);
            Debug.Log(string.Join(", ", route.Select(obj => obj.ToString())));

            yield return new WaitForSeconds(0.01f);
        }
    }

    public void setPos(Vector2Int pos)
    {
        _pos = pos;
    }
}