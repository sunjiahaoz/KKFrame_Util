using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Reflection;

namespace KK.Frame.Util
{
    /// <summary>
    /// 简单的文件或文件夹的处理
    /// </summary>
    public class SimpleFileProcess
    {
        #region _ReadOrWriteFile_
        /// <summary>
        /// 读取textAsset资源，将每一行返回到ArrayList中
        /// </summary>
        /// <param name="textAsset">要读取的textAsset</param>
        /// <returns>包含每一行数据的列表</returns>
        public static ArrayList LoadFile(TextAsset textAsset)
        {
            ArrayList list = new ArrayList();
            if (textAsset == null)
            {
                return list;
            }


            string[] split = textAsset.text.Split(new char[] { '\n' });
            for (int i = 0; i < split.Length; ++i)
            {
                list.Add(split[i]);
            }
            return list;
        }

        /// <summary>
        /// 读取指定位置的文件，返回每一行的列表
        /// </summary>
        /// <param name="path">读取文件的路径   </param>
        /// <param name="name">读取文件的名称</param>
        /// <returns>每行内容</returns>
        public static ArrayList LoadFile(string path, string name)
        {
            //使用流的形式读取   
            StreamReader sr = null;
            try
            {
                sr = File.OpenText(path + "//" + name);
            }
            catch (Exception e)
            {
                //路径与名称未找到文件则直接返回空                   
                return null;
            }
            string line;
            ArrayList arrlist = new ArrayList();
            while ((line = sr.ReadLine()) != null)
            {
                //一行一行的读取   
                //将每一行的内容存入数组链表容器中   
                arrlist.Add(line);
            }
            //关闭流   
            sr.Close();
            //销毁流   
            sr.Dispose();
            //将数组链表容器返回   
            return arrlist;
        }

        /// <summary>
        /// 读取指定文件，返回文件内容
        /// </summary>
        /// <param name="path">读取文件的路径</param>
        /// <param name="name">读取文件的名称</param>
        /// <returns>文件内容</returns>
        public static string LoadFileContent(string path, string name)
        {
            //使用流的形式读取   
            StreamReader sr = null;
            try
            {
                sr = File.OpenText(path + "//" + name);
            }
            catch (Exception e)
            {
                //路径与名称未找到文件则直接返回空                   
                return string.Empty;
            }
            string strContent = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();
            return strContent;
        }
        /// <summary>
        /// 读取指定文件，返回文件内容
        /// </summary>
        /// <param name="strPath">文件路径</param>
        /// <returns>文件内容</returns>
        public static string LoadFileContent(string strPath)
        {
            //使用流的形式读取   
            StreamReader sr = null;
            try
            {
                sr = File.OpenText(strPath);
            }
            catch (Exception e)
            {
                //路径与名称未找到文件则直接返回空                   
                return string.Empty;
            }
            string strContent = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();
            return strContent;
        }
        #endregion

        #region _File_Folder_Process_
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="strPath">文件夹路径</param>
        public static void CreateDir(string strPath)
        {
            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
            }
        }

       /// <summary>
       /// 创建文件
        /// </summary>
        /// <param name="path">文件创建目录</param>
        /// <param name="name">文件的名称</param>
        /// <param name="info">写入的内容</param>
        /// <param name="bOverwrite">如果文件已经存在是否覆盖，否则原有数据后面写</param>
        public static void CreateFile(string path, string name, string info, bool bOverwrite = false)
        {
            // 是否先删除已经存在的
            if (bOverwrite)
            {
                DeleteFile(path, name);
            }

            //文件流信息   
            StreamWriter sw;
            FileInfo t = new FileInfo(path + "//" + name);
            if (!t.Exists)
            {
                //如果此文件不存在则创建   
                sw = t.CreateText();
            }
            else
            {
                //如果此文件存在则打开   
                sw = t.AppendText();
            }
            //以行的形式写入信息   
            sw.WriteLine(info);
            //关闭流   
            sw.Close();
            //销毁流   
            sw.Dispose();
        }

       /// <summary>
       /// 删除指定文件
        /// </summary>
        /// <param name="path">删除文件的路径</param>
        /// <param name="name">删除文件的名称</param>
        public static void DeleteFile(string path, string name)
        {
            DeleteFile(path + "/" + name);
        }

        /// <summary>
        /// 删除指定文件
        /// </summary>
        /// <param name="strFilePath">文件路径</param>
        public static void DeleteFile(string strFilePath)
        {
            File.Delete(strFilePath);
            DeleteMeta(strFilePath);
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="strFile">文件路径</param>
        /// <returns>存在则为true</returns>
        public static bool FileExists(string strFile)
        {
            return System.IO.File.Exists(strFile);
        }

        /// <summary>
        /// 复制文件到指定目录
        /// </summary>
        /// <param name="strFilePath">要复制的文件，是个文件</param>
        /// <param name="strFolderTo">要复制到的目录，是个目录</param>
        /// <param name="bIsRewrite">如果目的目录有同名文件是否覆盖</param>
        /// <returns></returns>
        public static bool CopyFileTo(string strFilePath, string strFolderTo, bool bIsRewrite = true)
        {
            string strFileName = Path.GetFileName(strFilePath);
            strFolderTo += "/" + strFileName;
            return CopyFile(strFilePath, strFolderTo, bIsRewrite);
        }
        /// <summary>
        /// 复制文件到指定文件名称
        /// </summary>
        /// <param name="strFilePath">源文件</param>
        /// <param name="strFileNewPath">新的文件</param>
        /// <param name="bIsRewrite">如果新的文件名称已经存在是否要覆盖</param>
        /// <returns></returns>
        public static bool CopyFile(string strFilePath, string strFileNewPath, bool bIsRewrite = true)
        {
            if (!System.IO.File.Exists(strFilePath))
            {
                return false;
            }
            try
            {
                System.IO.File.Copy(strFilePath, strFileNewPath, bIsRewrite);
                return true;
            }
            catch (System.Exception ex)
            {                
                return false;
            }
        }

        /// <summary>
        /// 移动文件到指定目录
        /// </summary>
        /// <param name="strFilePath">要移动的文件，是个文件</param>
        /// <param name="strFolderTo">要移动到的目录，是个目录</param>
        /// <param name="bIsRewrite">如果目的目录有同名文件是否覆盖</param>
        /// <returns></returns>
        public static bool MoveFileTo(string strFilePath, string strFolderTo, bool bIsRewrite = true)
        {
            if (!System.IO.File.Exists(strFilePath))
            {
                return false;
            }
            string strFileName = Path.GetFileName(strFilePath);
            string strFileTo = strFolderTo + "/" + strFileName;
            // 如果文件已经存在，就先删除
            if (System.IO.File.Exists(strFileTo)
                && bIsRewrite)
            {
                DeleteFile(strFileTo);
            }
            try
            {
                System.IO.File.Move(strFilePath, strFileTo);
                // .meta文件
                DeleteMeta(strFilePath);
                return true;
            }
            catch (System.Exception ex)
            {
                
                return false;
            }
        }

        /// <summary>
        /// 指定文件夹是否存在
        /// </summary>
        /// <param name="strFolderPath">文件夹路径</param>
        /// <returns>存在则为true</returns>
        public static bool FolderExists(string strFolderPath)
        {
            try
            {
                return System.IO.Directory.Exists(strFolderPath);
            }
            catch (System.Exception ex)
            {                
                return false;
            }
        }
        
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="strFolderPath">文件夹路径</param>
        /// <returns>成功返回true</returns>
        public static bool CreateFolder(string strFolderPath)
        {
            if (System.IO.Directory.Exists(strFolderPath))
            {
                return true;
            }
            System.IO.Directory.CreateDirectory(strFolderPath);
            return true;
        }
        /// <summary>
        /// 删除指定的目录
        /// </summary>
        /// <param name="strFolderPath">文件夹名称</param>
        /// <returns>成功返回true</returns>
        public static bool DeleteFolder(string strFolderPath)
        {
            try
            {
                if (System.IO.Directory.Exists(strFolderPath))
                {
                    System.IO.Directory.Delete(strFolderPath, true);
                    DeleteMeta(strFolderPath);
                }
                else
                {                    
                    return false;
                }
                return true;
            }
            catch (System.Exception ex)
            {                
                return false;
            }
        }

        /// <summary>
        /// 复制文件夹到指定目录路径
        /// </summary>
        /// <param name="strSrc"></param>
        /// <param name="strDst"></param>
        /// <returns></returns>
        public static bool CopyFolder(string strSrc, string strDst)
        {
            return CopyDirectory(strSrc, strDst);
        }
        /// <summary>
        /// 复制目录到指定目录下面
        /// </summary>
        /// <param name="strFolderSrc">要复制的目录路径</param>
        /// <param name="strFolderTo">要复制到的目录路径，源目录会被复制到这个目录的下面</param>
        /// <param name="bIsRewrite">如果目的目录中已经存在同名目录，则会先删除</param>
        /// <returns></returns>
        public static bool CopyFolderTo(string strFolderSrc, string strFolderTo, bool bIsRewrite = false)
        {
            try
            {
                string strTargetPath = strFolderTo + "/" + Path.GetFileName(strFolderSrc);
                if (System.IO.Directory.Exists(strTargetPath)
                    && bIsRewrite)
                {
                    DeleteFolder(strTargetPath);
                }
                return CopyDirectory(strFolderSrc, strTargetPath);
            }
            catch (System.Exception ex)
            {                
                return false;
            }

        }

        /// <summary>
        /// 移动目录到指定目录下面
        /// </summary>
        /// <param name="strFolderSrc">要移动的文件夹</param>
        /// <param name="strFolderTo">目的地文件夹。源文件夹会在这个目录下面</param>
        /// <param name="bIsRewrite">如果目的文件夹已经有同名的文件夹存在，则会先删除</param>
        /// <returns></returns>
        public static bool MoveFolderTo(string strFolderSrc, string strFolderTo, bool bIsRewrite = false)
        {
            string strTargetPath = strFolderTo + "/" + Path.GetFileName(strFolderSrc);
            if (System.IO.Directory.Exists(strTargetPath)
                && bIsRewrite)
            {
                DeleteFolder(strTargetPath);
            }
            try
            {
                System.IO.Directory.Move(strFolderSrc, strTargetPath);
                // .meta文件
                DeleteMeta(strFolderSrc);
                return true;
            }
            catch (System.Exception ex)
            {                
                return false;
            }
        }

        /// <summary>
        /// 遍历指定路径文件夹下的所有文件
        /// </summary>
        /// <param name="strFolderRoot">要遍历的文件夹路径</param>
        /// <param name="actionPath">遍历操作</param>
        /// <param name="strSeachPattern">搜索过滤，正则表达式</param>
        /// <param name="bIterator">是否迭代搜索</param>
        public static void TravelFilesInFolder(string strFolderRoot, System.Action<string> actionPath, string strSeachPattern = "*", bool bIterator = true)
        {
            if (!Directory.Exists(strFolderRoot))
            {
                return;
            }
            string[] strPaths = Directory.GetFiles(strFolderRoot, strSeachPattern, bIterator ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            for (int i = 0; i < strPaths.Length; ++i)
            {
                actionPath(strPaths[i]);
            }
        }
        /// <summary>
        /// 遍历指定路径文件夹下的文件夹
        /// </summary>
        /// <param name="strFolderRoot">要遍历的文件夹路径</param>
        /// <param name="actionPath">遍历操作</param>
        /// <param name="strSeachPattern">搜索过滤，正则表达式</param>
        /// <param name="bIterator">是否迭代搜索</param>
        public static void TravelFoldersInFolder(string strFolderRoot, System.Action<string> actionPath, string strSeachPattern = "*", bool bIterator = true)
        {
            if (!Directory.Exists(strFolderRoot))
            {
                return;
            }
            string[] strPaths = Directory.GetDirectories(strFolderRoot, strSeachPattern, bIterator ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            for (int i = 0; i < strPaths.Length; ++i)
            {
                actionPath(strPaths[i]);
            }
        }

        #endregion

        #region _Inner_
        protected static void DeleteMeta(string strPath)
        {
            string strMeta = strPath + "/" + ".meta";
            if (System.IO.File.Exists(strMeta))
            {
                System.IO.File.Delete(strMeta);
            }
        }
        protected internal static string ApplicationPath()
        {
            string docsPath;
            switch (Application.platform)
            {
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    docsPath = Application.dataPath.TrimEnd('/') + "/";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    docsPath = Application.dataPath.TrimEnd('/') + "/";
                    docsPath = docsPath.Replace("/Data/", "/Documents/");
                    break;

                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsWebPlayer:
                    docsPath = Application.dataPath.TrimEnd('\\') + @"\";
                    break;

                default:
                    docsPath = Application.dataPath.TrimEnd('/');
                    break;
            }
            return docsPath;
        }

        protected internal static string DataPath()
        {
            string docsPath;
            switch (Application.platform)
            {
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    docsPath = Application.persistentDataPath.TrimEnd('/') + "/";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    docsPath = Application.persistentDataPath.TrimEnd('/') + "/";
                    docsPath = docsPath.Replace("/Data/", "/Documents/");
                    break;

                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsWebPlayer:
                    docsPath = Application.persistentDataPath.TrimEnd('\\') + @"\";
                    break;

                default:
                    docsPath = Application.persistentDataPath.TrimEnd('/');
                    break;
            }
            return docsPath;
        }

        protected static bool CopyDirectory(string srcDir, string tgtDir)
        {
            DirectoryInfo source = new DirectoryInfo(srcDir);
            DirectoryInfo target = new DirectoryInfo(tgtDir);

            if (target.FullName.StartsWith(source.FullName, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new Exception("父目录不能拷贝到子目录！");
            }

            if (!source.Exists)
            {
                return false;
            }

            if (!target.Exists)
            {
                target.Create();
            }

            FileInfo[] files = source.GetFiles();

            for (int i = 0; i < files.Length; i++)
            {
                System.IO.File.Copy(files[i].FullName, target.FullName + "/" + files[i].Name, true);
            }

            DirectoryInfo[] dirs = source.GetDirectories();

            for (int j = 0; j < dirs.Length; j++)
            {
                CopyDirectory(dirs[j].FullName, target.FullName + "/" + dirs[j].Name);
            }
            return true;
        }

        /// <summary>
        /// 获得一个目录的大小
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static long GetDirectoryLength(string dirPath)
        {
            //判断给定的路径是否存在,如果不存在则退出
            if (!Directory.Exists(dirPath))
                return 0;
            long len = 0;


            TravelFilesInFolder(dirPath, (strFile) =>
            {
                FileInfo fi = new FileInfo(strFile);
                len += fi.Length;
            });
            return len;

            ////定义一个DirectoryInfo对象
            //DirectoryInfo di = new DirectoryInfo(dirPath);

            ////通过GetFiles方法,获取di目录中的所有文件的大小
            //foreach (FileInfo fi in di.GetFiles())
            //{
            //    len += fi.Length;
            //}

            ////获取di中所有的文件夹,并存到一个新的对象数组中,以进行递归
            //DirectoryInfo[] dis = di.GetDirectories();
            //if (dis.Length > 0)
            //{
            //    for (int i = 0; i < dis.Length; i++)
            //    {
            //        len += GetDirectoryLength(dis[i].FullName);
            //    }
            //}
            //return len;
        }

        /// <summary>
        /// 获得某个文件的大小
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static long FileSize(string filePath)
        {
            long temp = 0;

            //判断当前路径所指向的是否为文件
            if (File.Exists(filePath) == false)
            {
                string[] str1 = Directory.GetFileSystemEntries(filePath);
                foreach (string s1 in str1)
                {
                    temp += FileSize(s1);
                }
            }
            else
            {
                //定义一个FileInfo对象,使之与filePath所指向的文件向关联,
                //以获取其大小
                FileInfo fileInfo = new FileInfo(filePath);
                return fileInfo.Length;
            }
            return temp;
        }
        #endregion
    }
    
    /* 路径相关操作
     * 1、判定一个给定的路径是否有效,合法
      通过Path.GetInvalidPathChars或Path.GetInvalidFileNameChars方法获得非法的路径/文件名字符，可以根据它来判断路径中是否包含非法字符；

    2、如何确定一个路径字符串是表示目录还是文件
       使用Directory.Exists或File.Exist方法，如果前者为真，则路径表示目录；如果后者为真，则路径表示文件
    上面的方法有个缺点就是不能处理那些不存在的文件或目录。这时可以考虑使用Path.GetFileName方法获得其包含的文件名，如果一个路径不为空，而文件名为空那么它表示目录，否则表示文件；
    3、获得路径的某个特定部分
       Path.GetDirectoryName ：返回指定路径字符串的目录信息。
       Path.GetExtension ：返回指定的路径字符串的扩展名。
       Path.GetFileName ：返回指定路径字符串的文件名和扩展名。
       Path.GetFileNameWithoutExtension ：返回不具有扩展名的路径字符串的文件名。
       Path.GetPathRoot ：获取指定路径的根目录信息。
    4、准确地合并两个路径而不用去担心那个烦人的“\”字符
       使用Path.Combine方法，它会帮你处理烦人的“\”。
    5、获得系统目录的路径
       Environment.SystemDirectory属性：获取系统目录的完全限定路径
       Environment.GetFolderPath方法：该方法接受的参数类型为Environment.SpecialFolder枚举，通过这个方法可以获得大量系统    文件夹的路径，如我的电脑，桌面，系统目录等
       Path.GetTempPath方法：返回当前系统的临时文件夹的路径
    6、判断一个路径是绝对路径还是相对路径
       使用Path.IsPathRooted方法
    7、读取或设置当前目录
       使用Directory类的GetCurrentDirectory和SetCurrentDirectory方法
    8、使用相对路径
       设置当前目录后（见上个问题），就可以使用相对路径了。对于一个相对路径，我们可以使用Path.GetFullPath方法获得它的完    全限定路径（绝对路径）。
        注意：如果打算使用相对路径，建议你将工作目录设置为各个交互文件的共同起点，否则可能会引入一些不易发现的安全隐患，被恶意用户利用来访问系统文件。

    9、文件夹浏览对话框（FolderBrowserDialog类）
      主要属性： Description：树视图控件上显示的说明文本，如上图中的“选择目录--练习”；RootFolder：获取或设置从其开始浏览的根文件夹，如上 图中设置的我的电脑（默认为桌面）；SelectedPath：获取或设置用户选定的路径，如果设置了该属性，打开对话框时会定位到指定路径，默认为根文 件夹，关闭对话框时根据该属性获取用户用户选定的路径；         ShowNewFolderButton：获取或设置是否显示新建对话框按钮；
     主要方法：  ShowDialog：打开该对话框，返回值为DialogResult类型值，如果为DialogResult.OK，则可以由SelectedPath属性获取用户选定的路径； 
    */
}