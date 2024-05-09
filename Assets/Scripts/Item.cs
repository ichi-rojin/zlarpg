using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Item : Token
{
    private StatsType _status;

    // Start is called before the first frame update
    private void Start()
    {
        _status = new List<StatsType>().GetRandomByEnum();
    }

    public Dictionary<StatsType, int> Provide()
    {
        var provide = new Dictionary<StatsType, int>();
        provide.Add(_status, _status.GetUpRandomValue());
        return provide;
    }
}