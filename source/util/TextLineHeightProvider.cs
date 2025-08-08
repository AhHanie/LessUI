using Verse;

namespace LessUI
{
    public class TextLineHeightProvider
    {
        public virtual float GetLineHeight()
        {
            return Verse.Text.LineHeight;
        }
    }
}
