using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using GDGeek;
using Assets.Scripts;
using System.Collections.Generic;

public class PlayView : MonoBehaviour {

    public SwipeRecognizer swipe;
    //与界面对应的方块类型
    private Square[] squareList = null;
    public Text HighestScore = null;//最高分
    public Text Score = null; //当前得分
    public event Action onPlayStart;
    public Square topSquare = null;
    public List<TweenPosition> tweenList;
    public GameObject WinDialog = null;
    public GameObject LostDialog = null;

    private int curScore;
    private int bestScore;
    public int BestScore
    {
        get
        {
            return bestScore;
        }
        set
        {
            int max = Math.Max(value, bestScore);
            if (bestScore != max)
            {
                bestScore = max;
                PlayerPrefs.SetInt("BESTSCORE", bestScore);
            }        
            HighestScore.text = bestScore.ToString();
        }
    }
    public int ScoreInt
    {
        get
        {
            return curScore;
        }
        set
        {
            curScore = value;
            BestScore = value;
            Score.text = value.ToString();
        }
    }
    void Awake()
    {
        squareList = GameObject.Find("PlayArea").GetComponentsInChildren<Square>();
        swipe = this.GetComponent<SwipeRecognizer>();
    }
    // Use this for initialization
    void Start()
    {
        foreach (Square square in squareList)
        {//初始化square，设置不可见
            square.number = 0;
            square.HideWithColor();
        }
        tweenList = new List<TweenPosition>();
        BestScore = PlayerPrefs.GetInt("BESTSCORE");
        if (onPlayStart != null)
        {
            onPlayStart();
        }
    }

    // Update is called once per frame
    void Update () {
	
	}
    public void doMoveSquare(Square start, Square end)
    {
        Square s = (Square)GameObject.Instantiate(topSquare);
        s.Hide();
        s.transform.SetParent(start.transform.parent);
        s.transform.localScale = start.transform.localScale;
        s.transform.localPosition = start.transform.localPosition;
        s.number = start.number;
        s.Show();
        s.ShowWithColor();
        start.HideWithColor();
        TweenPosition tp = TweenPosition.Begin(s.gameObject, 0.1f, end.transform.localPosition);
        EventDelegate evd = new EventDelegate(this, "OnTweenFinished");
        evd.parameters[0] = new EventDelegate.Parameter() { obj = s.gameObject };
        tp.AddOnFinished(evd);
        tweenList.Add(tp);
        //tp.PlayForward();
    }
    void OnTweenFinished(GameObject obj)
    {
        GameObject.DestroyImmediate(obj);
    }
    public void doAriseSquare(int index, int number)
    {//出现新方块的动画任务
        if (index < 0 || index >= squareList.Length)
        {
            return;
        }
        Square s = squareList[index];//新出现方块
        Vector3 original = s.transform.localScale;//保存原来的缩放比列
        s.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);//设置成开始的缩放比列
        s.number = number;
        s.ShowWithColor();
        TweenScale.Begin(s.gameObject, 0.3f, original);
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
