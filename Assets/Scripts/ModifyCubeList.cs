using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class ModifyCubeList
    {//可能需要通过执行传入的委托来达到移动时产生的效果
        //或者在Controller层根据列表的差异做效果(优先考虑)
        private int width = 4;
        private int height = 4;
        private List<Cube> _list;
        private List<Cube>[] rowList;//行
        private List<Cube>[] colList;//列
        private List<Cube>[] reRowList;//反向的行
        private List<Cube>[] reColList;//反向的列

        public event Action<Cube, Cube> OnSwapCube;//在controller层中记录变化的model
        public event Action<int> ScoreChanged;

        public ModifyCubeList(List<Cube> list)
        {
            _list = list;
            ModifyInit();
        }
        private void ModifyInit()
        {
            rowList = new List<Cube>[width];
            colList = new List<Cube>[height];
            reRowList = new List<Cube>[width];
            reColList = new List<Cube>[height];

            for (int i = 0; i < width; i++)
                rowList[i] = new List<Cube>();
            for (int i = 0; i < height; i++)
                colList[i] = new List<Cube>();
            for (int i = 0; i < width; i++)
                reRowList[i] = new List<Cube>();
            for (int i = 0; i < height; i++)
                reColList[i] = new List<Cube>();

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    colList[i].Add(_list[i + j * 4]);
                }
            }
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    rowList[j].Add(_list[i + j * 4]);
                }
            }
            for (int i = 0; i < width; i++)
            {
                for (int j = height-1; j >=0; j--)
                {
                    reColList[i].Add(_list[i + j * 4]);
                }
            }
            for (int j = 0; j < height; j++)
            {
                for (int i = width - 1; i >= 0; i--)
                {
                    reRowList[j].Add(_list[i + j * 4]);
                }
            }
        }
        private void SwapCube(Cube start,Cube end)
        {//交换cube
            int number = start.number;
            bool isEnabled = start.isEnabled;

            start.number = end.number;
            start.isEnabled = end.isEnabled;

            end.number = number;
            end.isEnabled = isEnabled;

            if (OnSwapCube!=null)
                OnSwapCube(start, end);
        }
        private bool RemoveBlank(List<Cube>[] targetList)
        {//相对于移动方向消除空白
            bool hasMoved = false;
            foreach (var target in targetList)
            {
                List<int> newIndexs = new List<int>();
                List<int> oldIndexs = new List<int>();
                var tempList = target.Where(i => i.isEnabled != false).ToList();
                tempList.ForEach(t =>
                {
                    newIndexs.Add(tempList.IndexOf(t));
                    oldIndexs.Add(target.IndexOf(t));
                });
                for (int i = 0; i < oldIndexs.Count; i++)
                {
                    if (oldIndexs[i] != newIndexs[i])
                    {//如果cube发生移动标记则hasMoved = true;
                        SwapCube(target[oldIndexs[i]], target[newIndexs[i]]);
                        hasMoved = true;
                    }
                }
            }
            return hasMoved;
        }
        private bool MergeCube(List<Cube>[] targetList)
        {
            bool hasMerged = false;
            foreach (var target in targetList)
            {
                int k = 0;
                while (k < target.Count)
                {
                    for (int i = k + 1; i < target.Count; i++)
                    {
                        if (target[i].isEnabled == true)
                        {//方块不为空
                            if (target[i].number == target[k].number)
                            {//相邻的方块相等
                                hasMerged = true;
                                target[k].number *= 2;
                                if (ScoreChanged != null)
                                    ScoreChanged(target[k].number);
                                target[i].number = 0;
                                target[i].isEnabled = false;
                                k = i;
                                if (OnSwapCube != null)
                                    OnSwapCube(target[i], target[k]);
                            }
                            break;
                        }
                    }
                    k++;
                }             
            }
            return hasMerged;
        }
        public bool MoveUp()
        {//向上移动
            if (_list == null) return false;
            bool isSucceeded = false;
            isSucceeded |= MergeCube(colList);
            isSucceeded |= RemoveBlank(colList);
            return isSucceeded;
        }
        public bool MoveDown()
        {//向下移动
            if (_list == null) return false;
            bool isSucceeded = false;
            isSucceeded |= MergeCube(reColList);
            isSucceeded |= RemoveBlank(reColList);
            return isSucceeded;
        }
        public bool MoveLeft()
        {//向左移动
            if (_list == null) return false;
            bool isSucceeded = false;
            isSucceeded |= MergeCube(rowList);
            isSucceeded |= RemoveBlank(rowList);
            return isSucceeded;
        }
        public bool MoveRight()
        {//向右移动
            if (_list == null) return false;
            bool isSucceeded = false;
            isSucceeded |= MergeCube(reRowList);
            isSucceeded |= RemoveBlank(reRowList);
            return isSucceeded;
        }
    }
}
