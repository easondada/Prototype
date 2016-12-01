using UnityEngine;
using System.Collections;
using GDGeek;
using System;
using Assets.Scripts;

public class Controller : MonoBehaviour {

    private FSM _fsm = new FSM();//有限状态机
    public View _view = null;//View层
    public Model _model = null;//Model层
    public ModifyCubeList modifier;//处理方块变化的对象
    private TaskSet taskSet = new TaskSet();//记录移动方块任务的集合
    void Start () {
        Debug.Log("Controller Start()");
        modifier = new ModifyCubeList(_model.list);
        //_model.OnStart += modifier.ModifyInit;

        _fsm.addState("begin",beginState());
        _fsm.addState("play", playState());

        _fsm.addState("input", inputState(), "play");
        _fsm.addState("move", moveState(), "play");
        _fsm.addState("arise", ariseState(), "play");

        _fsm.addState("end", endState());
        _fsm.init("input");
    }
  
    public void fsmPost(string name)
    {//发字符串给状态机
        _fsm.post(name);
    }
    private void mappingModel2View()
    {//开始时根据初始model显示方块(此方法相当于刷新页面)
        for (int i = 0; i < _model.width; i++)
        {
            for (int j = 0; j < _model.height; j++)
            {
                Square s = _view.play.getSquare(i, j);
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
    private void CubeChanged(Cube start, Cube end)
    {
        Square s = _view.play.getSquare(_model.list.IndexOf(start));
        Square e = _view.play.getSquare(_model.list.IndexOf(end));
        taskSet.push(_view.play.MoveSquareTask(s, e));
    }
    private void SwipeEventHandler(SwipeGesture gesture)
    {//此处根据手势处理游戏逻辑
        FingerGestures.SwipeDirection direction =  gesture.Direction;
        switch (direction)
        {
            case FingerGestures.SwipeDirection.Up:
                if (modifier.MoveUp())
                {//检查是否发生有效移动，有效移动才发送swipe消息
                    _fsm.post("swipe");
                }                
                break;
            case FingerGestures.SwipeDirection.Down:
                if (modifier.MoveDown())
                {
                    _fsm.post("swipe");
                }
                break;
            case FingerGestures.SwipeDirection.Left:
                if (modifier.MoveLeft())
                {
                    _fsm.post("swipe");
                }
                break;
            case FingerGestures.SwipeDirection.Right:
                if (modifier.MoveRight())
                {
                    _fsm.post("swipe");
                }
                break;
            default:break;
        }
    }
    private State beginState()
    {//开始状态
        StateWithEventMap state = new StateWithEventMap();
        state.onStart += delegate
        {
            _view.begin.Show();
        };
        state.onOver += delegate
        {
            _view.begin.Hide();
        };
        state.addEvent("begin", "play");
        return state;
    }

    private State playState()
    {//正在玩的状态
        StateWithEventMap state = new StateWithEventMap();
        state.onStart += delegate
        {
            _view.play.Show();
            _view.play.onPlayStart += mappingModel2View;
            
        };
        state.onOver += delegate
        {
            _view.play.Hide();
            _view.play.onPlayStart -= mappingModel2View;
        };
        return state;
    }

    private State endState()
    {//结束状态
        StateWithEventMap state = new StateWithEventMap();
        state.onStart += delegate
        {
            _view.end.Show();
        };
        state.onOver += delegate
        {
            _view.end.Hide();
        };
        state.addAction("end", delegate (FSMEvent evt)
        {//重新清空CubeList
            _model.ResetCubeList();
            mappingModel2View();
            return "input";
        });
        return state;
    }

    private State moveState()
    {//move状态执行完移动任务跳向arise状态
        StateWithEventMap state = TaskState.Create(delegate
        {
            //TaskWait tw = new TaskWait();
            //tw.setAllTime(0.1f);
            //return tw;
            return taskSet;
        }, _fsm, "arise");
        //StateWithEventMap state = new StateWithEventMap();
        //state.onStart += delegate
        //{
        //    TaskManager.Run(taskSet);
        //    //taskSet.init();
        //};
        state.onOver += delegate
        {
            taskSet.Clear();
            //_fsm.post("toArise");
        };
        //state.addEvent("toArise", "arise");
        //StateWithEventMap state = new StateWithEventMap();
        //state.addEvent("swipe", "arise");
        return state;
    }

    private State ariseState()
    {//方块出现的状态
        int newIndex = -1;
        StateWithEventMap state = TaskState.Create(delegate
        {
            mappingModel2View();//出现随机方块前需要先刷新页面
            newIndex = _model.createNewCube();//model层先创建新的方块出来
            //Task tk = _view.play.AriseTask(newIndex,_model.getCube(newIndex).number);//在此任务中完成view层动画
            //return tk;
            return new Task();
        }, _fsm, delegate
        {
            if (newIndex>=0)
            {
                return "input";
            }
            else
            {//游戏结束
                _view.end.Peroration.text = "Game Over!";
                return "end";
            }
        });
        return state;
    }
    private State inputState()
    {//监听输入的状态
        StateWithEventMap state = new StateWithEventMap();
        state.onStart += delegate
        {//进入input状态时订阅监听手势事件
            _view.play.swipe.OnGesture += SwipeEventHandler;
            modifier.OnSwapCube += CubeChanged;
        };
        state.onOver += delegate
        {//离开input状态时取消订阅监听手势事件
            _view.play.swipe.OnGesture -= SwipeEventHandler;
            modifier.OnSwapCube -= CubeChanged;
        };
        //state.addEvent("swipe", "arise"); //监听swipe消息，收到消息跳向arise状态
        state.addEvent("swipe", "move"); //监听swipe消息，收到消息跳向move状态
        return state;
    }

    // Update is called once per frame
    void Update () {

    }
}
