using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSettings", menuName = "ScriptableObjects/CharacterSettings")]
public class CharacterSettings : ScriptableObject
{
    //�L�����N�^�[�f�[�^
    public List<CharacterStats> datas;

    private static CharacterSettings instance;

    public static CharacterSettings Instance
    {
        get
        {
            if (!instance)
            {
                instance = Resources.Load<CharacterSettings>(nameof(CharacterSettings));
            }
            return instance;
        }
    }

    public CharacterStats Get(int id)
    {
        return datas.Find(item => item.Id == id).GetCopy<CharacterStats>();
    }

    // �L��������
    public GameObject CreateCharacter(int id, MapManager mapManager, Vector2Int pos, Transform parent)
    {
        // �X�e�[�^�X�擾
        CharacterStats stats = Instance.Get(id);
        Vector2 coord = mapManager.GetWorldPositionFromTile(pos.x, pos.y); // ��_�̍��W�����ɕϐ�pos��錾
        GameObject charaGameObject = Instantiate(stats.Prefab, coord, Quaternion.Euler(0, 0, 0f), parent);
        Character character = charaGameObject.GetComponent<Character>();
        character.SetPos(pos);
        character.Init(stats, mapManager);
        return charaGameObject;
    }
}

[Serializable]
public class CharacterStats : BaseStats
{
    public GameObject Prefab;

    // ���������t�H�[�XID
    public List<int> DefaultForceIds;

    // �����\�t�H�[�XID
    public List<int> UsableForceIds;

    // �����\��
    public int UsableForceMax;
}