using UnityEngine;
using UnityEngine.UI;

namespace Erwandi.Gamepangin.Utilities
{
    /// <summary>
    /// Extension methods for ScrollRect type
    /// </summary>
    public static class ScrollRectExtensions
    {
        /// <summary>
        /// Scrolls a scroll rect to the top.
        /// </summary>
        /// <param name="scrollRect"></param>
        public static void ScrollToTop(this ScrollRect scrollRect)
        {
            scrollRect.normalizedPosition = new Vector2(0, 1);
        }
        
        /// <summary>
        /// Scrolls a scroll rect to the bottom.
        /// </summary>
        /// <param name="scrollRect"></param>
        public static void ScrollToBottom(this ScrollRect scrollRect)
        {
            scrollRect.normalizedPosition = new Vector2(0, 0);
        }
    }
}