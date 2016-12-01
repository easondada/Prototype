using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public static class SetActiveExtend
    {//提供一个扩展方法以方便调用显示或隐藏方法
        public static void Show(this MonoBehaviour obj)
        {
            obj.gameObject.SetActive(true);
        }
        public static void Hide(this MonoBehaviour obj)
        {
            obj.gameObject.SetActive(false);
        }
    }
}
