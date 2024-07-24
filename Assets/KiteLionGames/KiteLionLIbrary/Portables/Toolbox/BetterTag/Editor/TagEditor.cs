using UnityEditor;

namespace KiteLionGames.BetterTag
{
    [CustomEditor(typeof(Tag))]
    [CanEditMultipleObjects]
    public class TagEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
