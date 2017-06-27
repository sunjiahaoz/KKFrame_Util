using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KK.Frame.Util.Editor
{
    /// <summary>
    /// 编辑器下常用工具方法
    /// </summary>
    public class ToolsEditor
    {
        /// <summary>
        /// 添加宏
        /// 多个宏请用分号隔开
        /// </summary>
        /// <param name="strSym"></param>
        /// <param name="target">目标平台</param>
        public static void AddDefineSymble(string strSym, UnityEditor.BuildTargetGroup target)
        {
            UnityEditor.BuildTargetGroup btg = target;
            string strDef = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(btg);

            if (strDef.IndexOf(strSym) > 0)
            {
                return;
            }

            if (strDef.Length > 0)
            {
                strDef += ";" + strSym;
            }
            else
            {
                strDef = strSym;
            }
            UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, strDef);
        }

        /// <summary>
        /// 移除宏
        /// 要移除多个宏请用分号分隔
        /// </summary>
        /// <param name="strSym"></param>
        /// <param name="target">目标平台</param>
        public static void RemoveDefineSymble(string strSym, UnityEditor.BuildTargetGroup target)
        {
            UnityEditor.BuildTargetGroup btg = target;
            string strDef = UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(btg);
            string[] strSyms = strSym.Split(';');
            for (int i = 0; i < strSyms.Length; ++i)
            {
                if (strSyms[i].Length == 0)
                {
                    continue;
                }

                strDef = strDef.Replace(strSyms[i] + ";", "");
                strDef = strDef.Replace(strSyms[i], "");
            }

            // 移除最后的分号
            if (strDef[strDef.Length - 1] == ';')
            {
                strDef = strDef.Remove(strDef.Length - 1, 1);
            }

            UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, strDef);
        }

        /// <summary>
        /// 打开指定目录,只能在Windows编辑器下使用
        /// </summary>
        /// <param name="strFolderPath">文件夹绝对路径</param>
        public static bool OpenFolder(string strFolderPath)
        {
            if (System.IO.Directory.Exists(strFolderPath))
            {                
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = "/cstart " + strFolderPath;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
