using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace Plugins.StickyNode
{
    [CreateNodeMenu("Sticky Note", 5000)]
    public class StickyNoteNode : Node
    {
        [Title("Sticky Node Data")]
        [TextArea(0, 8)]
        public string text;

        public bool squared;
        public int width = 200;
        public int height = 200;

        private int lines
        {
            get
            {
                if (string.IsNullOrEmpty(text) == false)
                    return text.Split('\n').Length + 1;
                return 1;
            }
        }

        private int longestLine
        {
            get
            {
                if (string.IsNullOrEmpty(text) == false)
                    return text.Split('\n').Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur).Length;
                return 5;
            }
        }

        public int fontSize => Mathf.Min(height / lines, width / (longestLine / 2));
    }
}