using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private MapManager _mapManager;

    [SerializeField]
    private GameObject _treasureChestPrefab; //各種プレファブ

    [SerializeField]
    private Transform _itemsParent; //マップのゲームオブジェクト

    // Start is called before the first frame update
    private void Start()
    {
        _mapManager = this.gameObject.GetComponent<MapManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        // タップ検出処理
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mapPos = Input.mousePosition;
            Vector2 tilePos = _mapManager.GetNormalizeWorldPosition(mapPos.x, mapPos.y);
            Instantiate(_treasureChestPrefab, tilePos, Quaternion.Euler(0, 0, 0f), _itemsParent);
        }
    }
}