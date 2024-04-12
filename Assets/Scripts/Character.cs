using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private int _sense; //知覚能力

    // 状態.
    public enum eOrientation
    {
        East,
        West,
        South,
        North,
    }

    private eOrientation _orientation = eOrientation.South;

    public eOrientation orientation
    {
        get { return _orientation; }
    }

    // 位置
    private Vector2Int _pos;

    public Vector2Int pos
    {
        get { return _pos; }
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void SetPos(Vector2Int pos)
    {
        _pos = pos;
    }

    public void SetOrientation(eOrientation orientation)
    {
        _orientation = orientation;
    }
}