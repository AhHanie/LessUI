namespace LessUI
{
    public struct Size
    {
        public float width;
        public float height;

        public Size(float width, float height)
        {
            this.width = width;
            this.height = height;
        }

        public static Size zero => new Size(0, 0);
    }
}
