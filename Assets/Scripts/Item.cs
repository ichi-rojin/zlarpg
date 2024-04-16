using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Token
{
    public Dictionary<string, int> Provide()
    {
        var provide = new Dictionary<string, int>();
        provide.Add("UpAttack", 10);
        return provide;
    }
}