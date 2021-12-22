using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using System;
using System.Text;


namespace MetroIconPackExport.utils
{
    public class PathWriter : IGeometryContext
    {
        StringBuilder sb = new();
        Point _translate;
        Point _scale;
        public PathWriter(Point translate, Point scale)
        {
            _translate = translate;
            _scale = scale;
        }
        private string FormatPoint(Point point)
        {
            point = new Point((point.X + _translate.X) * _scale.X, (point.Y + _translate.Y) * _scale.Y);
            return $"{SignificantDigits.ToString2(point.X, 4)},{SignificantDigits.ToString2(point.Y, 4)}";
        }
        private string FormatSize(Size size)
        {
            size = new Size(size.Width * _scale.X, size.Height * _scale.Y);
            return $"{SignificantDigits.ToString2(size.Width, 4)},{SignificantDigits.ToString2(size.Height, 4)}";
        }
        public void ArcTo(Point point, Size size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection)
        {
            if (_scale.Y >= 0)
            {
                sb.Append($"A{FormatSize(size)} {rotationAngle} {(isLargeArc ? 1 : 0)} {(sweepDirection == SweepDirection.Clockwise ? 1 : 0)} {FormatPoint(point)} ");
            }
            else
            {
                sb.Append($"A{FormatSize(size)} {rotationAngle} {(isLargeArc ? 1 : 0)} {(sweepDirection == SweepDirection.Clockwise ? 0 : 1)} {FormatPoint(point)} ");

            }
        }

        public void BeginFigure(Point startPoint, bool isFilled = true)
        {
            sb.Append($"M{FormatPoint(startPoint)} ");
        }

        public void CubicBezierTo(Point point1, Point point2, Point point3)
        {
            sb.Append($"C{FormatPoint(point1)} {FormatPoint(point2)} {FormatPoint(point3)} ");
        }

        public void Dispose()
        {
        }

        public void EndFigure(bool isClosed)
        {
            sb.Append($"{(isClosed ? "z" : "")} ");
        }

        public void LineTo(Point point)
        {
            sb.Append($"L{FormatPoint(point)} ");
        }

        public void QuadraticBezierTo(Point control, Point endPoint)
        {
            sb.Append($"Q{FormatPoint(control)} {FormatPoint(endPoint)} ");
        }

        public void SetFillRule(FillRule fillRule)
        {
            sb.Append($"F{(fillRule == FillRule.EvenOdd ? 0 : 1)} ");
        }
        public string GetPath()
        {
            return sb.ToString().Trim();
        }
    }
    
}