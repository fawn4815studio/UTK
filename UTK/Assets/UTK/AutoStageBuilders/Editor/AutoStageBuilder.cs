using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UTK.AutoStageBuilder
{
    public class AutoStageBuilder : EditorWindow
    {
        static AutoStageBuilder autoStageBuilder;

        [MenuItem("UTK/Procedural/AutoStageBuilder", false, 2)]
        static void Open()
        {
            if (autoStageBuilder == null)
            {
               autoStageBuilder = CreateInstance<AutoStageBuilder>();
            }

            autoStageBuilder.minSize = new Vector2(500, 400);
            autoStageBuilder.titleContent.text = "AutoStageBuilder";
            autoStageBuilder.ShowUtility();
        }
    }

}
