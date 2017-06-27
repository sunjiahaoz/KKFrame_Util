using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;

namespace KK.Frame.Util
{

    public class FindAssetDependenceWindow : EditorWindow
    {
        //public static FindAssetDependenceWindow editor;

        int objectCount = 1;

        //List<string> pathList = new List<string>();
        //List<Object> objectList = new List<Object>();
        Object[] objectArray = new Object[1];

        Dictionary<string, DependenciesContent> dependenciesDic = new Dictionary<string, DependenciesContent>();

        [MenuItem("Tools/Asset/FindAssetDependence")]
        public static void ShowWindow()
        {
            FindAssetDependenceWindow editor = CreateInstance<FindAssetDependenceWindow>();
            editor.titleContent.text = "FindAssetDependence";
            editor.Show();
        }

        //Object obj;

        Vector2 allAssetBundleScroll = Vector2.zero;

        bool isNeedShowAssetsDependencesAssetBundleName = true;

        bool isInited = false;

        List<GUIStyle> guiStyleList = new List<GUIStyle>();

        int maxRefCount = 0;

        bool isNeedShowPrefab = true;

        bool isNeedShowTexture2D = true;

        bool isNeedShowMaterial = true;

        bool isNeedShowTextAsset = true;

        bool isNeedShowAnimation = true;

        bool isNeedShowAnimator = true;

        bool isNeedShowShader = true;

        bool isNeedShowFont = true;

        void Init()
        {
            if (!isInited)
            {
                for (int i = 0; i < 10; i++)
                {
                    GUIStyle number = new GUIStyle();
                    Color color = Color.HSVToRGB(i / 10f, 1, 1f);
                    number.normal.textColor = color;
                    number.hover.textColor = color;
                    guiStyleList.Add(number);
                }
                isInited = true;
            }
        }

        void OnGUI()
        {
            Init();
            GUILayout.BeginHorizontal();
            GUILayout.Label("检测资源数量", GUILayout.MaxWidth(150));
            int _objectCount = EditorGUILayout.IntField(objectCount, GUILayout.MaxWidth(20));
            if (_objectCount != objectCount)
            {
                objectCount = _objectCount;
                Object[] _objectArray = new Object[_objectCount];
                for (int i = 0; i < _objectArray.Length; i++)
                {
                    if (i < objectArray.Length)
                    {
                        _objectArray[i] = objectArray[i];
                    }
                }
                objectArray = _objectArray;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("请拖入资源");
            for (int i = 0; i < objectCount; i++)
            {
                int realIndex = i % 10;
                GUILayout.Label((i + 1).ToString(), guiStyleList[realIndex], GUILayout.Width(10));
                Object _obj = EditorGUILayout.ObjectField(objectArray[i], typeof(Object));
                if (_obj != objectArray[i])
                {
                    objectArray[i] = _obj;
                }
            }
            //obj = EditorGUILayout.ObjectField(obj, typeof(Object));
            if (GUILayout.Button("开始检测"))
            {
                //pathList.Clear();
                dependenciesDic.Clear();
                maxRefCount = 0;
                allAssetBundleScroll = Vector2.zero;
                for (int count = 0; count < objectCount; count++)
                {
                    Object obj = objectArray[count];
                    if (null != obj)
                    {
                        System.Type type = obj.GetType();
                        if (type != typeof(DefaultAsset))
                        {
                            string objPath = AssetDatabase.GetAssetPath(obj);
                            string[] objPaths = AssetDatabase.GetDependencies(objPath);
                            for (int j = 0; j < objPaths.Length; j++)
                            {
                                if (objPaths[j] == objPath) continue;
                                Object loadObj = AssetDatabase.LoadMainAssetAtPath(objPaths[j]);
                                string objType = loadObj.GetType().ToString();
                                if (!dependenciesDic.ContainsKey(objType))
                                {
                                    dependenciesDic[objType] = new DependenciesContent();
                                }
                                if (!dependenciesDic[objType].pathDic.ContainsKey(objPaths[j]))
                                {
                                    dependenciesDic[objType].pathDic[objPaths[j]] = new SingleDepenciesContent();
                                }
                                dependenciesDic[objType].pathDic[objPaths[j]].count++;
                                dependenciesDic[objType].pathDic[objPaths[j]].refenerences.Add(obj);
                                dependenciesDic[objType].pathDic[objPaths[j]].refenerencePaths.Add(objPath);
                                dependenciesDic[objType].pathDic[objPaths[j]].loadObject = loadObj;
                                dependenciesDic[objType].pathDic[objPaths[j]].refenerencesIndexs.Add(count);
                            }
                        }
                        else
                        {
                            string realPath = Application.dataPath + AssetDatabase.GetAssetPath(obj).Substring(6);
                            DirectoryInfo dir = new DirectoryInfo(realPath);
                            if (dir.Exists)
                            {
                                List<GameObject> objList = new List<GameObject>();
                                GetAllPrefabs(dir, objList);
                                for (int i = 0; i < objList.Count; i++)
                                {
                                    string objPath = AssetDatabase.GetAssetPath(objList[i]);
                                    string[] objPaths = AssetDatabase.GetDependencies(objPath);
                                    for (int j = 0; j < objPaths.Length; j++)
                                    {
                                        if (objPaths[j] == objPath) continue;
                                        Object loadObj = AssetDatabase.LoadMainAssetAtPath(objPaths[j]);
                                        string objType = loadObj.GetType().ToString();
                                        if (!dependenciesDic.ContainsKey(objType))
                                        {
                                            dependenciesDic[objType] = new DependenciesContent();
                                        }
                                        if (!dependenciesDic[objType].pathDic.ContainsKey(objPaths[j]))
                                        {
                                            dependenciesDic[objType].pathDic[objPaths[j]] = new SingleDepenciesContent();
                                        }
                                        dependenciesDic[objType].pathDic[objPaths[j]].count++;
                                        dependenciesDic[objType].pathDic[objPaths[j]].refenerences.Add(objList[i]);
                                        dependenciesDic[objType].pathDic[objPaths[j]].refenerencePaths.Add(objPath);
                                        dependenciesDic[objType].pathDic[objPaths[j]].loadObject = loadObj;
                                        if (!dependenciesDic[objType].pathDic[objPaths[j]].refenerencesIndexs.Contains(count))
                                        {
                                            dependenciesDic[objType].pathDic[objPaths[j]].refenerencesIndexs.Add(count);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var item in dependenciesDic)
                {
                    foreach (var item2 in item.Value.pathDic)
                    {
                        if (item2.Value.refenerencesIndexs.Count > maxRefCount)
                        {
                            maxRefCount = item2.Value.refenerencesIndexs.Count;
                        }
                    }
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("需要显示的资源类型：");
            GUILayout.Label("Prefab");
            isNeedShowPrefab = EditorGUILayout.Toggle(isNeedShowPrefab);
            GUILayout.Label("Texture");
            isNeedShowTexture2D = EditorGUILayout.Toggle(isNeedShowTexture2D);
            GUILayout.Label("Material");
            isNeedShowMaterial = EditorGUILayout.Toggle(isNeedShowMaterial);
            GUILayout.Label("TextAsset");
            isNeedShowTextAsset = EditorGUILayout.Toggle(isNeedShowTextAsset);
            GUILayout.Label("Animation");
            isNeedShowAnimation = EditorGUILayout.Toggle(isNeedShowAnimation);
            GUILayout.Label("Animator");
            isNeedShowAnimator = EditorGUILayout.Toggle(isNeedShowAnimator);
            GUILayout.Label("Shader");
            isNeedShowShader = EditorGUILayout.Toggle(isNeedShowShader);
            GUILayout.Label("Font");
            isNeedShowFont = EditorGUILayout.Toggle(isNeedShowFont);
            if (GUILayout.Button("全选"))
            {
                isNeedShowPrefab = true;
                isNeedShowTexture2D = true;
                isNeedShowMaterial = true;
                isNeedShowTextAsset = true;
                isNeedShowAnimation = true;
                isNeedShowAnimator = true;
                isNeedShowShader = true;
                isNeedShowFont = true;
            }
            if (GUILayout.Button("全不选"))
            {
                isNeedShowPrefab = false;
                isNeedShowTexture2D = false;
                isNeedShowMaterial = false;
                isNeedShowTextAsset = false;
                isNeedShowAnimation = false;
                isNeedShowAnimator = false;
                isNeedShowShader = false;
                isNeedShowFont = false;
            }
            GUILayout.EndHorizontal();

            if (dependenciesDic.Count > 0)
            {
                allAssetBundleScroll = GUILayout.BeginScrollView(allAssetBundleScroll);
                string prefabType = typeof(GameObject).ToString();
                if (dependenciesDic.ContainsKey(prefabType) && isNeedShowPrefab)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Prefab:", GUILayout.Width(100));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("资源文件夹", GUILayout.Width(400));
                    GUILayout.Label("AssetBundle名", GUILayout.Width(175));
                    GUILayout.Label("引用次数", GUILayout.Width(50));
                    GUILayout.Label("", GUILayout.Width(10 * maxRefCount));
                    GUILayout.Label("资源引用");
                    GUILayout.EndHorizontal();

                    foreach (var item in dependenciesDic[prefabType].pathDic)
                    {
                        DrawSingleAsset(item.Key, item.Value);
                    }
                }
                string textureType = typeof(Texture2D).ToString();
                if (dependenciesDic.ContainsKey(textureType) && isNeedShowTexture2D)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Texture:", GUILayout.Width(100));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("资源文件夹", GUILayout.Width(400));
                    GUILayout.Label("AssetBundle名", GUILayout.Width(175));
                    GUILayout.Label("引用次数", GUILayout.Width(50));
                    GUILayout.Label("引用源", GUILayout.Width(10 * maxRefCount));
                    GUILayout.Label("资源引用");
                    GUILayout.EndHorizontal();

                    foreach (var item in dependenciesDic[textureType].pathDic)
                    {
                        DrawSingleAsset(item.Key, item.Value);
                    }
                }
                string materialType = typeof(Material).ToString();
                if (dependenciesDic.ContainsKey(materialType) && isNeedShowMaterial)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Material:", GUILayout.Width(100));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("资源文件夹", GUILayout.Width(400));
                    GUILayout.Label("AssetBundle名", GUILayout.Width(175));
                    GUILayout.Label("引用次数", GUILayout.Width(50));
                    GUILayout.Label("引用源", GUILayout.Width(10 * maxRefCount));
                    GUILayout.Label("资源引用");
                    GUILayout.EndHorizontal();

                    foreach (var item in dependenciesDic[materialType].pathDic)
                    {
                        DrawSingleAsset(item.Key, item.Value);
                    }
                }
                string textAssetType = typeof(TextAsset).ToString();
                if (dependenciesDic.ContainsKey(textAssetType) && isNeedShowTextAsset)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("TextAsset:", GUILayout.Width(100));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("资源文件夹", GUILayout.Width(400));
                    GUILayout.Label("AssetBundle名", GUILayout.Width(175));
                    GUILayout.Label("引用次数", GUILayout.Width(50));
                    GUILayout.Label("引用源", GUILayout.Width(10 * maxRefCount));
                    GUILayout.Label("资源引用");
                    GUILayout.EndHorizontal();

                    foreach (var item in dependenciesDic[textAssetType].pathDic)
                    {
                        DrawSingleAsset(item.Key, item.Value);
                    }
                }

                string animationType = typeof(AnimationClip).ToString();
                if (dependenciesDic.ContainsKey(animationType) && isNeedShowAnimation)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Animation:", GUILayout.Width(100));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("资源文件夹", GUILayout.Width(400));
                    GUILayout.Label("AssetBundle名", GUILayout.Width(175));
                    GUILayout.Label("引用次数", GUILayout.Width(50));
                    GUILayout.Label("引用源", GUILayout.Width(10 * maxRefCount));
                    GUILayout.Label("资源引用");
                    GUILayout.EndHorizontal();

                    foreach (var item in dependenciesDic[animationType].pathDic)
                    {
                        DrawSingleAsset(item.Key, item.Value);
                    }
                }

                string animatorType = typeof(UnityEditor.Animations.AnimatorController).ToString();
                if (dependenciesDic.ContainsKey(animatorType) && isNeedShowAnimator)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Animator:", GUILayout.Width(100));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("资源文件夹", GUILayout.Width(400));
                    GUILayout.Label("AssetBundle名", GUILayout.Width(175));
                    GUILayout.Label("引用次数", GUILayout.Width(50));
                    GUILayout.Label("引用源", GUILayout.Width(10 * maxRefCount));
                    GUILayout.Label("资源引用");
                    GUILayout.EndHorizontal();

                    foreach (var item in dependenciesDic[animatorType].pathDic)
                    {
                        DrawSingleAsset(item.Key, item.Value);
                    }
                }


                string shaderType = typeof(Shader).ToString();
                if (dependenciesDic.ContainsKey(shaderType) && isNeedShowShader)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Shader:", GUILayout.Width(100));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("资源文件夹", GUILayout.Width(400));
                    GUILayout.Label("AssetBundle名", GUILayout.Width(175));
                    GUILayout.Label("引用次数", GUILayout.Width(50));
                    GUILayout.Label("引用源", GUILayout.Width(10 * maxRefCount));
                    GUILayout.Label("资源引用");
                    GUILayout.EndHorizontal();

                    foreach (var item in dependenciesDic[shaderType].pathDic)
                    {
                        DrawSingleAsset(item.Key, item.Value);
                    }
                }

                string fontType = typeof(Font).ToString();
                if (dependenciesDic.ContainsKey(fontType) && isNeedShowFont)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Font:", GUILayout.Width(100));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("资源文件夹", GUILayout.Width(400));
                    GUILayout.Label("AssetBundle名", GUILayout.Width(175));
                    GUILayout.Label("引用次数", GUILayout.Width(50));
                    GUILayout.Label("引用源", GUILayout.Width(10 * maxRefCount));
                    GUILayout.Label("资源引用");
                    GUILayout.EndHorizontal();

                    foreach (var item in dependenciesDic[fontType].pathDic)
                    {
                        DrawSingleAsset(item.Key, item.Value);
                    }
                }
                GUILayout.EndScrollView();
            }

            GUILayout.BeginHorizontal();
            isNeedShowAssetsDependencesAssetBundleName = EditorGUILayout.Toggle(isNeedShowAssetsDependencesAssetBundleName, GUILayout.Width(20));
            GUILayout.Label("是否需要显示依赖中有AssetBundle名字的资源");
            GUILayout.EndHorizontal();
        }

        void DrawSingleAsset(string path, SingleDepenciesContent singleContent)
        {
            Object loadObj = singleContent.loadObject;
            string assetBundleName = "";
            AssetImporter importer = AssetImporter.GetAtPath(path);
            if (null != importer)
            {
                assetBundleName = importer.assetBundleName;
            }
            if (loadObj.GetType() == typeof(MonoScript)) return;
            if (!isNeedShowAssetsDependencesAssetBundleName)
            {
                if (string.IsNullOrEmpty(assetBundleName))
                {
                    GUILayout.BeginHorizontal();
                    int index = path.LastIndexOf("/");
                    string folderPath = "";
                    if (index > 0)
                    {
                        folderPath = path.Substring(0, index) + "/";
                    }
                    else
                    {
                        folderPath = path;
                    }
                    GUILayout.Label(folderPath, GUILayout.Width(400));
                    GUILayout.Label(assetBundleName, GUILayout.Width(175));
                    GUILayout.Label(singleContent.count.ToString(), GUILayout.Width(50));
                    for (int i = 0; i < singleContent.refenerencesIndexs.Count; i++)
                    {
                        if (i < maxRefCount)
                        {
                            int realIndex = singleContent.refenerencesIndexs[i] % 10;
                            GUILayout.Label((singleContent.refenerencesIndexs[i] + 1).ToString(), guiStyleList[realIndex], GUILayout.Width(10));
                        }
                        else
                        {
                            GUILayout.Label("", guiStyleList[0], GUILayout.Width(10));
                        }
                    }
                    EditorGUILayout.ObjectField(loadObj, loadObj.GetType(), false);
                    if (!singleContent.isExtend)
                    {
                        if (GUILayout.Button("查找被引用文件"))
                        {
                            singleContent.isExtend = true;
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("收起被引用文件"))
                        {
                            singleContent.isExtend = false;
                        }
                    }
                    GUILayout.EndHorizontal();

                    if (singleContent.isExtend)
                    {
                        for (int i = 0; i < singleContent.refenerences.Count; i++)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label(singleContent.refenerencePaths[i], GUILayout.Width(500));
                            EditorGUILayout.ObjectField(singleContent.refenerences[i], singleContent.refenerences[i].GetType(), false);
                            GUILayout.EndHorizontal();
                        }
                    }
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
                int index = path.LastIndexOf("/");
                string folderPath = "";
                if (index > 0)
                {
                    folderPath = path.Substring(0, index) + "/";
                }
                else
                {
                    folderPath = path;
                }
                GUILayout.Label(folderPath, GUILayout.Width(400));
                GUILayout.Label(assetBundleName, GUILayout.Width(175));
                GUILayout.Label(singleContent.count.ToString(), GUILayout.Width(50));
                for (int i = 0; i < maxRefCount; i++)
                {
                    if (i < singleContent.refenerencesIndexs.Count)
                    {
                        int realIndex = singleContent.refenerencesIndexs[i] % 10;
                        GUILayout.Label((singleContent.refenerencesIndexs[i] + 1).ToString(), guiStyleList[realIndex], GUILayout.Width(10));
                    }
                    else
                    {
                        GUILayout.Label("", guiStyleList[0], GUILayout.Width(10));
                    }
                }
                EditorGUILayout.ObjectField(loadObj, loadObj.GetType(), false);
                if (!singleContent.isExtend)
                {
                    if (GUILayout.Button("查找被引用文件"))
                    {
                        singleContent.isExtend = true;
                    }
                }
                else
                {
                    if (GUILayout.Button("收起被引用文件"))
                    {
                        singleContent.isExtend = false;
                    }
                }
                GUILayout.EndHorizontal();

                if (singleContent.isExtend)
                {
                    for (int i = 0; i < singleContent.refenerences.Count; i++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(singleContent.refenerencePaths[i], GUILayout.Width(500));
                        EditorGUILayout.ObjectField(singleContent.refenerences[i], singleContent.refenerences[i].GetType(), false);
                        GUILayout.EndHorizontal();
                    }
                }
            }
        }

        void GetAllPrefabs(DirectoryInfo dir, List<GameObject> objectList)
        {
            FileInfo[] allFile = dir.GetFiles();
            foreach (FileInfo fi in allFile)
            {
                if (fi.Extension.Equals(".prefab"))
                {
                    string filePath = fi.FullName;
                    filePath = filePath.Replace(@"\", "/").Replace(Application.dataPath, "Assets");

                    GameObject gameObj = AssetDatabase.LoadAssetAtPath(filePath, typeof(GameObject)) as GameObject;

                    if (gameObj != null)
                        objectList.Add(gameObj);
                }
            }

            DirectoryInfo[] allDir = dir.GetDirectories();
            foreach (DirectoryInfo d in allDir)
            {
                GetAllPrefabs(d, objectList);
            }
        }
    }

    public class DependenciesContent
    {
        public SortedDictionary<string, SingleDepenciesContent> pathDic = new SortedDictionary<string, SingleDepenciesContent>();
    }

    public class SingleDepenciesContent
    {
        public int count;

        public List<int> refenerencesIndexs = new List<int>();

        public List<Object> refenerences = new List<Object>();

        public List<string> refenerencePaths = new List<string>();

        public Object loadObject;

        public bool isExtend = false;
    }
#endif
}