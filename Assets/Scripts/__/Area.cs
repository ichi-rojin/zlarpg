
//���A�����A�ʘH�Ȃǂ̋�`�p�̃N���X
public class Area
{
    private Rect section; //�G���A�̑�g�̋�`
    private Rect room;  //�G���A�̕����̋�`
    private Rect road;  //�G���A�̓��̋�`
    public Rect Section { get => section; set => section = value; }
    public Rect Room { get => room; set => room = value; }
    public Rect Road { get => road; set => road = value; }

    //�R���X�g���N�^
    public Area()
    {
        section = new Rect(); //�G���A��g��`�����i�����l�͏㉺���E0�j
        room = new Rect(); //�G���A�̕�����`�����i�����l�͏㉺���E0�j
        road = null; //�G���A�̓���`�����i�����l��null�j
    }
    //���̋�`����郁�\�b�h
    public void SetRoad(int left, int top, int right, int bottom)
    {
        road = new Rect(left, top, right, bottom);
    }
}
