/********************************************************************
	created:	2016/11/30 		
	file base:	CustomDictionary.cs	
	author:		sunjiahaoz
	
	purpose:	自定义的简单Dictionary，主要是使用两个List进行实现，避免遍历Dict时产生垃圾
    当然这种实现并不高效，效率敏感的地方不要用,数据量大的也不要用
*********************************************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KK.Frame.Util
{
    /// <summary>
    /// 自定义的简单Dictionary，主要是使用两个List进行实现，避免遍历Dict时产生垃圾
    /// 当然这种实现并不高效，效率敏感的地方不要用,数据量大的也不要用
    /// </summary>
    /// <typeparam name="T">Key</typeparam>
    /// <typeparam name="U">Value</typeparam>
    public class CustomDictionary<T, U>
    {
        List<T> _lstKey = new List<T>();
        List<U> _lstValue = new List<U>();

        // 谁用这两个谁作死
        public List<T> lstKey
        {
            get { return _lstKey; }
        }
        public List<U> lstValue
        {
            get { return _lstValue; }
        }

        public U this[T key]
        {
            get
            {
                int nIndex = GetKeyIndex(key);
                if (nIndex < 0)
                {
                    return default(U);
                }
                else
                {
                    return _lstValue[nIndex];
                }
            }
            set
            {
                int nIndex = GetKeyIndex(key);
                if (nIndex < 0)
                {
                    _lstKey.Add(key);
                    _lstValue.Add(value);
                }
                else
                {
                    _lstValue[nIndex] = value;
                }
            }
        }

        public int Count
        {
            get { return _lstKey.Count; }
        }

        public void Add(T key, U value)
        {
            int nIndex = GetKeyIndex(key);
            if (nIndex < 0)
            {
                _lstKey.Add(key);
                _lstValue.Add(value);
            }
            else
            {
                throw new System.Exception(key + "已经存在了，不能重复添加");
            }
        }

        public void Remove(T key)
        {
            int nIndex = GetKeyIndex(key);
            RemoveAt(nIndex);
        }

        public void RemoveAt(int nIndex)
        {
            if (nIndex < 0)
            {
                return;
            }
            _lstKey.RemoveAt(nIndex);
            _lstValue.RemoveAt(nIndex);
        }

        public void Clear()
        {
            _lstKey.Clear();
            _lstValue.Clear();
        }

        public bool Contains(T key)
        {
            return _lstKey.Contains(key);
        }

        // 通过索引获取数据
        public KeyValuePair<T, U> GetAt(int nIndex)
        {
            if (nIndex < 0
                || nIndex >= Count)
            {
                return default(KeyValuePair<T, U>);
            }
            return new KeyValuePair<T, U>(_lstKey[nIndex], _lstValue[nIndex]);
        }

        public T GetKeyAt(int nIndex)
        {
            if (nIndex < 0
                || nIndex >= Count)
            {
                return default(T);
            }
            return _lstKey[nIndex];
        }

        public U GetValueAt(int nIndex)
        {
            if (nIndex < 0
                || nIndex >= Count)
            {
                return default(U);
            }
            return _lstValue[nIndex];
        }

        // 遍历
        public void TravelDict(System.Action<T, U> actionProcess)
        {
            for (int i = 0; i < Count; ++i)
            {
                actionProcess(_lstKey[i], _lstValue[i]);
            }
        }

        // 测试输出
        public void DebugLogOug()
        {
            TravelDict((key, value) => 
            {
                Debug.Log("Key:" + key + " Value:" + value);
            });
        }

        int GetKeyIndex(T Key)
        {
            return _lstKey.IndexOf(Key);
        }
    }
}
