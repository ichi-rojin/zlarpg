using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseForceSpawner : MonoBehaviour
{
    public GameObject PrefabForce;
    public GameObject _parent;
    public Token _parentToken;
    protected GameObject _forcesParent;
    protected ForceSpawnerStats _stats;

    public ForceSpawnerStats stats
    {
        get { return _stats; }
    }

    internal MapManager _mapManager;
    private Character _character;

    [SerializeField]
    [Header("クールタイム")]
    protected int _coolTime;

    protected List<BaseForce> _forces;

    private void FixedUpdate()
    {
        decreaseCooltime();
    }

    private void decreaseCooltime()
    {
        if (_coolTime <= 0) return;
        _coolTime -= 1;
    }

    public virtual void Init(ForceSpawnerStats stats)
    {
        _parent = transform.parent.gameObject;
        _character = _parent.GetComponent<Character>();
        _mapManager = _character._mapManager;
        _parentToken = _parent.GetComponent<Token>();
        _forcesParent = GameObject.Find("Forces");
        _forces = new List<BaseForce>();
        _stats = stats;
    }

    protected BaseForce CreateForce(Vector2 coord, Vector2Int forward, Character target = null, Transform parent = null)
    {
        GameObject obj = Instantiate(PrefabForce, coord, PrefabForce.transform.rotation, parent);
        BaseForce force = obj.GetComponent<BaseForce>();
        force.Init(this, forward);
        force.SetPos(_parentToken.pos);
        _forces.Add(force);

        return force;
    }

    protected BaseForce CreateForce(Vector2 coord, Character target = null, Transform parent = null)
    {
        return CreateForce(coord, Vector2Int.zero, target, parent);
    }

    public virtual void Action(Vector2 coord, Character target)
    {
    }
}