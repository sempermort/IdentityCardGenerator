class RoundedHexagonDrawable : IDrawable
{
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.StrokeColor = Colors.LightGreen;
        canvas.StrokeSize = 5;

        var width = dirtyRect.Width;
        var height = dirtyRect.Height;

        var path = new PathF();

        // Start top mid
        path.MoveTo(width / 2, 0);

        // Define cubic Bezier or rounded corners manually
        path.LineTo(width, height * 0.25f);
        path.LineTo(width, height * 0.75f);
        path.LineTo(width / 2, height);
        path.LineTo(0, height * 0.75f);
        path.LineTo(0, height * 0.25f);
        path.Close();

        canvas.DrawPath(path);
    }
}
