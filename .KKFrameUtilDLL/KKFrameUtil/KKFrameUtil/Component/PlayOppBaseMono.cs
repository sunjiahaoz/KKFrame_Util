/********************************************************************
	created:	2017/04/04 		
	file base:	PlayOppBaseMono.cs	
	author:		sunjiahaoz
	
	purpose:	根据Mono的几个关键函数，在某个时机执行指定内容
*********************************************************************/
using UnityEngine;
using System.Collections;

namespace KK.Frame.Util
{
    /// <summary>
    /// 根据Mono的几个关键函数，在某个时机执行指定内容
    /// 一般作为基类
    /// </summary>
    public class PlayOppBaseMono : MonoBehaviour
    {
        /// <summary>
        /// 执行时机
        /// </summary>
        public enum PlayOpp
        {            
            Awake,
            Enable,
            Start,
            /// <summary>
            /// 每帧执行
            /// </summary>
            Update,
            /// <summary>
            /// 手动执行
            /// </summary>
            Manual,
        }
        public PlayOpp _opp = PlayOpp.Manual;
        public virtual void Awake()
        {
            if (_opp == PlayOpp.Awake)
            {
                Excute();
            }
        }

        public virtual void Enable()
        {
            if (_opp == PlayOpp.Enable)
            {
                Excute();
            }
        }

        public virtual void Start()
        {
            if (_opp == PlayOpp.Start)
            {
                Excute();
            }
        }

        public virtual void Update()
        {
            if (_opp == PlayOpp.Update)
            {
                Excute();
            }
        }

        /// <summary>
        /// 执行内容
        /// </summary>
        public virtual void Excute()
        {
            //  todo
        }
    }
}
