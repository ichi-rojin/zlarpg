using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField]
    GameObject characterPrefab; //�e��v���t�@�u

    [SerializeField]
    private Transform parent; //�}�b�v�̃Q�[���I�u�W�F�N�g


    // Start is called before the first frame update
    void Start()
    {
        Vector2 pos = new Vector2(0.0f, 0.0f); // ��_�̍��W�����ɕϐ�pos��錾

        Instantiate(characterPrefab, pos, Quaternion.Euler(0, 0, 0f), parent.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
