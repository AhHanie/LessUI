using Verse;

namespace LessUI
{
    public class TextWidthProvider
    {
        public virtual float GetTextWidth(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0f;

            var originalWordWrap = Text.WordWrap;
            try
            {
                Text.WordWrap = false;
                var size = Text.CalcSize(text);
                return size.x;
            }
            finally
            {
                Text.WordWrap = originalWordWrap;
            }
        }
    }
}
