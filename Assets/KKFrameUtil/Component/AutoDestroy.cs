using UnityEngine;
using System.Collections;

namespace KK.Frame.Util
{
    /// <summary>
    /// 指定时间后自动销毁
    /// 销毁是指放到对象池子里
    /// </summary>
    public class AutoDestroy : MonoBehaviour
    {
        // Use this for initialization
        public float m_period = 1.0f;
        float m_start = 0.0f;
        void OnEnable()
        {
            m_start = Time.time;
        }

        // Update is called once per frame
        void Update()
        {            
            if (Time.time - m_start >= m_period)
            {
                if (null != gameObject)
                {                    
                    //gameObject.transform.parent = null;
                    ObjectPoolController.Destroy(gameObject);                    
                }
            }
        }

        void DeleteEffect()
        {
            //gameObject.transform.parent = null;
            ObjectPoolController.Destroy(gameObject);            
        }
    }
}
