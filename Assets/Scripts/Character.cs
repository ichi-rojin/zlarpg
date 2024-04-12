using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Token
{
    [SerializeField]
    private int _sense; //ímäoî\óÕ

    public int sense
    {
        get { return _sense; }
    }

    // èÛë‘.
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

    // Start is called before the first frame update
    private void Start()
    {
        _sense = 3;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void SetOrientation(eOrientation orientation)
    {
        _orientation = orientation;
    }
}