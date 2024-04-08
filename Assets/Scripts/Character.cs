using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private Vector2Int _pos;

    // プロパティ
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

    public void setPos(Vector2Int pos)
    {
        _pos = pos;
    }
}