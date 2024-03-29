using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class CharacterManager : MonoBehaviour
{
    [SerializeField]
    private GameObject characterPrefab; //各種プレファブ

    [SerializeField]
    private Transform parent; //マップのゲームオブジェクト

    private MapManager mapManager;

    private void Start()
    {
        mapManager = this.gameObject.GetComponent<MapManager>();
        Vector2 pos = mapManager.GetRandomPosition(); // 基点の座標を元に変数posを宣言

        GameObject chara = Instantiate(characterPrefab, pos, Quaternion.Euler(0, 0, 0f), parent);
        CharacterAI charaAI = chara.GetComponent<CharacterAI>();
        charaAI.setPos(pos);
    }

    // Update is called once per frame
    private void Update()
    {
    }
}