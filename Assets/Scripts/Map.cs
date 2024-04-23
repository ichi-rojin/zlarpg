using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : Token
{
    private List<Character> Characters;//マップチップ上に存在するキャラクターのリスト

    //キャラクターが侵入する際の挙動
    public bool GetAdvancePermission(Character character)
    {
        //キャラクターに進出能力があるかチェック
        var AdvancePermission = CheckAdvanceAbility(character);
        return AdvancePermission;
    }

    private bool CheckAdvanceAbility(Character character)
    {
        var charaAI = character.GetComponent<CharacterAI>();
        var costMap = charaAI.costMap;
        var cost = costMap[pos.x, pos.y];
        if (cost <= character.jump) return true;
        return false;
    }
}