using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace Plugins.StickyNode.Editor
{
    [CustomNodeEditor(typeof(StickyNoteNode))]
    public class StickyNoteNodeEditor : NodeEditor
    {
        private static readonly Color32 BackgroundColor = new(255, 247, 64, 255);
        private static readonly Color32 TextColor = new(40, 40, 40, 255);
        private static Texture2D _roundedBody;
        private static Texture2D _squaredBody;
        private static Font _stickyFont;
        private bool _editing;

        private bool _isDragging;
        private StickyNoteNode _stickyNoteNode;

        private static Texture2D roundedBody => _roundedBody != null
            ? _roundedBody
            : _roundedBody = Resources.Load<Texture2D>("flat_node_rounded");

        private static Texture2D squaredBody => _squaredBody != null
            ? _squaredBody
            : _squaredBody = Resources.Load<Texture2D>("flat_node_squared");

        private static Font stickyFont => _stickyFont != null
            ? _stickyFont
            : _stickyFont = Resources.Load<Font>("sticky_note_node_font");

        private bool IsSquared => _stickyNoteNode != null && _stickyNoteNode.squared;

        public override void OnHeaderGUI()
        {
            GUILayout.Label(string.Empty, NodeEditorResources.styles.nodeHeader, GUILayout.Height(10));
        }

        public override GUIStyle GetBodyStyle()
        {
            var nodeBody = new GUIStyle();
            nodeBody.normal.background = IsSquared ? squaredBody : roundedBody;
            nodeBody.border = new RectOffset(32, 32, 32, 32);
            nodeBody.padding = new RectOffset(16, 16, 4, 16);
            return nodeBody;
        }

        public override void OnBodyGUI()
        {
            _stickyNoteNode = target as StickyNoteNode;
            if (_stickyNoteNode == null) return;

            var prevBackgroundColor = GUI.backgroundColor;
            var prevFont = GUI.skin.label.font;

            GUI.backgroundColor = BackgroundColor;
            GUI.skin.font = stickyFont;

            GUIStyle labelGUIStyle = new()
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = _stickyNoteNode.fontSize,
                normal = new GUIStyleState
                {
                    textColor = TextColor
                }
            };

            GUILayout.Label(_stickyNoteNode.text, labelGUIStyle, GUILayout.Height(_stickyNoteNode.height));

            GUI.skin.font = prevFont;
            GUI.backgroundColor = prevBackgroundColor;
            
            Resizable(_stickyNoteNode, ref _isDragging, ref _stickyNoteNode.width, ref _stickyNoteNode.height);
        }

        public override Color GetTint()
        {
            return BackgroundColor;
        }

        public override int GetWidth()
        {
            return _stickyNoteNode ? _stickyNoteNode.width : 200;
        }
        
        private static void Resizable(Node node, ref bool isDragging, ref int width, ref int height)
        {
            var e = Event.current;
            Vector2 size;

            switch (e.type)
            {
                case EventType.MouseDrag:
                    if (isDragging)
                    {
                        width = Mathf.Max(100, (int)e.mousePosition.x + 16);
                        height = Mathf.Max(100, (int)e.mousePosition.y - 34);
                        NodeEditorWindow.current.Repaint();
                    }

                    break;
                case EventType.MouseDown:
                    // Ignore everything except left clicks
                    if (e.button != 0) return;
                    if (NodeEditorWindow.current.nodeSizes.TryGetValue(node, out size))
                    {
                        // Mouse position checking is in node local space
                        var lowerRight = new Rect(size.x - 34, size.y - 34, 30, 30);
                        if (lowerRight.Contains(e.mousePosition)) isDragging = true;
                    }

                    break;
                case EventType.MouseUp:
                    isDragging = false;
                    break;
                case EventType.Repaint:
                    // Add scale cursors
                    if (NodeEditorWindow.current.nodeSizes.TryGetValue(node, out size))
                    {
                        var lowerRight = new Rect(node.position, new Vector2(30, 30));
                        lowerRight.y += size.y - 34;
                        lowerRight.x += size.x - 34;
                        lowerRight = NodeEditorWindow.current.GridToWindowRect(lowerRight);
                        NodeEditorWindow.current.onLateGUI += () => AddMouseRect(lowerRight);
                    }

                    break;
            }
        }

        private static void AddMouseRect(Rect rect)
        {
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeUpLeft);
        }
    }
}