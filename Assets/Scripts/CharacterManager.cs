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
            Vector2Int coord = _mapManager.GetRandomCoord();
            Vector2 pos = _mapManager.GetWorldPositionFromTile(coord.x, coord.y); // 基点の座標を元に変数posを宣言
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