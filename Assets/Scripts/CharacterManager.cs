using System;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class CharacterManager : MonoBehaviour
{
    private const int SPAWN_NUMBER = 8;//Charaのスポーン数

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

            foreach (StatsType Value in Enum.GetValues(typeof(StatsType)))
            {
                character.maxStats.UpValue(Value, Value.GetInitialBonus());
                character.stats[Value] = character.maxStats[Value];
            }
        }
    }
}