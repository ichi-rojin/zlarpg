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

    public Vector2Int GetNormalizePosition(int addX, int addY)
    {
        Vector2Int current = new Vector2Int(this.pos.x, this.pos.y);
        current.x += addX;
        current.y += addY;
        return current;
    }

    public void Vanish()
    {
        Destroy(this.gameObject);
    }
}