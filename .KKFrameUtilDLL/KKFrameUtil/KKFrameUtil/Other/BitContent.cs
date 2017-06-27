/********************************************************************
	created:	2016/12/05 		
	file base:	BitContent.cs	
	author:		sunjiahaoz
	
	purpose:	工具类，用于保存适用于只有两种状态值值数量又多的数据，也即用0,1即可表示的数据。
    用int来保存，一个int可以保存32个数据
    比如某个序列中的对象是否被标记，即可以用索引来查找或设置数据
*********************************************************************/
using UnityEngine;
using System.Collections;

namespace KK.Frame.Util
{
    /// <summary>
    /// 工具类，用于保存适用于只有两种状态值值数量又多的数据，也即用0,1即可表示的数据。
    /// 用int来保存，一个int可以保存32个数据
    /// 比如某个序列中的对象是否被标记，即可以用索引来查找或设置数据
    /// </summary>
    public class BitContent
    {
        int[] m_pBitContent;        // 保存所有信息
        int m_nBitDataSize; // 设置的需要保存的数据数量
        int m_nIntSize;         // 根据Init的数量计算出需要使用多少个int值
        int m_nMemBitSize = sizeof(int) * 8;        // int的字节数

        /// <summary>
        /// 初始化大小，必须调用
        /// </summary>
        /// <param name="nObstacleSize">要标记的最大数量</param>
        /// <param name="nInit">全部初始化的值，默认为0</param>
        public void Init(int nObstacleSize, int nInit = 0)
        {
            // 存储障碍的数据的字节数
            //m_nMemBitSize = sizeof(int) * 8;

            // 障碍数量
            m_nBitDataSize = nObstacleSize;

            // 要使用多少个int值保存这些障碍
            m_nIntSize = nObstacleSize / m_nMemBitSize;
            if (nObstacleSize % m_nMemBitSize != 0)
            {
                m_nIntSize++;
            }
            if (m_nIntSize == 0 && nObstacleSize != 0)
            {
                m_nIntSize++;
            }

            // 保存障碍容器
            m_pBitContent = new int[m_nIntSize];
            // 初始化为全部为非障碍
            for (int i = 0; i < m_nIntSize; ++i)
            {
                m_pBitContent[i] = nInit;
            }
        }

        /// <summary>
        /// 通过其他int数据进行设置值
        /// </summary>
        /// <param name="pObstacleData"></param>
        /// <param name="nDataSize"></param>
        public void SetData(int[] pObstacleData, int nDataSize)
        {
            if (m_pBitContent == null
                || nDataSize > m_nIntSize)
            {
                m_pBitContent = null;
                m_pBitContent = new int[nDataSize];
            }
            m_nIntSize = nDataSize;
            m_nBitDataSize = nDataSize * 32;

            for (int i = 0; i < nDataSize; ++i)
            {
                m_pBitContent[i] = pObstacleData[i];
            }
        }

        /// <summary>
        /// 获取int数据大小
        /// </summary>
        /// <returns></returns>
        public int GetDataSize() { return m_pBitContent.Length; }
        /// <summary>
        /// 根据索引过去具体的int数据
        /// </summary>
        /// <param name="nIndex"></param>
        /// <returns></returns>
        public int GetDataByIndex(int nIndex)
        {
            if (nIndex < 0
                || nIndex >= m_pBitContent.Length)
            {
                return 0;
            }
            return m_pBitContent[nIndex];
        }

        /// <summary>
        /// 是否所有数据为0
        /// </summary>
        /// <returns></returns>
        public bool IsNone()
        {
            bool bIsNone = true;

            for (int i = 0; i < m_pBitContent.Length; ++i)
            {
                if (m_pBitContent[i] != 0)
                {
                    bIsNone = false;
                    break;
                }
            }
            return bIsNone;
        }

        /// <summary>
        /// 设置指定位置处的bool标记
        /// </summary>
        /// <param name="nPos">位置，该位置不应该大于初始化时传入的大小</param>
        /// <param name="bYes">标记</param>
        /// <returns></returns>
        public int SetValue(int nPos, bool bYes)
        {
            if (nPos >= m_nBitDataSize)     // 超出范围
            {
                return 0;
            }

            // 先计算出该障碍所处的 位 会在哪个数据里面
            int nIntPos = nPos / m_nMemBitSize;

            // 再计算在该数据的哪一位
            int nObPos = (nPos % m_nMemBitSize);
            int unTerrain = 0;
            if (bYes)
            {
                unTerrain = 1;
                unTerrain = unTerrain << nObPos;
                m_pBitContent[nIntPos] |= unTerrain;
            }
            else
            {
                unTerrain = 1;
                unTerrain = unTerrain << nObPos;
                unTerrain = ~unTerrain;
                m_pBitContent[nIntPos] &= unTerrain;
            }
            return 1;
        }

        /// <summary>
        /// 获取指定位置的标记
        /// </summary>
        /// <param name="nPos">位置索引，该位置不应该大于初始化时的大小</param>
        /// <returns></returns>
        public bool GetValue(int nPos)
        {
            if (nPos >= m_nBitDataSize) // 超出范围
            {
                return false;
            }

            // 计算出该障碍所处的 位 会在哪个数据里面
            int nIntPos = nPos / m_nMemBitSize;
            // 计算该数据的哪一位
            int nObPos = (nPos % m_nMemBitSize);

            int index = 1;
            index <<= nObPos;

            index &= m_pBitContent[nIntPos];

            return (index != 0);
        }

        /// <summary>
        /// 重置所有数据为0
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < m_nIntSize; ++i)
            {
                m_pBitContent[i] = 0;
            }
        }

        /// <summary>
        /// log输出
        /// </summary>
        public void DebugOutLog()
        {
            string strValue = "";
            for (int i = 0; i < m_nBitDataSize; ++i)
            {
                strValue += GetValue(i) ? "1" : "0";
            }
            Debug.Log("<color=cyan>"+ strValue + "</color>");
        }
    }
}
