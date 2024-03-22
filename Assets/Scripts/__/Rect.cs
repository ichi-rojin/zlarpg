
//���A�����A�ʘH�Ȃǂ̋�`�p�̃N���X
public class Rect
{
    int top = 0; //��`�̏���W
    int right = 0; //��`�̉E���W
    int bottom = 0; //��`�̉����W
    int left = 0; //��`�̍����W

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
    //���̉����i�E���獶�������j
    public int Width { get => right - left; }
    //���̍����i���������Ђ��j
    public int Height { get => bottom - top; }
    //��`�̖ʐ�
    public int Size { get => Width * Height; }
    //�R���X�g���N�^
    public Rect(int left = 0, int top = 0, int right = 0, int bottom = 0)
    {
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
    }
    //��`�̍��W���w�肷�郁�\�b�h
    public void SetPoints(int left, int top, int right, int bottom)
    {
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
    }
}
