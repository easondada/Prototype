using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts;
using GDGeek;
using UnityEngine.UI;

public class Play : MonoBehaviour {

    //Play初始化时需要执行的委托列表，一般是由Controller添加委托
    public event Action onPlayStart;
    //SwipeRecognizer类型对象用来监听手势，处理逻辑放在Controller
    public SwipeRecognizer swipe;
    //与界面对应的方块类型
    private Square[] squareList = null;
    public Square topSquare = null;
    void Awake()
    {
        squareList = GameObject.Find("PlayArea").GetComponentsInChildren<Square>();
        //squareList = this.GetComponentsInChildren<Square>();
        swipe = this.GetComponent<SwipeRecognizer>();
    }
    // Use this for initialization
    void Start() {
        Debug.Log("Play Start()");
        foreach (Square square in squareList)
        {//初始化square，设置不可见
            square.number = 0;
            square.HideWithColor();
        }
        if (onPlayStart != null)
            onPlayStart();//在初始化后执行委托列表
    }
    public void AriseTask(int index,int number)
    {//出现新方块的动画任务
        if (index < 0 || index >= squareList.Length)
        {
            return;
        }
        Square s = squareList[index];//新出现方块
        Vector3 original = s.transform.localScale;//保存原来的缩放比列
        s.transform.localScale =new Vector3(0.1f, 0.1f, 0.1f);//设置成开始的缩放比列
        s.number = number;
        s.ShowWithColor();
        //TweenTask tt = new TweenTask(delegate ()
        //{//方块插值缩放
        //    return null;
        //    //return TweenScale.Begin(s.gameObject, 0.5f, original);
        //});
        //return tt;
    }
    public Task MoveSquareTask(Square start, Square end)
    {
        Square s = (Square)GameObject.Instantiate(topSquare);
        s.Hide();
        s.transform.SetParent(start.transform.parent);
        s.transform.localScale = start.transform.localScale;
        s.transform.localPosition = start.transform.localPosition;
        s.number = start.number;
        TweenTask tt = new TweenTask(delegate ()
        {//方块插值缩放
            TweenLocalPosition tp = TweenLocalPosition.Begin(s.gameObject, 0.2f, end.transform.localPosition);
            return tp;
        });
        TaskManager.PushFront(tt, delegate
        {
            s.Show();
            s.ShowWithColor();
            start.HideWithColor();
        });
        TaskManager.PushBack(tt, delegate
        {
            GameObject.DestroyObject(s.gameObject);
        });
        return tt;
    }
    // Update is called once per frame
    void Update () {

	}
    public Square getSquare(int index)
    {//根据行索引和列索引来找到square
        if (index < 0 || index >= squareList.Length)
        {
            return null;
        }
        return squareList[index];
    }
    public Square getSquare(int x, int y)
    {//根据行索引和列索引来找到square
        int n = x + y * 4;
        if (n < 0 || n >= squareList.Length)
        {
            return null;
        }
        else
        {
            return squareList[n];
        }
    }
}
