using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class CharacterManager : MonoBehaviour
{
    private const int SPAWN_NUMBER = 1;//Charaのスポーン数

    [SerializeField]
    private GameObject _characterPrefab; //各種プレファブ

    [SerializeField]
    private Transform _parent; //マップのゲームオブジェクト

    private MapManager _mapManager;

    private void Start()
    {
        _mapManager = this.gameObject.GetComponent<MapManager>();

        for (int i = 0; i < SPAWN_NUMBER; i++)
        {
            Vector2Int coord = _mapManager.GetRandomCoord();
            Vector2 pos = _mapManager.GetWorldPositionFromTile(coord.x, coord.y); // 基点の座標を元に変数posを宣言
            GameObject charaGameObject = Instantiate(_characterPrefab, pos, Quaternion.Euler(0, 0, 0f), _parent);
            CharacterAI charaAI = charaGameObject.GetComponent<CharacterAI>();
            Character character = charaGameObject.GetComponent<Character>();
            character.SetPos(new Vector2Int(coord.x, coord.y));
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}