using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Avalonia.IconPacks.Utils
{
    public class IconFile
    {
        static public void SaveToICO(Drawing drawing, List<int> sizes, string filename)
        {
            int headerLen = 6 + sizes.Count * 16;
            using (var icoStream = new MemoryStream())
            {
                //write ICONDIR
                icoStream.WriteByte(0);
                icoStream.WriteByte(0);
                icoStream.WriteByte(1);
                icoStream.WriteByte(0);
                icoStream.WriteByte((byte)sizes.Count);
                icoStream.WriteByte(0);
                
                icoStream.Position = headerLen;
                
                for (int i =0; i< sizes.Count; i++)
                {
                    var start = icoStream.Position;
                    SaveDrawing(drawing, sizes[i], icoStream);
                    var end = icoStream.Position;
                    int pngLen = (int)(end - start);
               
                    icoStream.Position = 6 + i * 16;
                    icoStream.WriteByte((byte)sizes[i]);
                    icoStream.WriteByte((byte)sizes[i]);
                    icoStream.WriteByte(0);
                    icoStream.WriteByte(0);
                    icoStream.WriteByte(0);
                    icoStream.WriteByte(0);
                    icoStream.WriteByte(0);
                    icoStream.WriteByte(32);

                    icoStream.WriteByte((byte)(pngLen & 0xff));
                    icoStream.WriteByte((byte)((pngLen >> 8) & 0xff)); 
                    icoStream.WriteByte((byte)((pngLen >> 16) & 0xff));
                    icoStream.WriteByte((byte)(pngLen >> 24));
                   
                    icoStream.WriteByte((byte)(start & 0xff)); 
                    icoStream.WriteByte((byte)((start >> 8) & 0xff));
                    icoStream.WriteByte((byte)((start >> 16) & 0xff));
                    icoStream.WriteByte((byte)(start >> 24));
           
                    icoStream.Position=end;
                }
                using(var icoFile=File.Create(filename))
                {
                    icoFile.Write(icoStream.GetBuffer(), 0, (int)icoStream.Position);
                }
            }
        }
        static public void SaveToICNS(Drawing drawing, List<int> sizes, string filename)
        {
            int headerLen = 8;
            int fileLen = headerLen;
            using (var icoStream = new MemoryStream())
            {
                icoStream.WriteByte(0x69);
                icoStream.WriteByte(0x63);
                icoStream.WriteByte(0x6e);
                icoStream.WriteByte(0x73);
                
                icoStream.Position = headerLen;

                for (int i = 0; i < sizes.Count; i++)
                {
                    var start = icoStream.Position;
                    icoStream.Position += 8;
                    SaveDrawing(drawing, sizes[i], icoStream);
                    var end = icoStream.Position;
                    int pngLen = (int)(end - start);
                    fileLen += pngLen;

                    icoStream.Position = start;
                    string iconType;
                    switch(sizes[i])
                    {
                        case 16: iconType = "icp4"; break;
                        case 32: iconType = "icp5"; break;
                        case 64: iconType = "icp6"; break;
                        case 128: iconType = "ic07"; break;
                        case 256: iconType = "ic08"; break;
                        case 512: iconType = "ic09"; break;
                        default: throw new Exception($"Unsupported icns size {sizes[i]}");
                    }
                    icoStream.Write(ASCIIEncoding.ASCII.GetBytes(iconType));

                    icoStream.WriteByte((byte)(pngLen >> 24));
                    icoStream.WriteByte((byte)((pngLen >> 16) & 0xff));
                    icoStream.WriteByte((byte)((pngLen >> 8) & 0xff));
                    icoStream.WriteByte((byte)(pngLen & 0xff));
                    
                    icoStream.Position = end;
                }
                icoStream.Position = 4;

                icoStream.WriteByte((byte)(fileLen >> 24));
                icoStream.WriteByte((byte)((fileLen >> 16) & 0xff));
                icoStream.WriteByte((byte)((fileLen >> 8) & 0xff));
                icoStream.WriteByte((byte)(fileLen & 0xff));
                
                using (var icoFile = File.Create(filename))
                {
                    icoFile.Write(icoStream.GetBuffer(), 0, fileLen);
                }
            }
        }

        static void SaveDrawing(Drawing drawing,int size, Stream pngStream)
        { 
            var target = new RenderTargetBitmap(new PixelSize(size, size));

            var (drawingsize, transform) = CalculateSizeAndTransform(target.Size, drawing.GetBounds(), Stretch.Uniform);

            using (var ctxi = target.CreateDrawingContext(null))
            using (var ctx = new DrawingContext(ctxi, false))
            {
                using (ctx.PushPreTransform(transform))
                    drawing.Draw(ctx);
            }
            target.Save(pngStream);
        }

        // copied from https://github.com/AvaloniaUI/Avalonia/blob/d40368120e55c294f2ceb741dc086d955b25bb5f/src/Avalonia.Controls/Shapes/Shape.cs
        internal static (Size, Matrix) CalculateSizeAndTransform(Size availableSize, Rect shapeBounds, Stretch Stretch)
        {
            Size shapeSize = new Size(shapeBounds.Right, shapeBounds.Bottom);
            Matrix translate = Matrix.Identity;
            double desiredX = availableSize.Width;
            double desiredY = availableSize.Height;
            double sx = 0.0;
            double sy = 0.0;

            if (Stretch != Stretch.None)
            {
                shapeSize = shapeBounds.Size;
                translate = Matrix.CreateTranslation(-(Vector)shapeBounds.Position);
            }

            if (double.IsInfinity(availableSize.Width))
            {
                desiredX = shapeSize.Width;
            }

            if (double.IsInfinity(availableSize.Height))
            {
                desiredY = shapeSize.Height;
            }

            if (shapeBounds.Width > 0)
            {
                sx = desiredX / shapeSize.Width;
            }

            if (shapeBounds.Height > 0)
            {
                sy = desiredY / shapeSize.Height;
            }

            if (double.IsInfinity(availableSize.Width))
            {
                sx = sy;
            }

            if (double.IsInfinity(availableSize.Height))
            {
                sy = sx;
            }

            switch (Stretch)
            {
                case Stretch.Uniform:
                    sx = sy = Math.Min(sx, sy);
                    break;
                case Stretch.UniformToFill:
                    sx = sy = Math.Max(sx, sy);
                    break;
                case Stretch.Fill:
                    if (double.IsInfinity(availableSize.Width))
                    {
                        sx = 1.0;
                    }

                    if (double.IsInfinity(availableSize.Height))
                    {
                        sy = 1.0;
                    }

                    break;
                default:
                    sx = sy = 1;
                    break;
            }

            var transform = translate * Matrix.CreateScale(sx, sy);
            var size = new Size(shapeSize.Width * sx, shapeSize.Height * sy);
            return (size, transform);
        }
    } 
}
