using UnityEditor;
using UnityEngine;

namespace UTK.RecentFileViewer
{
    public class RecentFileViewer : EditorWindow
    {
        static RecentFileViewer recentFileViewer;

        [SerializeField]
        RecentFileViewerConfig config;


        [MenuItem("UTK/RecentFileViewer",false,12)]
        static void Open()
        {
            if (recentFileViewer == null)
            {
                recentFileViewer = CreateInstance<RecentFileViewer>();
            }

            recentFileViewer.config = RecentFileViewerConfig.GetRecentFileViewerConfig();

            recentFileViewer.titleContent.text = "RecentFileViewer";
            recentFileViewer.Show();
        }

        #region Internal

        void OnGUI()
        {
            
        }

        #endregion
    }

}
