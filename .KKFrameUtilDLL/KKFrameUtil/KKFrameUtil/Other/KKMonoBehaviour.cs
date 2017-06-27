using UnityEngine;
using System.Collections;

public class KKMonoBehaviour : MonoBehaviour {
    
}

public static class MonoExtend
{
    public static T GetComponentForce<T>(this GameObject gameObject) where T : Component
    {
        if (!gameObject.GetComponent<T>())
        {
            gameObject.AddComponent<T>();
        }
        return gameObject.GetComponent<T>();
    }
}
