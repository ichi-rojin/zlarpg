
//区画、部屋、通路などの矩形用のクラス
public class Area
{
    private Rect section; //エリアの大枠の矩形
    private Rect room;  //エリアの部屋の矩形
    private Rect road;  //エリアの道の矩形
    public Rect Section { get => section; set => section = value; }
    public Rect Room { get => room; set => room = value; }
    public Rect Road { get => road; set => road = value; }

    //コンストラクタ
    public Area()
    {
        section = new Rect(); //エリア大枠矩形を作る（初期値は上下左右0）
        room = new Rect(); //エリアの部屋矩形を作る（初期値は上下左右0）
        road = null; //エリアの道矩形を作る（初期値はnull）
    }
    //道の矩形を作るメソッド
    public void SetRoad(int left, int top, int right, int bottom)
    {
        road = new Rect(left, top, right, bottom);
    }
}
