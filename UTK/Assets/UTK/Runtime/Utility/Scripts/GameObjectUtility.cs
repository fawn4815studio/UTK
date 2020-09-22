using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UTK.Runtime.Utility
{
    public static class GameObjectUtility
    {
        public static void SetActiveIfNeeded(this GameObject go,bool value)
        {
            if(go.activeSelf != value)
            {
                go.SetActive(value);
            }
        }
    }

}
