using UnityEngine;
using System.Collections;
using Assets.Scripts;
using GDGeek;
using System;

public class Ctrl : MonoBehaviour {

    private FSM _fsm = new FSM();//有限状态机
    public PlayView playView = null;//view层(只有playview才需要逻辑控制)
    private ModifyCubeList modifier;//处理方块变化的对象
    public SquareModel _model { get; set; }//Model层
    public MenuManager manager = null;
    //private bool lostFlag = false;//用来判断是否输了
    // Use this for initialization
    void Start () {
        _model = new SquareModel();
        _model.OnLost += doLost;
        modifier = new ModifyCubeList(_model.list);
        modifier.ScoreChanged += CalculateScore;

        _fsm.addState("begin", beginState());
        _fsm.addState("play", playState());

        _fsm.addState("input", inputState(), "play");
        //_fsm.addState("move", moveState(), "play");
        _fsm.addState("arise", ariseState(), "play");

        _fsm.addState("end", endState());
        _fsm.init("begin");
    }

    private void doLost()
    {//输了的时候要干什么
        ShowObj(playView.LostDialog);
        //lostFlag = true;
        _fsm.post("toEnd");
    }
    private void doWin()
    {
        ShowObj(playView.WinDialog);
        _fsm.post("toEnd");
    }
    private void CalculateScore(int score)
    {
        playView.ScoreInt += score;
        if (score == 1024)
        {
            doWin();
        }
    }



    public void fsmPost(string name)
    {//发字符串给状态机
        _fsm.post(name);
    }
    private State endState()
    {
        StateWithEventMap state = new StateWithEventMap();
        //state.onStart += delegate
        //{
        //    Debug.Log("end state");
        //};
        //state.onOver += delegate
        //{
        //    Debug.Log("left end");
        //};
        state.addEvent("restart", "input");
        return state;
    }

    private State ariseState()
    {
        int newIndex = -1;
        StateWithEventMap state = TaskState.Create(delegate
        {
            if (playView.tweenList != null)
            {//mappingModel2View();//出现随机方块前需要先刷新页面
                playView.tweenList[playView.tweenList.Count - 1].AddOnFinished(mappingModel2View);
            }
            newIndex = _model.createNewCube();//model层先创建新的方块出来
            playView.doAriseSquare(newIndex, _model.getCube(newIndex).number);//在此任务中完成view层动画
            return new Task();
        }, _fsm, "input"); 
        return state;
    }

    //private State moveState()
    //{
    //    StateWithEventMap state = new StateWithEventMap();
    //    return state;
    //}

    private State inputState()
    {
        StateWithEventMap state = new StateWithEventMap();
        state.onStart += delegate
        {//进入input状态时订阅监听手势事件
            //Debug.Log("input state");
            playView.swipe.OnGesture += SwipeEventHandler;
            modifier.OnSwapCube += CubeChanged;
        };
        state.onOver += delegate
        {//离开input状态时取消订阅监听手势事件
            //Debug.Log("left input state");
            playView.swipe.OnGesture -= SwipeEventHandler;
            modifier.OnSwapCube -= CubeChanged;
        };
        state.addEvent("toEnd", "end");
        state.addEvent("swipe", "arise"); //监听swipe消息，收到消息跳向arise状态
        //state.addEvent("swipe", "move"); //监听swipe消息，收到消息跳向move状态
        return state;
    }

    private void CubeChanged(Cube start, Cube end)
    {
        Square s = playView.getSquare(_model.list.IndexOf(start));
        Square e = playView.getSquare(_model.list.IndexOf(end));
        playView.doMoveSquare(s, e);
    }

    private State playState()
    {
        StateWithEventMap state = new StateWithEventMap();
        state.onStart += delegate
        {
            //Debug.Log("play state");
            playView.onPlayStart += mappingModel2View;

        };
        state.onOver += delegate
        {
            playView.onPlayStart -= mappingModel2View;
        };
        return state;
    }

    private State beginState()
    {
        StateWithEventMap state = new StateWithEventMap();
        //state.onStart += delegate
        //{
        //    Debug.Log("begin state");
        //};
        state.addEvent("begin", "input");
        return state;
    }

    // Update is called once per frame
    void Update () {

    }
    private void SwipeEventHandler(SwipeGesture gesture)
    {//此处根据手势处理游戏逻辑
        bool moveFlag = false;
        FingerGestures.SwipeDirection direction = gesture.Direction;
        switch (direction)
        {
            case FingerGestures.SwipeDirection.Up:
                if (modifier.MoveUp())
                {//检查是否发生有效移动，有效移动才发送swipe消息
                    moveFlag = true;
                }
                break;
            case FingerGestures.SwipeDirection.Down:
                if (modifier.MoveDown())
                {
                    moveFlag = true;
                }
                break;
            case FingerGestures.SwipeDirection.Left:
                if (modifier.MoveLeft())
                {
                    moveFlag = true;
                }
                break;
            case FingerGestures.SwipeDirection.Right:
                if (modifier.MoveRight())
                {
                    moveFlag = true;
                }
                break;
            default: break;
        }
        if (moveFlag)
        {//发生移动
            _fsm.post("swipe");
        }
    }
    public void mappingModel2View()
    {//开始时根据初始model显示方块(此方法相当于刷新页面)
        for (int i = 0; i < _model.width; i++)
        {
            for (int j = 0; j < _model.height; j++)
            {
                Square s = playView.getSquare(i, j);
                Cube c = _model.getCube(i, j);
                if (c.isEnabled)
                {
                    s.number = c.number;
                    s.ShowWithColor();
                }
                else
                {
                    s.HideWithColor();
                }
            }
        }
    }
    public void Restart()
    {//重新开始游戏
        _model.ResetCubeList();
        mappingModel2View();
        playView.ScoreInt = 0;
        //lostFlag = false;
    }
    public void ShowObj(GameObject obj)
    {
        obj.gameObject.SetActive(true);
    }
    public void HideObj(GameObject obj)
    {
        obj.gameObject.SetActive(false);
    }
}
