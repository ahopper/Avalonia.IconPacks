using Avalonia;
using Avalonia.Media;

namespace MetroIconPackExport.utils
{
    public class PathUtils
    {
        public static string Transform(string path,double shiftX, double shiftY,double scaleX, double scaleY)
        {
            var writer = new PathWriter(new Point(shiftX,shiftY), new Point(scaleX, scaleY));
            var parser = new PathMarkupParser(writer);
            parser.Parse(path);
            return writer.GetPath();
        }
    }
}
