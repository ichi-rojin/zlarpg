using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum DivideDirection
{
    Vertical, //垂直（縦）方向
    Horizontal //水平（横）方向
}
public enum CellType
{
    Path, //空白
    Wall, //壁
    BorderLine //境界線 
}

public class RougeGenerator
{
    const int MIN_ROOM_SIZE = 4; //部屋の最小サイズ
    const int MAX_ROOM_SIZE = 8; //部屋の最大サイズ
    const int MIN_SPACE_BETWEEN_ROOM_AND_ROAD = 2; //部屋と道との余白
    int width, height; //マップ全体の横幅と高さ
    int[,] map; //マップデータを格納する配列
    List<Area> areaList; //エリアを格納しておくArea型のList

    public RougeGenerator(int width, int height) //後にMapクラスから引数を受け取ります。
    {
        this.width = width;
        this.height = height;

        map = new int[this.width, this.height];
    }

    /* 略 */
    //マップを作るメインのメソッド　int[,]型のマップデータを返却します。
    public int[,] GenerateMap()
    {
        areaList = new List<Area>(); //エリアを格納するListの初期化
        InitMap(); //マップ初期化メソッド実行
        InitFirstArea(); //最初のエリアを決めるメソッド実行

        //エリアを分割するメソッドを実行 引数はランダムで0か1を渡す。0の場合はtrue,1の場合はfalse
        DivideArea(Random.Range(0, 2) == 0);
        return map; //マップデータを返す
    }

    //最初のエリアを作るメソッド
    void InitFirstArea()
    {
        //インスタンス生成
        Area area = new Area();

        //四隅の座標登録（左、上、右、下）の並びで（0,0,マップ全体の横幅-1,マップ全体の縦幅-1）
        area.Section.SetPoints(0, 0, width - 1, height - 1);

        //リストに追加
        areaList.Add(area);
    }

    //マップデータをすべて壁で埋めて初期化するメソッド
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

    /*　ここからエリアを分割して行く処理 */

    //エリアを分割するメインのメソッド　引数にどちら向きに分割するかを判断するbool型の値を取ります
    void DivideArea(bool horizontalDivide)
    {
        //Listの末尾からAreaを取り出す。初回はInitFirstAreaで作られたエリア
        Area parentArea = areaList.LastOrDefault();
        //エリアが無ければreturnして終了
        if (parentArea == null) return;

        //Listから先程取り出したエリアを削除する
        areaList.Remove(parentArea);

        //分割方向に応じてparentAreaを分割する　返り値に子エリアを受け取りchildAreaに保存
        Area childArea = horizontalDivide ? DivideHorizontally(parentArea) : DivideVertially(parentArea);

        //childAreaがnullではない場合の処理
        if (childArea != null)
        {
            //親と子の境界線をマップデータに保存する
            DrawBorder(parentArea);
            DrawBorder(childArea);

            //親と子のエリアのSectionサイズを比較し、大きい方を次の分割エリアにする
            if (parentArea.Section.Size > childArea.Section.Size)
            {
                //親エリアが大きい場合はparentAreaを後にListへ保存　次回はparentAreaが分割される
                areaList.Add(childArea);
                areaList.Add(parentArea);
            }
            else
            {
                //子エリアが大きい場合はchildAreaを後にListへ保存　次回はchildAreaが分割される
                areaList.Add(parentArea);
                areaList.Add(childArea);
            }
            //再度分割処理を行う　このとき分割方向を逆にする
            DivideArea(!horizontalDivide);
        }
    }

    //水平方向にエリアを分割するメソッド　引数で親のAreaを受け取り、子エリアとなるchildAreaを返します。
    Area DivideHorizontally(Area area)
    {
        //エリアの高さが分割するために十分な高さかどうかチェックする
        if (!CheckRectSize(area.Section.Height))
        {
            //もし高さが不十分な場合は分割せず、areaListに戻す
            areaList.Add(area);
            //nullを返して処理を終了する。
            return null;
        };

        //エリアのSectionの上下をCalculateDivideLineメソッドに渡し分割位置を決める
        int divideLine = CalculateDivideLine(area.Section.Top, area.Section.Bottom);
        //子エリアを作成する
        Area childArea = new Area();

        //子エリアSectionの上下左右座標を登録する
        childArea.Section.SetPoints(area.Section.Left, divideLine, area.Section.Right, area.Section.Bottom);
        //親エリアのSectionの下座標を分割ラインに設定する
        area.Section.Bottom = divideLine;

        //子エリアを返却して終了
        return childArea;
    }

    //垂直方向にエリアを分割するメソッド　引数で親のAreaを受け取り、子エリアとなるchildAreaを返します。
    Area DivideVertially(Area area)
    {
        //エリアの横幅が分割するために十分な幅かどうかチェックする
        if (!CheckRectSize(area.Section.Width))
        {
            //もし幅が不十分な場合は分割せず、areaListに戻す
            areaList.Add(area);
            //nullを返して処理を終了する。
            return null;
        };

        //エリアのSectionの左右をCalculateDivideLineメソッドに渡し分割位置を決める
        int divideLine = CalculateDivideLine(area.Section.Left, area.Section.Right);
        Area childArea = new Area();

        childArea.Section.SetPoints(divideLine, area.Section.Top, area.Section.Right, area.Section.Bottom);
        //親エリアのSectionの右座標を分割ラインに設定する
        area.Section.Right = divideLine;
        return childArea;
    }

    //セクションのサイズをチェックするメソッド 引数にintを受け取り、エリアのセクションの大きさが十分かを判断しbool型を返します。
    //Height,Widthのいずれかの値からエリアを分割できるかをチェックする
    bool CheckRectSize(int size)
    {
        //分割に必要となる最低限の大きさ計算
        //最小の部屋サイズ＋区画のマージンを*2（2分割するため）＋1（道幅）
        int min = (MIN_ROOM_SIZE + MIN_SPACE_BETWEEN_ROOM_AND_ROAD) * 2 + 1;

        //渡ってきたsizeと最低限の大きさを比較してboolを返却
        return size >= min;
    }

    //分割ラインを計算するメソッド 引数にintを2つ取り、どこを分割ラインにするかの計算結果をint型で返します。
    //startとendを受け取る
    int CalculateDivideLine(int start, int end)
    {
        //分割する最小値を計算
        //startに部屋の最小サイズと部屋と通路までの余白を足して算出
        int min = start + (MIN_ROOM_SIZE + MIN_SPACE_BETWEEN_ROOM_AND_ROAD);

        //分割する最大値を計算
        //endから部屋の最小サイズと部屋と通路までの余白の合計を引いて算出
        int max = end - (MIN_ROOM_SIZE + MIN_SPACE_BETWEEN_ROOM_AND_ROAD);

        //最小値から最大値の間をランダムで取得しintを返す
        return Random.Range(min, max + 1);
    }

    //境界線をマップデータに書き込むメソッド　確認用なので最終的には不要になる
    void DrawBorder(Area area)
    {
        //エリアのセクションのTopからBottomまでループ
        for (int y = area.Section.Top; y <= area.Section.Bottom; y++)
        {
            //エリアのセクションのLeftからRightまでループ
            for (int x = area.Section.Left; x <= area.Section.Right; x++)
            {
                //xとyがセクションの上下左右と同じならば境界線を書き込む
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

            // 追加 孫エリアとの接続を試みる
            // iがareaList.Conunt-2よりも小さい場合に孫エリア取得可能
            if (i < areaList.Count - 2)
            {
                //孫エリア取得
                Area grandchildArea = areaList[i + 2];
                //親と孫の接続関係を調べる
                CreateRoadBetweenAreas(parentArea, grandchildArea, true);
            }
        }
    }
    //部屋をつなぐメソッド
    void CreateRoadBetweenAreas(Area parentArea, Area childArea, bool isGrandchild = false)//引数一つ追加 初期値はfalse
    {
        if (parentArea.Section.Bottom == childArea.Section.Top || parentArea.Section.Top == childArea.Section.Bottom)
        {
            //CreateVerticalRoadメソッドへ孫フラグを渡す
            CreateVerticalRoad(parentArea, childArea, isGrandchild);
        }
        else if (parentArea.Section.Right == childArea.Section.Left || parentArea.Section.Left == childArea.Section.Right)
        {
            //CreateHorizontalRoadメソッドへ孫フラグを渡す
            CreateHorizontalRoad(parentArea, childArea, isGrandchild);
        }
        else //孫と接続できなかったときの確認
        {
            Debug.Log("孫との接続不可能");
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
        //xStartの取得方法変更　孫接続時で親エリアのRoadがnullでなければ親エリアのRoad.LeftをxStartにする
        int xStart = isGrandchild && parentArea.Road != null ? parentArea.Road.Left : Random.Range(parentArea.Room.Left, parentArea.Room.Right);
        //xEndの取得方法変更　孫接続時で孫エリアのRoadがnullでなければ親エリアのRoad.LeftをxStartにする
        int xEnd = isGrandchild && childArea.Road != null ? childArea.Road.Left : Random.Range(childArea.Room.Left, childArea.Room.Right);
        int connectY = parentArea.Section.Bottom == childArea.Section.Top ? childArea.Section.Top : parentArea.Section.Top;

        //部屋から接続部分まで道を作る
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
        //部屋から接続部分までを道にする
        DrawRoadFromRoomToConnectLine(parentArea);
        DrawRoadFromRoomToConnectLine(childArea);

        //接続部分を道にする
        DrawVerticalRoad(xStart, xEnd, connectY);
    }

    void DrawRoadFromRoomToConnectLine(Area area)
    {
        //縦のループ
        for (int y = 0; y < area.Road.Height; y++)
        {
            //横のループ
            for (int x = 0; x < area.Road.Width; x++)
            {
                //マップデータを道にする
                map[x + area.Road.Left, y + area.Road.Top] = (int)CellType.Path;
            }
        }
    }
    void DrawVerticalRoad(int xStart, int xEnd, int y)
    {
        //xの1ループのみで完結。
        //エリア分割によって開始位置と終了位置が逆になっている場合があるのでそれぞれ判定してループ開始
        for (int x = Mathf.Min(xStart, xEnd); x <= Mathf.Max(xStart, xEnd); x++)
        {
            //マップデータを上書き
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