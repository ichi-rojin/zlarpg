using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : Token
{
    private List<Character> Characters;//�}�b�v�`�b�v��ɑ��݂���L�����N�^�[�̃��X�g

    //�L�����N�^�[���N������ۂ̋���
    public bool GetAdvancePermission(Character character)
    {
        //�L�����N�^�[�ɐi�o�\�͂����邩�`�F�b�N
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