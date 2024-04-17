using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Item : Token
{
    private StatusType _status;

    // Start is called before the first frame update
    private void Start()
    {
        _status = new List<StatusType>().GetRandomByEnum();
    }

    public Dictionary<string, int> Provide()
    {
        var provide = new Dictionary<string, int>();
        provide.Add(_status.GetUpMethod(), _status.GetUpRandomValue());
        return provide;
    }
}