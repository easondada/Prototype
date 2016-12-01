using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SquareModel
{
    public int width = 4;
    public int height = 4;
    public List<Cube> list { get; set; }
    private Cube[][] cubeArray;//用来检测胜负
    public event Action OnLost;

    public SquareModel()
    {
        list = new List<Cube>();
        cubeArray = new Cube[height][];
        for (int i = 0; i < width; i++)
        {
            cubeArray[i] = new Cube[width];
        }
        int length = width * height;
        for (int i = 0; i < length; i++)
        {
            list.Add(new Cube
            {
                number = 0,
                isEnabled = false,
            });
        }
        for (int i = 0; i < height; i++)
        {//行
            for (int j = 0; j < width; j++)
            {//列
                cubeArray[i][j] = list[j + i * 4];
            }
        }
        //随机生成两个方块
        createNewCube();
        createNewCube();
    }
    public void ResetCubeList()
    {//重置方块
        foreach (var c in list)
        {
            c.number = 0;
            c.isEnabled = false;
        }
        createNewCube();
        createNewCube();
    }
    public bool CheckLost()
    {//true判负
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if ((i % 2 == 0 && j % 2 == 0)|| (i % 2 != 0 && j % 2 != 0))
                {//偶数行
                    if (cubeArray[i][j].number != 0)
                    {
                        if (i - 1 >= 0 && i - 1 < height)
                        {//相邻上一个元素存在
                            if (cubeArray[i - 1][j].number == cubeArray[i][j].number)
                                return false;
                        }
                        if (i + 1 >= 0 && i + 1 < height)
                        {//相邻下一个元素存在
                            if (cubeArray[i + 1][j].number == cubeArray[i][j].number)
                                return false;
                        }
                        if (j - 1 >= 0 && j - 1 < width)
                        {//相邻前一个元素存在
                            if (cubeArray[i][j-1].number == cubeArray[i][j].number)
                                return false;
                        }
                        if (j + 1 >= 0 && j + 1 < width)
                        {//相邻后一个元素存在
                            if (cubeArray[i][j + 1].number == cubeArray[i][j].number)
                                return false;
                        }
                    }
                }
            }
        }
        return true;
    }
    public int createNewCube()
    {//初始化要显示的cube(随机两个方块) 
        int newCubeIndex = -1;//创建失败时返回-1
        var emptyCubeArray = list.FindAll(c => c.isEnabled == false).ToArray();
        if (emptyCubeArray.Length != 0)
        {//
            long tick = DateTime.Now.Ticks;
            System.Random rand = new System.Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
            int index = rand.Next(emptyCubeArray.Length);
            emptyCubeArray[index].number = (rand.Next(0, 100) % 2 == 0) ? 2 : 4;
            emptyCubeArray[index].isEnabled = true;
            newCubeIndex = list.IndexOf(emptyCubeArray[index]);
        }
        if (emptyCubeArray.Length == 1)
        {
            if (CheckLost() && OnLost!=null)
            {
                OnLost();
            }
        }
        return newCubeIndex;//返回新创建的cube的在list中的index
    }
    public Cube getCube(int index)
    {
        if (index < 0 || index >= list.Count)
        {
            return null;
        }
        return list[index];
    }
    public Cube getCube(int x, int y)
    {//通过行列获取指定cube
        if (x < 0 || x >= width)
        {
            return null;
        }
        if (y < 0 || y >= height)
        {
            return null;
        }
        return list[x + y * 4];
    }
}
