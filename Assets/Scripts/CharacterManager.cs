using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class CharacterManager : MonoBehaviour
{
    private const int SPAWN_NUMBER = 2;//Charaのスポーン数

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
            Vector2Int pos = _mapManager.GetRandomCoord();
            int characterId = 0;
            GameObject charaGameObject = CharacterSettings.Instance.CreateCharacter(characterId, _mapManager, pos, _parent);
            Character character = charaGameObject.GetComponent<Character>();

            character.stats.UpValue(StatsType.MaxHp, StatsType.MaxHp.GetInitialBonus());
            character.stats.Hp = character.stats.MaxHp;
            character.stats.UpValue(StatsType.Sense, StatsType.Sense.GetInitialBonus());
            character.stats.UpValue(StatsType.Strength, StatsType.Strength.GetInitialBonus());
            character.stats.UpValue(StatsType.Speed, StatsType.Speed.GetInitialBonus());
            character.stats.UpValue(StatsType.Jump, StatsType.Jump.GetInitialBonus());
        }
    }
}