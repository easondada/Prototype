using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Model : MonoBehaviour {
    public int width = 4;
    public int height = 4;
    [HideInInspector]
    public List<Cube> list = null;
    public event Action OnStart;
    void Start()
    {//在Start中初始化公共变量
        int length = width * height;
        for (int i = 0; i < length; i++)
        {
            list.Add(new Cube
            {
                number = 0,
                isEnabled = false,
            });
        }
        //随机生成两个方块
        createNewCube();
        createNewCube();

        if (OnStart != null)
            OnStart();
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
        if (y < 0|| y >= height)
        {
            return null;
        }
        return list[x + y * 4];
    }
}
