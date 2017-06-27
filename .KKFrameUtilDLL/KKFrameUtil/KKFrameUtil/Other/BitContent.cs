/********************************************************************
	created:	2016/12/05 		
	file base:	BitContent.cs	
	author:		sunjiahaoz
	
	purpose:	�����࣬���ڱ���������ֻ������״ֵֵ̬�����ֶ�����ݣ�Ҳ����0,1���ɱ�ʾ�����ݡ�
    ��int�����棬һ��int���Ա���32������
    ����ĳ�������еĶ����Ƿ񱻱�ǣ������������������һ���������
*********************************************************************/
using UnityEngine;
using System.Collections;

namespace KK.Frame.Util
{
    /// <summary>
    /// �����࣬���ڱ���������ֻ������״ֵֵ̬�����ֶ�����ݣ�Ҳ����0,1���ɱ�ʾ�����ݡ�
    /// ��int�����棬һ��int���Ա���32������
    /// ����ĳ�������еĶ����Ƿ񱻱�ǣ������������������һ���������
    /// </summary>
    public class BitContent
    {
        int[] m_pBitContent;        // ����������Ϣ
        int m_nBitDataSize; // ���õ���Ҫ�������������
        int m_nIntSize;         // ����Init�������������Ҫʹ�ö��ٸ�intֵ
        int m_nMemBitSize = sizeof(int) * 8;        // int���ֽ���

        /// <summary>
        /// ��ʼ����С���������
        /// </summary>
        /// <param name="nObstacleSize">Ҫ��ǵ��������</param>
        /// <param name="nInit">ȫ����ʼ����ֵ��Ĭ��Ϊ0</param>
        public void Init(int nObstacleSize, int nInit = 0)
        {
            // �洢�ϰ������ݵ��ֽ���
            //m_nMemBitSize = sizeof(int) * 8;

            // �ϰ�����
            m_nBitDataSize = nObstacleSize;

            // Ҫʹ�ö��ٸ�intֵ������Щ�ϰ�
            m_nIntSize = nObstacleSize / m_nMemBitSize;
            if (nObstacleSize % m_nMemBitSize != 0)
            {
                m_nIntSize++;
            }
            if (m_nIntSize == 0 && nObstacleSize != 0)
            {
                m_nIntSize++;
            }

            // �����ϰ�����
            m_pBitContent = new int[m_nIntSize];
            // ��ʼ��Ϊȫ��Ϊ���ϰ�
            for (int i = 0; i < m_nIntSize; ++i)
            {
                m_pBitContent[i] = nInit;
            }
        }

        /// <summary>
        /// ͨ������int���ݽ�������ֵ
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
        /// ��ȡint���ݴ�С
        /// </summary>
        /// <returns></returns>
        public int GetDataSize() { return m_pBitContent.Length; }
        /// <summary>
        /// ����������ȥ�����int����
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
        /// �Ƿ���������Ϊ0
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
        /// ����ָ��λ�ô���bool���
        /// </summary>
        /// <param name="nPos">λ�ã���λ�ò�Ӧ�ô��ڳ�ʼ��ʱ����Ĵ�С</param>
        /// <param name="bYes">���</param>
        /// <returns></returns>
        public int SetValue(int nPos, bool bYes)
        {
            if (nPos >= m_nBitDataSize)     // ������Χ
            {
                return 0;
            }

            // �ȼ�������ϰ������� λ �����ĸ���������
            int nIntPos = nPos / m_nMemBitSize;

            // �ټ����ڸ����ݵ���һλ
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
        /// ��ȡָ��λ�õı��
        /// </summary>
        /// <param name="nPos">λ����������λ�ò�Ӧ�ô��ڳ�ʼ��ʱ�Ĵ�С</param>
        /// <returns></returns>
        public bool GetValue(int nPos)
        {
            if (nPos >= m_nBitDataSize) // ������Χ
            {
                return false;
            }

            // ��������ϰ������� λ �����ĸ���������
            int nIntPos = nPos / m_nMemBitSize;
            // ��������ݵ���һλ
            int nObPos = (nPos % m_nMemBitSize);

            int index = 1;
            index <<= nObPos;

            index &= m_pBitContent[nIntPos];

            return (index != 0);
        }

        /// <summary>
        /// ������������Ϊ0
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < m_nIntSize; ++i)
            {
                m_pBitContent[i] = 0;
            }
        }

        /// <summary>
        /// log���
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
