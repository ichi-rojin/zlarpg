using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Vector2 _pos;

    private GameObject managers;
    private MapManager mapManager;

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        managers = GameObject.Find("Managers");
        mapManager = managers.GetComponent<MapManager>();

        player = this.gameObject.transform;
        print(mapManager.GetRandomPosition());

        yield return new WaitForSeconds(0.1f);

        {
            Vector2 startPos = _pos;
            print(startPos);
            Vector2 endPos = mapManager.GetRandomPosition();
            print(endPos);
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void setPos(Vector2 pos)
    {
        _pos = pos;
    }
}