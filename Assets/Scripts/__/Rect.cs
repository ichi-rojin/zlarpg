
//区画、部屋、通路などの矩形用のクラス
public class Rect
{
    int top = 0; //矩形の上座標
    int right = 0; //矩形の右座標
    int bottom = 0; //矩形の下座標
    int left = 0; //矩形の左座標

    public int Top
    {
        get { return top; }
        set { top = value; }
    }
    public int Right
    {
        get { return right; }
        set { right = value; }
    }
    public int Bottom
    {
        get { return bottom; }
        set { bottom = value; }
    }
    public int Left
    {
        get { return left; }
        set { left = value; }
    }
    //区画の横幅（右から左を引く）
    public int Width { get => right - left; }
    //区画の高さ（下から上をひく）
    public int Height { get => bottom - top; }
    //矩形の面積
    public int Size { get => Width * Height; }
    //コンストラクタ
    public Rect(int left = 0, int top = 0, int right = 0, int bottom = 0)
    {
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
    }
    //矩形の座標を指定するメソッド
    public void SetPoints(int left, int top, int right, int bottom)
    {
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
    }
}
