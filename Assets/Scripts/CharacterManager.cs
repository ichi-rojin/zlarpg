using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class CharacterManager : MonoBehaviour
{
    [SerializeField]
    private GameObject characterPrefab; //�e��v���t�@�u

    [SerializeField]
    private Transform parent; //�}�b�v�̃Q�[���I�u�W�F�N�g

    private MapManager mapManager;

    private void Start()
    {
        mapManager = this.gameObject.GetComponent<MapManager>();
        Vector2Int coord = mapManager.GetRandomCoord();
        Vector2 pos = mapManager.GetWorldPositionFromTile(coord.x, coord.y); // ��_�̍��W�����ɕϐ�pos��錾

        GameObject chara = Instantiate(characterPrefab, pos, Quaternion.Euler(0, 0, 0f), parent);
        CharacterAI charaAI = chara.GetComponent<CharacterAI>();
        charaAI.setPos(new Vector2Int(coord.x, coord.y));
    }

    // Update is called once per frame
    private void Update()
    {
    }
}