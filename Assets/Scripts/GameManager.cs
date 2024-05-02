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

    [SerializeField]
    private Transform _charactersParent; //アイテムの親ゲームオブジェクト

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

        ShowSensedArea();
    }

    private void ShowSensedArea()
    {
        GameObject[,] mapTips = _mapManager.mapTips;
        List<Vector2Int> senses = new List<Vector2Int>();

        List<CharacterAI> charas = new List<CharacterAI>();
        _charactersParent.GetComponentsInChildren(charas);
        foreach (var chara in charas)
        {
            foreach (var p in chara.GetComponent<CharacterAI>().sensed)
            {
                senses.Add(p);
            }
        }

        for (var d1 = 0; d1 < mapTips.GetLength(0); d1++)
        {
            for (var d2 = 0; d2 < mapTips.GetLength(1); d2++)
            {
                var p = new Vector2Int(d1, d2);
                var findedMapTip = _mapManager.GetMapTip(p);
                var mapSpRdr = findedMapTip.GetComponent<SpriteRenderer>();
                if (senses.Contains(p))
                {
                    mapSpRdr.color = new Color(0f, 255f, 255f);
                }
                else
                {
                    mapSpRdr.color = new Color(255f, 255f, 255f);
                }
            }
        }
    }
}