using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Token : MonoBehaviour
{
    // ˆÊ’u
    [SerializeField]
    private Vector2Int _pos;

    public Vector2Int pos
    {
        get { return _pos; }
    }

    public void SetPos(Vector2Int pos)
    {
        _pos = pos;
    }

    public void Vanish()
    {
        Destroy(this.gameObject);
    }
}