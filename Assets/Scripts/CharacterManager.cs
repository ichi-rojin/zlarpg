using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class CharacterManager : MonoBehaviour
{
    private const int SPAWN_NUMBER = 2;//Chara�̃X�|�[����

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
            int characterId = 0;
            GameObject charaGameObject = CharacterSettings.Instance.CreateCharacter(characterId, pos, _parent);
            Character character = charaGameObject.GetComponent<Character>();

            character.stats.UpValue(StatsType.MaxHp, StatsType.MaxHp.GetInitialBonus());
            character.stats.Hp = character.stats.MaxHp;
            character.stats.UpValue(StatsType.Sense, StatsType.Sense.GetInitialBonus());
            character.stats.UpValue(StatsType.Strength, StatsType.Strength.GetInitialBonus());
            character.stats.UpValue(StatsType.Speed, StatsType.Speed.GetInitialBonus());
            character.stats.UpValue(StatsType.Jump, StatsType.Jump.GetInitialBonus());

            character.SetPos(new Vector2Int(coord.x, coord.y));
        }
    }
}