using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ForceSpawnerSettings", menuName = "ScriptableObjects/ForceSpawnerSettings")]
public class ForceSpawnerSettings : ScriptableObject
{
    //�L�����N�^�[�f�[�^
    public List<ForceSpawnerStats> datas;

    private static ForceSpawnerSettings instance;

    public static ForceSpawnerSettings Instance
    {
        get
        {
            if (!instance)
            {
                instance = Resources.Load<ForceSpawnerSettings>(nameof(ForceSpawnerSettings));
            }
            return instance;
        }
    }

    public ForceSpawnerStats Get(int id, int lv)
    {
        //�w�肳�ꂽ���x���̃f�[�^���Ȃ���Έ�ԍ������x���̃f�[�^��Ԃ�
        ForceSpawnerStats ret = null;

        foreach (var force in datas)
        {
            if (id != force.Id) continue;
            //�w�背�x���ƈ�v
            if (lv == force.Lv)
            {
                return (ForceSpawnerStats)force.GetCopy();
            }

            //���̃f�[�^���Z�b�g����Ă��Ȃ����A����𒴂��郌�x���������������ւ���
            if (ret == null)
            {
                ret = force;
            }
            else if (force.Lv < lv && ret.Lv < force.Lv)
            {
                ret = force;
            }
        }
        return (ForceSpawnerStats)ret.GetCopy();
    }

    // �t�H�[�X����
    public BaseForceSpawner CreateForceSpawner(int id, MapManager mapManager, Vector2Int pos, Transform parent = null)
    {
        // �f�[�^�擾
        ForceSpawnerStats stats = Instance.Get(id, 1);
        Vector2 coord = mapManager.GetWorldPositionFromTile(pos.x, pos.y); // ��_�̍��W�����ɕϐ�pos��錾
        GameObject forceGameObject = Instantiate(stats.Prefab, coord, Quaternion.Euler(0, 0, 0f), parent);
        BaseForceSpawner spawner = forceGameObject.GetComponent<BaseForceSpawner>();
        spawner.Init(stats);
        return spawner;
    }
}

[Serializable]
public class ForceSpawnerStats : BaseForceSpawnerStats
{
    public GameObject Prefab;
}