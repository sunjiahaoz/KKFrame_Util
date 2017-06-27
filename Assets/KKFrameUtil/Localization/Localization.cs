using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum LocaliztionLan
{
    CN = 0,
    EN,
}

public class Localization : MonoBehaviour {
    public string _strKey;
    public LocaliztionLan _eLan = LocaliztionLan.CN;

    Text _text;
    Text text
    {
        get
        {
            if (_text == null)
            {
                _text = GetComponent<Text>();
                if (_text == null)
                {
                    Debug.LogError("<color=red>[Error]</color>---" + "没有Text控件", gameObject);
                }
            }
            return _text;
        }
    }

    void Awake()
    {
        Excute();
    }
    
    [ContextMenu("立即执行")]
    void Excute()
    {
        if (text != null)
        {
            text.text = GetValue(_strKey, _eLan);
        }
    }

    public static string GetValue(string strKey, LocaliztionLan eType = LocaliztionLan.CN, bool bReplaceNewLine = true)
    {
        string strValue = string.Empty;        

        if (!LocalizationConfig.dic.ContainsKey(strKey))
        {
            Debug.LogWarning("<color=orange>[Warning]</color>---" + "找不到Key:" + strKey);
            return strKey;
        }
        LocalizationConfig config = LocalizationConfig.dic[strKey];
        switch (eType)
        {
            case LocaliztionLan.CN:
                strValue = config.CN;
                break;
            case LocaliztionLan.EN:
                strValue = config.EN;
                break;
            default:
                {
                    Debug.LogWarning("<color=orange>[Warning]</color>---" + "找不到该Key指定语言类型："+eType);
                    strValue = strKey;
                    break;
                }                
        }
        if (bReplaceNewLine)
        {
            strValue = strValue.Replace("\\n", "\n");
        }
        return strValue;
    }
}
