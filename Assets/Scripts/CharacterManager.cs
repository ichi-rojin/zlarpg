using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField]
    GameObject characterPrefab; //各種プレファブ

    [SerializeField]
    private Transform parent; //マップのゲームオブジェクト


    // Start is called before the first frame update
    void Start()
    {
        Vector2 pos = new Vector2(0.0f, 0.0f); // 基点の座標を元に変数posを宣言

        Instantiate(characterPrefab, pos, Quaternion.Euler(0, 0, 0f), parent.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
