using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum DivideDirection
{
    Vertical, //�����i�c�j����
    Horizontal //�����i���j����
}
public enum CellType
{
    Path, //��
    Wall, //��
    BorderLine //���E�� 
}

public class RougeGenerator
{
    const int MIN_ROOM_SIZE = 4; //�����̍ŏ��T�C�Y
    const int MAX_ROOM_SIZE = 8; //�����̍ő�T�C�Y
    const int MIN_SPACE_BETWEEN_ROOM_AND_ROAD = 2; //�����Ɠ��Ƃ̗]��
    int width, height; //�}�b�v�S�̂̉����ƍ���
    int[,] map; //�}�b�v�f�[�^���i�[����z��
    List<Area> areaList; //�G���A���i�[���Ă���Area�^��List

    public RougeGenerator(int width, int height) //���Map�N���X����������󂯎��܂��B
    {
        this.width = width;
        this.height = height;

        map = new int[this.width, this.height];
    }

    /* �� */
    //�}�b�v����郁�C���̃��\�b�h�@int[,]�^�̃}�b�v�f�[�^��ԋp���܂��B
    public int[,] GenerateMap()
    {
        areaList = new List<Area>(); //�G���A���i�[����List�̏�����
        InitMap(); //�}�b�v���������\�b�h���s
        InitFirstArea(); //�ŏ��̃G���A�����߂郁�\�b�h���s

        //�G���A�𕪊����郁�\�b�h�����s �����̓����_����0��1��n���B0�̏ꍇ��true,1�̏ꍇ��false
        DivideArea(Random.Range(0, 2) == 0);
        return map; //�}�b�v�f�[�^��Ԃ�
    }

    //�ŏ��̃G���A����郁�\�b�h
    void InitFirstArea()
    {
        //�C���X�^���X����
        Area area = new Area();

        //�l���̍��W�o�^�i���A��A�E�A���j�̕��тŁi0,0,�}�b�v�S�̂̉���-1,�}�b�v�S�̂̏c��-1�j
        area.Section.SetPoints(0, 0, width - 1, height - 1);

        //���X�g�ɒǉ�
        areaList.Add(area);
    }

    //�}�b�v�f�[�^�����ׂĕǂŖ��߂ď��������郁�\�b�h
    void InitMap()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                map[x, y] = (int)CellType.Wall;
            }
        }
    }

    /*�@��������G���A�𕪊����čs������ */

    //�G���A�𕪊����郁�C���̃��\�b�h�@�����ɂǂ�������ɕ������邩�𔻒f����bool�^�̒l�����܂�
    void DivideArea(bool horizontalDivide)
    {
        //List�̖�������Area�����o���B�����InitFirstArea�ō��ꂽ�G���A
        Area parentArea = areaList.LastOrDefault();
        //�G���A���������return���ďI��
        if (parentArea == null) return;

        //List���������o�����G���A���폜����
        areaList.Remove(parentArea);

        //���������ɉ�����parentArea�𕪊�����@�Ԃ�l�Ɏq�G���A���󂯎��childArea�ɕۑ�
        Area childArea = horizontalDivide ? DivideHorizontally(parentArea) : DivideVertially(parentArea);

        //childArea��null�ł͂Ȃ��ꍇ�̏���
        if (childArea != null)
        {
            //�e�Ǝq�̋��E�����}�b�v�f�[�^�ɕۑ�����
            DrawBorder(parentArea);
            DrawBorder(childArea);

            //�e�Ǝq�̃G���A��Section�T�C�Y���r���A�傫���������̕����G���A�ɂ���
            if (parentArea.Section.Size > childArea.Section.Size)
            {
                //�e�G���A���傫���ꍇ��parentArea�����List�֕ۑ��@�����parentArea�����������
                areaList.Add(childArea);
                areaList.Add(parentArea);
            }
            else
            {
                //�q�G���A���傫���ꍇ��childArea�����List�֕ۑ��@�����childArea�����������
                areaList.Add(parentArea);
                areaList.Add(childArea);
            }
            //�ēx�����������s���@���̂Ƃ������������t�ɂ���
            DivideArea(!horizontalDivide);
        }
    }

    //���������ɃG���A�𕪊����郁�\�b�h�@�����Őe��Area���󂯎��A�q�G���A�ƂȂ�childArea��Ԃ��܂��B
    Area DivideHorizontally(Area area)
    {
        //�G���A�̍������������邽�߂ɏ\���ȍ������ǂ����`�F�b�N����
        if (!CheckRectSize(area.Section.Height))
        {
            //�����������s�\���ȏꍇ�͕��������AareaList�ɖ߂�
            areaList.Add(area);
            //null��Ԃ��ď������I������B
            return null;
        };

        //�G���A��Section�̏㉺��CalculateDivideLine���\�b�h�ɓn�������ʒu�����߂�
        int divideLine = CalculateDivideLine(area.Section.Top, area.Section.Bottom);
        //�q�G���A���쐬����
        Area childArea = new Area();

        //�q�G���ASection�̏㉺���E���W��o�^����
        childArea.Section.SetPoints(area.Section.Left, divideLine, area.Section.Right, area.Section.Bottom);
        //�e�G���A��Section�̉����W�𕪊����C���ɐݒ肷��
        area.Section.Bottom = divideLine;

        //�q�G���A��ԋp���ďI��
        return childArea;
    }

    //���������ɃG���A�𕪊����郁�\�b�h�@�����Őe��Area���󂯎��A�q�G���A�ƂȂ�childArea��Ԃ��܂��B
    Area DivideVertially(Area area)
    {
        //�G���A�̉������������邽�߂ɏ\���ȕ����ǂ����`�F�b�N����
        if (!CheckRectSize(area.Section.Width))
        {
            //���������s�\���ȏꍇ�͕��������AareaList�ɖ߂�
            areaList.Add(area);
            //null��Ԃ��ď������I������B
            return null;
        };

        //�G���A��Section�̍��E��CalculateDivideLine���\�b�h�ɓn�������ʒu�����߂�
        int divideLine = CalculateDivideLine(area.Section.Left, area.Section.Right);
        Area childArea = new Area();

        childArea.Section.SetPoints(divideLine, area.Section.Top, area.Section.Right, area.Section.Bottom);
        //�e�G���A��Section�̉E���W�𕪊����C���ɐݒ肷��
        area.Section.Right = divideLine;
        return childArea;
    }

    //�Z�N�V�����̃T�C�Y���`�F�b�N���郁�\�b�h ������int���󂯎��A�G���A�̃Z�N�V�����̑傫�����\�����𔻒f��bool�^��Ԃ��܂��B
    //Height,Width�̂����ꂩ�̒l����G���A�𕪊��ł��邩���`�F�b�N����
    bool CheckRectSize(int size)
    {
        //�����ɕK�v�ƂȂ�Œ���̑傫���v�Z
        //�ŏ��̕����T�C�Y�{���̃}�[�W����*2�i2�������邽�߁j�{1�i�����j
        int min = (MIN_ROOM_SIZE + MIN_SPACE_BETWEEN_ROOM_AND_ROAD) * 2 + 1;

        //�n���Ă���size�ƍŒ���̑傫�����r����bool��ԋp
        return size >= min;
    }

    //�������C�����v�Z���郁�\�b�h ������int��2���A�ǂ��𕪊����C���ɂ��邩�̌v�Z���ʂ�int�^�ŕԂ��܂��B
    //start��end���󂯎��
    int CalculateDivideLine(int start, int end)
    {
        //��������ŏ��l���v�Z
        //start�ɕ����̍ŏ��T�C�Y�ƕ����ƒʘH�܂ł̗]���𑫂��ĎZ�o
        int min = start + (MIN_ROOM_SIZE + MIN_SPACE_BETWEEN_ROOM_AND_ROAD);

        //��������ő�l���v�Z
        //end���畔���̍ŏ��T�C�Y�ƕ����ƒʘH�܂ł̗]���̍��v�������ĎZ�o
        int max = end - (MIN_ROOM_SIZE + MIN_SPACE_BETWEEN_ROOM_AND_ROAD);

        //�ŏ��l����ő�l�̊Ԃ������_���Ŏ擾��int��Ԃ�
        return Random.Range(min, max + 1);
    }

    //���E�����}�b�v�f�[�^�ɏ������ރ��\�b�h�@�m�F�p�Ȃ̂ōŏI�I�ɂ͕s�v�ɂȂ�
    void DrawBorder(Area area)
    {
        //�G���A�̃Z�N�V������Top����Bottom�܂Ń��[�v
        for (int y = area.Section.Top; y <= area.Section.Bottom; y++)
        {
            //�G���A�̃Z�N�V������Left����Right�܂Ń��[�v
            for (int x = area.Section.Left; x <= area.Section.Right; x++)
            {
                //x��y���Z�N�V�����̏㉺���E�Ɠ����Ȃ�΋��E������������
                if (x == area.Section.Left || x == area.Section.Right || y == area.Section.Top || y == area.Section.Bottom)
                {
                    map[x, y] = (int)CellType.BorderLine;
                }
            }
        }
    }
    void ConnectRooms()
    {
        for (int i = 0; i < areaList.Count - 1; i++)
        {
            Area parentArea = areaList[i];
            Area childArea = areaList[i + 1];
            CreateRoadBetweenAreas(parentArea, childArea);

            // �ǉ� ���G���A�Ƃ̐ڑ������݂�
            // i��areaList.Conunt-2�����������ꍇ�ɑ��G���A�擾�\
            if (i < areaList.Count - 2)
            {
                //���G���A�擾
                Area grandchildArea = areaList[i + 2];
                //�e�Ƒ��̐ڑ��֌W�𒲂ׂ�
                CreateRoadBetweenAreas(parentArea, grandchildArea, true);
            }
        }
    }
    //�������Ȃ����\�b�h
    void CreateRoadBetweenAreas(Area parentArea, Area childArea, bool isGrandchild = false)//������ǉ� �����l��false
    {
        if (parentArea.Section.Bottom == childArea.Section.Top || parentArea.Section.Top == childArea.Section.Bottom)
        {
            //CreateVerticalRoad���\�b�h�֑��t���O��n��
            CreateVerticalRoad(parentArea, childArea, isGrandchild);
        }
        else if (parentArea.Section.Right == childArea.Section.Left || parentArea.Section.Left == childArea.Section.Right)
        {
            //CreateHorizontalRoad���\�b�h�֑��t���O��n��
            CreateHorizontalRoad(parentArea, childArea, isGrandchild);
        }
        else //���Ɛڑ��ł��Ȃ������Ƃ��̊m�F
        {
            Debug.Log("���Ƃ̐ڑ��s�\");
        }
    }
    void CreateHorizontalRoad(Area parentArea, Area childArea, bool isGrandchild)
    {
        int yStart = isGrandchild && parentArea.Road != null ? parentArea.Road.Top : Random.Range(parentArea.Room.Top, parentArea.Room.Bottom);
        int yEnd = isGrandchild && childArea.Road != null ? childArea.Road.Top : Random.Range(childArea.Room.Top, childArea.Room.Bottom);
        int connectX = parentArea.Section.Right == childArea.Section.Left ? childArea.Section.Left : parentArea.Section.Left;
        if (parentArea.Section.Left > childArea.Section.Left)
        {
            parentArea.SetRoad(connectX, yStart, parentArea.Room.Left, yStart + 1);
            childArea.SetRoad(childArea.Room.Right, yEnd, connectX, yEnd + 1);
        }
        else
        {
            connectX = childArea.Section.Left;
            parentArea.SetRoad(parentArea.Room.Right, yStart, connectX, yStart + 1);
            childArea.SetRoad(connectX, yEnd, childArea.Room.Left, yEnd + 1);
        }
        DrawRoadFromRoomToConnectLine(parentArea);
        DrawRoadFromRoomToConnectLine(childArea);
        DrawHorizontalRoad(yStart, yEnd, connectX);
    }

    void CreateVerticalRoad(Area parentArea, Area childArea, bool isGrandchild)
    {
        //xStart�̎擾���@�ύX�@���ڑ����Őe�G���A��Road��null�łȂ���ΐe�G���A��Road.Left��xStart�ɂ���
        int xStart = isGrandchild && parentArea.Road != null ? parentArea.Road.Left : Random.Range(parentArea.Room.Left, parentArea.Room.Right);
        //xEnd�̎擾���@�ύX�@���ڑ����ő��G���A��Road��null�łȂ���ΐe�G���A��Road.Left��xStart�ɂ���
        int xEnd = isGrandchild && childArea.Road != null ? childArea.Road.Left : Random.Range(childArea.Room.Left, childArea.Room.Right);
        int connectY = parentArea.Section.Bottom == childArea.Section.Top ? childArea.Section.Top : parentArea.Section.Top;

        //��������ڑ������܂œ������
        if (parentArea.Section.Top > childArea.Section.Top)
        {
            parentArea.SetRoad(xStart, connectY, xStart + 1, parentArea.Room.Top);
            childArea.SetRoad(xEnd, childArea.Room.Bottom, xEnd + 1, connectY);
        }
        else
        {
            parentArea.SetRoad(xStart, parentArea.Room.Bottom, xStart + 1, connectY);
            childArea.SetRoad(xEnd, connectY, xEnd + 1, childArea.Room.Top);
        }
        //��������ڑ������܂ł𓹂ɂ���
        DrawRoadFromRoomToConnectLine(parentArea);
        DrawRoadFromRoomToConnectLine(childArea);

        //�ڑ������𓹂ɂ���
        DrawVerticalRoad(xStart, xEnd, connectY);
    }

    void DrawRoadFromRoomToConnectLine(Area area)
    {
        //�c�̃��[�v
        for (int y = 0; y < area.Road.Height; y++)
        {
            //���̃��[�v
            for (int x = 0; x < area.Road.Width; x++)
            {
                //�}�b�v�f�[�^�𓹂ɂ���
                map[x + area.Road.Left, y + area.Road.Top] = (int)CellType.Path;
            }
        }
    }
    void DrawVerticalRoad(int xStart, int xEnd, int y)
    {
        //x��1���[�v�݂̂Ŋ����B
        //�G���A�����ɂ���ĊJ�n�ʒu�ƏI���ʒu���t�ɂȂ��Ă���ꍇ������̂ł��ꂼ�ꔻ�肵�ă��[�v�J�n
        for (int x = Mathf.Min(xStart, xEnd); x <= Mathf.Max(xStart, xEnd); x++)
        {
            //�}�b�v�f�[�^���㏑��
            map[x, y] = (int)CellType.Path;
        }
    }

    void DrawHorizontalRoad(int yStart, int yEnd, int x)
    {
        for (int y = Mathf.Min(yStart, yEnd); y <= Mathf.Max(yStart, yEnd); y++)
        {
            map[x, y] = (int)CellType.Path;
        }
    }
}