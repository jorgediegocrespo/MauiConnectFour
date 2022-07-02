using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace ConnectFour.Controls;

public class CellBoard : SKCanvasView
{
    public static readonly BindableProperty BoardColorProperty = BindableProperty.Create(nameof(BoardColor), typeof(Color), typeof(CellBoard), Colors.Blue, propertyChanged: OnPropertyChanged);
    public static readonly BindableProperty ColumnProperty = BindableProperty.Create(nameof(Column), typeof(int), typeof(CellBoard));
    public static readonly BindableProperty RowProperty = BindableProperty.Create(nameof(Row), typeof(int), typeof(CellBoard));

    private SKPaint eraser;
    private SKCanvas cv;
    private SKBitmap bm;
    private float currentWith;
    private float currentHeight;

    public CellBoard() : base()
    {
        eraser = new SKPaint();
        eraser.BlendMode = SKBlendMode.Clear;
        eraser.IsAntialias = true;
    }

    public Color BoardColor
    {
        get { return (Color)GetValue(BoardColorProperty); }
        set { SetValue(BoardColorProperty, value); }
    }

    public int Column
    {
        get { return (int)GetValue(ColumnProperty); }
        set { SetValue(ColumnProperty, value); }
    }

    public int Row
    {
        get { return (int)GetValue(RowProperty); }
        set { SetValue(RowProperty, value); }
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        SKImageInfo info = e.Info;
        if (info.Width != currentWith || info.Height != currentHeight)
        {
            currentHeight = info.Height;
            currentWith = info.Width;
            bm = new SKBitmap(info.Width, info.Height);
            cv = new SKCanvas(bm);
        }

        SKSurface surface = e.Surface;
        SKCanvas canvas = surface.Canvas;
        
        bm.Erase(SKColors.Transparent);
        cv.DrawColor(BoardColor.ToSKColor());

        int spacing = 20;
        int width = info.Width - spacing;
        int height = info.Height - spacing;
        int radius = width > height ? height / 2 : width / 2;
        cv.DrawCircle(info.Width / 2, info.Height / 2, radius, eraser);
        canvas.DrawBitmap(bm, 0, 0);

        base.OnPaintSurface(e);
    }

    private static void OnPropertyChanged(BindableObject bindable, object oldVal, object newVal)
    {
        var circleProgress = bindable as CellBoard;
        circleProgress?.InvalidateSurface();
    }
}
