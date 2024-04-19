using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class CharacterManager : MonoBehaviour
{
    private const int SPAWN_NUMBER = 1;//Chara�̃X�|�[����

    [SerializeField]
    private GameObject _characterPrefab; //�e��v���t�@�u

    [SerializeField]
    private Transform _parent; //�}�b�v�̃Q�[���I�u�W�F�N�g

    private MapManager _mapManager;

    private void Start()
    {
        _mapManager = this.gameObject.GetComponent<MapManager>();

        for (int i = 0; i < SPAWN_NUMBER; i++)
        {
            Vector2Int coord = _mapManager.GetRandomCoord();
            Vector2 pos = _mapManager.GetWorldPositionFromTile(coord.x, coord.y); // ��_�̍��W�����ɕϐ�pos��錾
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