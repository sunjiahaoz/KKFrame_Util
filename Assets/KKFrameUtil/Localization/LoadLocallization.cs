using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KK.Frame.Util;

public class LoadLocallization : MonoBehaviour {
    public bool isClearWhenDestroy;        //切换出游戏的时候是否清空当前游戏所用的Locallization配置
    public TextAsset locallizationliConfig;     //配置文件
    List<CsvRow> rows;     //从配置文件读取出来的数据
	void Awake () {
        if (locallizationliConfig != null) 
        {
            rows = CsvHelper.ParseCSV(locallizationliConfig.name, LocalizationConfig.OnLoaded, true);        //加载当前游戏所需的配置文件
        }       
	}
	// Update is called once per frame
	void OnDestroy () {
        if (isClearWhenDestroy) 
        {
            for (int i = 3; i < rows.Count; i++)
            {
                string[] values = rows[i].ToArray();
                if (values.Length <= 0) continue;
                LocalizationConfig elem = new LocalizationConfig();
                if (values.Length > 0)
                {
                    LocalizationConfig.dic.Remove(values[0]);           //删除当前游戏的配置文件配置
                }
            }
        }
	}
}
