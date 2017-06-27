using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UI;

namespace KK.Frame.Util
{

    public class FindAssetReferenceWindow : EditorWindow
    {
        public static FindAssetReferenceWindow editor;

        [MenuItem("Tools/Asset/FindAssetReference")]
        public static void ShowWindow()
        {
            editor = EditorWindow.GetWindow<FindAssetReferenceWindow>();
        }

        List<SingleReferenceContent> referenceList = new List<SingleReferenceContent>();

        Vector2 allAssetBundleScroll = Vector2.zero;

        Object originObj;

        Object referObj;

        void SetAllChild(Transform trRoot, System.Action<Transform> process)
        {
            process(trRoot);
            for (int i = 0; i < trRoot.childCount; ++i)
            {
                process(trRoot.GetChild(i));
            }
        }

        void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("请拖入待检测资源");
            originObj = EditorGUILayout.ObjectField(originObj, typeof(GameObject));
            GUILayout.Label("请拖入待检测资源");
            referObj = EditorGUILayout.ObjectField(referObj, typeof(Object));
            if (GUILayout.Button("开始检测"))
            {
                referenceList.Clear();
                if (null == originObj || null == referObj) return;
                string refGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(referObj));
                GameObject go = originObj as GameObject;
                if (referObj.GetType() == typeof(Texture2D))
                {
                    SetAllChild(go.transform, (t) =>
                    {
                        Image image = t.GetComponent<Image>();
                        if (null != image)
                        {
                            Sprite sp = image.sprite;
                            if (null == sp)
                            {
                                sp = image.overrideSprite;
                            }
                            if (null != sp)
                            {
                                string spriteGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(sp));
                                if (spriteGuid == refGuid)
                                {
                                    SingleReferenceContent s = new SingleReferenceContent() { refObj = t.gameObject, sp = sp };
                                    referenceList.Add(s);
                                }
                            }
                        }

                    //UIImage uiImage = t.GetComponent<UIImage>();
                    //if (null != uiImage)
                    //{
                    //    Sprite sp = uiImage.sprite;
                    //    if (null == sp)
                    //    {
                    //        sp = uiImage.overrideSprite;
                    //    }
                    //    if (null != sp)
                    //    {
                    //        string spriteGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(sp));
                    //        if (spriteGuid == refGuid)
                    //        {
                    //            SingleReferenceContent s = new SingleReferenceContent() { refObj = t.gameObject, sp = sp };
                    //            referenceList.Add(s);
                    //        }
                    //    }
                    //}

                    SpriteRenderer spRenderer = t.GetComponent<SpriteRenderer>();
                        if (null != spRenderer)
                        {
                            Sprite sp = spRenderer.sprite;
                            if (null != sp)
                            {
                                string spriteGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(sp));
                                if (spriteGuid == refGuid)
                                {
                                    SingleReferenceContent s = new SingleReferenceContent() { refObj = t.gameObject, sp = sp };
                                    referenceList.Add(s);
                                }
                            }
                        }

                        Renderer[] renderers = t.GetComponents<Renderer>();
                        if (null != renderers)
                        {
                            for (int i = 0; i < renderers.Length; i++)
                            {
                                Material material = renderers[i].sharedMaterial;
                                if (null != material)
                                {
                                    if (material.HasProperty("_MainTex"))
                                    {
                                        Texture texture = material.mainTexture;
                                        string textureGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(texture));
                                        if (textureGuid == refGuid)
                                        {
                                            SingleReferenceContent s = new SingleReferenceContent() { refObj = t.gameObject };
                                            referenceList.Add(s);
                                        }
                                    }
                                }
                            }
                        }
                    });
                }
                else if (referObj.GetType() == typeof(Material))
                {
                    SetAllChild(go.transform, (t) =>
                    {
                        Renderer renderer = t.GetComponent<Renderer>();
                        if (null != renderer)
                        {
                            Material material = renderer.sharedMaterial;
                            string materialGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(material));
                            if (materialGuid == refGuid)
                            {
                                SingleReferenceContent s = new SingleReferenceContent() { refObj = t.gameObject };
                                referenceList.Add(s);
                            }
                        }
                    });
                }
            }
            GUILayout.EndHorizontal();

            if (referenceList.Count > 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("引用物体", GUILayout.MinWidth(250));
                GUILayout.Label("引用Sprite", GUILayout.MinWidth(250));
                GUILayout.EndHorizontal();
                allAssetBundleScroll = GUILayout.BeginScrollView(allAssetBundleScroll);
                for (int i = 0; i < referenceList.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(referenceList[i].refObj, typeof(GameObject), GUILayout.MinWidth(250));
                    if (null != referenceList[i].sp)
                    {
                        EditorGUILayout.ObjectField(referenceList[i].sp, typeof(Sprite), GUILayout.MinWidth(250));
                    }
                    else
                    {
                        GUILayout.Label("", GUILayout.MinWidth(250));
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("没有找到引用或者原始数据错误");
                GUILayout.EndHorizontal();
            }
        }
    }
    public class SingleReferenceContent
    {
        public GameObject refObj;

        public Sprite sp;
    }
#endif
}