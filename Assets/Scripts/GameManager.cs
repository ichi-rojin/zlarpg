using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private MapManager _mapManager;

    [SerializeField]
    private GameObject _treasureChestPrefab; //各種プレファブ

    [SerializeField]
    private Transform _itemsParent; //アイテムの親ゲームオブジェクト

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
            var map = _mapManager.map;
            Vector2Int tileIndex = _mapManager.GetTilePosFromWorldPosition(mapPos.x, mapPos.y);
            if (map[tileIndex.x, tileIndex.y] == 'w') return;

            Vector2 tilePos = _mapManager.GetNormalizeWorldPosition(mapPos.x, mapPos.y);
            GameObject treasureChest = Instantiate(_treasureChestPrefab, tilePos, Quaternion.Euler(0, 0, 0f), _itemsParent);
            treasureChest.GetComponent<Item>().SetPos(tileIndex);
        }
    }
}