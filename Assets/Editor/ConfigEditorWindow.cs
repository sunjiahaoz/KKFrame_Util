using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using KK.Frame.Util;
using System.IO;

namespace KK.Frame.Util.Editor
{
    public class ConfigEditorWindow : EditorWindow
    {

        [MenuItem("Tools/ConfigEditor")]
        static void AddWindow()
        {
            ConfigEditorWindow window = (ConfigEditorWindow)EditorWindow.GetWindow<ConfigEditorWindow>("Config编辑");
            window.Show();
        }

        private TextAsset _config;
        private string _strKey;

        public void Awake()
        {
            _config = null;
            _strKey = "";
        }

        Vector2 scrollPos = Vector2.zero;
        void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));

            GUILayout.Label("配置文件", GUILayout.Width(200));

            _config = EditorGUILayout.ObjectField(_config, typeof(TextAsset), true) as TextAsset;

            if (GUILayout.Button("加载", GUILayout.Width(100)))
            {
                InitData();
            }

            if (_bDataIsLoad)
            {
                EditorGUILayout.BeginVertical();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Key", GUILayout.Width(50));
                _strKey = GUILayout.TextField(_strKey, GUILayout.Width(100));
                GUILayout.EndHorizontal();

                if (_strKey.Length > 0
                    && _dictData.ContainsKey(_strKey))
                {
                    GUILayout.BeginVertical();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("名称", GUILayout.MaxWidth(200));
                    GUILayout.Label("值", GUILayout.MaxWidth(200));
                    GUILayout.Label("类型", GUILayout.MaxWidth(200));
                    GUILayout.Label("变量", GUILayout.MaxWidth(200));
                    GUILayout.EndHorizontal();

                    for (int i = 0; i < _dictData[_strKey].Length; ++i)
                    {
                        GUILayout.BeginHorizontal();
                        // 中文名称   
                        string CHName = "字段名称";
                        if (rowParamCHName.Length > i)
                        {
                            //CHName = rowParamName[i];                        
                            CHName = rowParamCHName[i];
                        }
                        GUILayout.Label(CHName, GUILayout.MaxWidth(200));
                        // 值
                        _dictData[_strKey][i] = GUILayout.TextField(_dictData[_strKey][i], GUILayout.MaxWidth(200));
                        // 类型
                        string strType = "类型";
                        if (rowType.Length > i)
                        {
                            strType = rowType[i];
                        }
                        GUILayout.Label(strType, GUILayout.MaxWidth(200));
                        // 变量
                        GUILayout.Label(rowParamName[i], GUILayout.MaxWidth(200));

                        GUILayout.EndHorizontal();
                    }

                    if (GUILayout.Button("保存！", GUILayout.MaxWidth(100)))
                    {
                        SaveData();
                    }
                    if (Application.isPlaying)
                    {
                        if (GUILayout.Button("动态改变", GUILayout.MaxWidth(100)))
                        {
                            RuntimeChanged();
                        }
                    }

                    GUILayout.EndVertical();

                }
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();
        }


        /// <summary>
        /// 为了我们中国汉字！冲（重(写)）啊
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="onResourceLoaded"></param>
        /// <param name="isRequest"></param>
        /// <returns></returns>
        public List<CsvRow> LocalParseCSV(string pathName, CsvHelper.OnResourceLoaded onResourceLoaded = null, bool isRequest = false)
        {
            FileStream fileStream = File.OpenRead(pathName);
            // 主要是这里，配置文件用的是ANSI字符集，所以这里要用Default读取
            StreamReader streamReader = new StreamReader(fileStream, System.Text.Encoding.Default);
            string readStr = streamReader.ReadToEnd();
            streamReader.Close();
            fileStream.Close();

            List<CsvRow> rows = CsvHelper.ParseText(readStr);
            if (null != onResourceLoaded)
                onResourceLoaded(rows);
            return rows;
        }

        bool _bDataIsLoad = false;
        void InitData()
        {
            if (_config != null)
            {
                // 只对游戏配置有效果
                string strCheckPath = Application.dataPath + "/Resources/GameConfig/" + _config.name + ".csv";

                if (System.IO.File.Exists(strCheckPath))
                {
                    LocalParseCSV(strCheckPath, OnLoaded, true);
                }
                else
                {
                    this.ShowNotification(new GUIContent(strCheckPath + "不存在"));
                }
            }
        }

        string[] rowType = null;
        string[] rowParamName = null;
        string[] rowParamCHName = null;
        Dictionary<string, string[]> _dictData = new Dictionary<string, string[]>();
        void OnLoaded(List<CsvRow> rows)
        {
            _dictData.Clear();
            if (rows.Count < 3)
            {
                return;
            }
            // 第一行是字段类型
            rowType = rows[0].ToArray();
            // 第二行是英文名称
            rowParamName = rows[1].ToArray();
            // 第三行是中文名称
            rowParamCHName = rows[2].ToArray();

            for (int i = 3; i < rows.Count; ++i)
            {
                string[] values = rows[i].ToArray();
                if (values.Length > 0)
                {
                    // 0是主键
                    _dictData.Add(values[0], values);
                }
            }

            _bDataIsLoad = true;
        }

        void SaveData()
        {
            if (_config == null)
            {
                ShowNotification(new GUIContent("配置文件为空！！！"));
                return;
            }

            string strFilePath = Application.dataPath + "/Resources/GameConfig/" + _config.name + ".csv";

            FileStream fileStream = File.OpenWrite(strFilePath);
            CsvFileWriter write = new CsvFileWriter(fileStream, System.Text.Encoding.Default);
            // 先把前三行的内容搞定
            SaveData_Line(rowType, write);
            SaveData_Line(rowParamName, write);
            SaveData_Line(rowParamCHName, write);
            // 具体数据 
            foreach (var item in _dictData)
            {
                SaveData_Line(item.Value, write);
            }

            write.Close();
            Debug.Log("保存完成！！！！！！！！！！！");
        }

        void SaveData_Line(string[] values, CsvFileWriter writer)
        {
            if (values.Length == 0)
            {
                return;
            }
            CsvRow row = new CsvRow();
            for (int i = 0; i < values.Length; ++i)
            {
                row.Add(values[i]);
            }
            writer.WriteRow(row);
        }

        void RuntimeChanged()
        {
            if (_config == null
                || _strKey.Length == 0)
            {
                ShowNotification(new GUIContent("Please认真一点，配置文件或键值没写！"));
                return;
            }
            // 找到对应的配置类
            Assembly ass = Assembly.Load("Assembly-CSharp");
            Type e = ass.GetType(_config.name);

            // 获取dict
            FieldInfo fi = e.GetField("dic");
            if (fi == null)
            {
                ShowNotification(new GUIContent("没找到类" + _config.name + "中的dic！"));
                return;
            }
            IDictionary dict = (IDictionary)fi.GetValue(null);
            // 如果没有数据就加载
            if (dict.Count == 0)
            {
                MethodInfo mi = e.GetMethod("load");
                if (mi == null)
                {
                    return;
                }
                Debug.Log(_config.name + " Loaded in Editor !!!");
                mi.Invoke(null, null);
            }
            // 将key转成int类型的，因为基本上配置文件的主键都是int类型
            int nKey = 0;
            try
            {
                int.TryParse(_strKey, out nKey);
                if (!dict.Contains(nKey))
                {
                    return;
                }

                for (int i = 0; i < rowParamName.Length; ++i)
                {
                    FieldInfo fi0 = dict[nKey].GetType().GetField(rowParamName[i]);
                    System.Object obj = fi0.GetValue(dict[nKey]);
                    if (obj.GetType() == typeof(int))
                    {
                        int nValue0 = 0;
                        int.TryParse(_dictData[_strKey][i], out nValue0);
                        fi0.SetValue(obj, nValue0);
                    }
                    else if (obj.GetType() == typeof(float))
                    {
                        float fValue = 0;
                        float.TryParse(_dictData[_strKey][i], out fValue);
                        fi0.SetValue(obj, fValue);
                    }
                    else if (obj.GetType() == typeof(string))
                    {
                        fi0.SetValue(obj, _dictData[_strKey][i]);
                    }
                    else if (obj.GetType() == typeof(int[]))
                    {
                        fi0.SetValue(obj, CsvHelper.ToIntArray(_dictData[_strKey][i]));
                    }
                    else if (obj.GetType() == typeof(float[]))
                    {
                        fi0.SetValue(obj, CsvHelper.ToFloatArray(_dictData[_strKey][i]));
                    }
                    else if (obj.GetType() == typeof(string[]))
                    {
                        fi0.SetValue(obj, CsvHelper.ToStringArray(_dictData[_strKey][i]));
                    }
                    else
                    {
                        ShowNotification(new GUIContent(obj.GetType() + " 是个新类型，通知程序员添加！！！"));
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex);
                return;
            }
        }


    }

}
