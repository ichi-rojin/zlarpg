using System.Collections.Generic;
using UnityEngine;

public class Character : Token
{
    [SerializeField]
    [Header("����X�e�[�^�X")]
    private CharacterStats _maxStats;

    public CharacterStats maxStats
    {
        get { return _maxStats; }
    }

    [SerializeField]
    [Header("���݃X�e�[�^�X")]
    private CharacterStats _stats;

    public CharacterStats stats
    {
        get { return _stats; }
    }

    // ���.
    public enum eOrientation
    {
        East,
        West,
        South,
        North,
    }

    [SerializeField]
    [Header("�̂̌���")]
    private eOrientation _orientation = eOrientation.South;

    public eOrientation orientation
    {
        get { return _orientation; }
    }

    private List<BaseForceSpawner> _forceSpawners;

    public List<BaseForceSpawner> forceSpawners
    {
        get { return _forceSpawners; }
    }

    private MapManager _mapManager;

    private Animator _animator;

    // Start is called before the first frame update

    public void Init(CharacterStats maxStats, MapManager mapManager)
    {
        _animator = GetComponent<Animator>();
        _maxStats = maxStats;
        _stats = _maxStats.GetCopy<CharacterStats>();
        _forceSpawners = new List<BaseForceSpawner>();
        _mapManager = mapManager;
        AddForces(stats);
    }

    public void AddForces(CharacterStats stats)
    {
        //�t�H�[�X�f�[�^�Z�b�g
        foreach (var forceId in stats.DefaultForceIds)
        {
            AddForceSpawner(forceId);
        }
    }

    public void SetOrientation(eOrientation orientation)
    {
        if (_orientation == orientation) return;
        _orientation = orientation;
        switch (_orientation)
        {
            case eOrientation.East:
                _animator.SetTrigger("isRight");
                break;

            case eOrientation.West:
                _animator.SetTrigger("isLeft");
                break;

            case eOrientation.South:
                _animator.SetTrigger("isDown");
                break;

            case eOrientation.North:
                _animator.SetTrigger("isUp");
                break;

            default:
                _animator.SetTrigger("isUp");
                break;
        }
    }

    public void Damage(int attack, Character aggressor)
    {
        _stats[StatsType.Hp] -= attack;
        if (_stats[StatsType.Hp] <= 0)
        {
            Vanish();
        }
    }

    //�t�H�[�X��ǉ�
    private void AddForceSpawner(int id)
    {
        //�yTODO�z�����ς݂Ȃ烌�x���A�b�v
        BaseForceSpawner spawner = _forceSpawners.Find(force => force._stats.Id == id);

        if (spawner) return;

        //�V�K�ǉ�
        spawner = ForceSpawnerSettings.Instance.CreateForceSpawner(id, _mapManager, pos, this.transform);

        if (spawner == null)
        {
            Debug.LogError("�t�H�[�X�f�[�^������܂���");
            return;
        }

        //�����ς݃��X�g�֒ǉ�
        _forceSpawners.Add(spawner);
    }
}