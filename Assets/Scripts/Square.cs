using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Square : MonoBehaviour {
    public Text NumText = null;
    public Image background = null;
    private int? _number;
    private Color originalColor = Color.HSVToRGB(0.0583f,0.12f,0.39f);
    private Color overColor = Color.HSVToRGB(0.475f,0.78f,0.9f);
    public int? number
    {
        get
        {
            return _number;
        }
        set
        {
            if (value == null)
                NumText.text = null;
            NumText.text = value.ToString();
            _number = value;
            #region 根据数字改变颜色
            switch (value)
            {
                case 16: NumText.color = Color.HSVToRGB(0.075f, 1f, 1f);
                    break;
                case 32:
                    NumText.color = Color.HSVToRGB(0.025f, 0.72f, 1f);
                    break;
                case 64:
                    NumText.color = Color.HSVToRGB(0.022f, 1f, 1f);
                    break;
                case 128:
                    NumText.color = Color.HSVToRGB(0.917f, 1f, 1f);
                    break;
                case 256:
                    NumText.color = Color.HSVToRGB(0.8f, 0.5f, 1f);
                    break;
                case 512:
                    NumText.color = Color.HSVToRGB(0.583f, 0.88f, 1f);
                    break;
                case 1024:
                    NumText.color = Color.HSVToRGB(0.2f, 1f, 1f);
                    break;
                case 2048:
                    NumText.color = Color.HSVToRGB(0.953f, 0.78f, 0.87f);
                    break;
                default:
                    if (value > 2048)
                    {
                        NumText.color = overColor;
                    }
                    else
                    {
                        NumText.color = originalColor;
                    }
                    break;
            }
            #endregion
        }
    }
    public void ShowWithColor()
    {  
        background.color = Color.HSVToRGB(0.1f,0.32f,1f);
    }
    public void HideWithColor()
    {
        NumText.text = string.Empty;
        background.color = Color.clear;
    }
}
