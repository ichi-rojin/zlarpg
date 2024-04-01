using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{
    public static void dumpCharArray(char[,] data)
    {
        for (int i = 0; i < data.GetLength(0); i++)
        {
            string str = "";
            for (int j = 0; j < data.GetLength(1); j++)
            {
                str = str + data[j, i] + " ";
            }
            Debug.Log(str);
        }
    }
}