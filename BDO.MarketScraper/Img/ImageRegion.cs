using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace BDO.MarketScraper.Img
{
    public class ImageRegion
    {
        List<ImageRegion> _childRegions = new List<ImageRegion>();

        public ImageRegion()
        {
            ChildRegions = new ReadOnlyCollection<ImageRegion>(_childRegions);
        }

        public ImageRegion(string name, int offsetX, int offsetY, int width, int height)
        {
            ChildRegions = new ReadOnlyCollection<ImageRegion>(_childRegions);
            RegionName = name;
            RelativeOffsetX = offsetX;
            RelativeOffsetY = offsetY;
            RegionWidth = width;
            RegionHeight = height;
        }

        public ImageRegion Parent { get; set; }

        public int RelativeOffsetX { get; set; }
        public int RelativeOffsetY { get; set; }
        
        public string RegionName { get; set; }

        public int RegionWidth { get; set; }
        public int RegionHeight { get; set; }

        public int RegionArea => RegionWidth*RegionHeight;

        public Bitmap FullImage { get; set; }

        public Bitmap GetColorData()
        {
            return GetColorData(FullImage);
        }

        public Bitmap GetColorData(Color[,] fullImage)
        {
            var absX = 0;
            var absY = 0;

            ImageRegion p = Parent;

            while (p != null)
            {
                absX += p.RelativeOffsetX;
                absY += p.RelativeOffsetY;
                p = p.Parent;
            }

            var bmp = new Bitmap(RegionWidth, RegionHeight);
            for (int i = 0; i < RegionWidth; i++)
                for (int j = 0; j < RegionHeight; j++)
                    bmp.SetPixel(i, j, fullImage[i + absX, j + absY]);
            return bmp;
        }

        public Bitmap GetColorData(Bitmap fullImage)
        {
            var absX = RelativeOffsetX;
            var absY = RelativeOffsetY;

            ImageRegion p = Parent;

            while (p != null)
            {
                absX += p.RelativeOffsetX;
                absY += p.RelativeOffsetY;
                p = p.Parent;
            }

            var bmp = new Bitmap(RegionWidth, RegionHeight);
            for (int i = 0; i < RegionWidth; i++)
                for (int j = 0; j < RegionHeight; j++)
                    bmp.SetPixel(i, j, fullImage.GetPixel(i + absX, j + absY));
            return bmp;
        }

        //public Color[,] ColorData { get; private set ; }

        //public void SetColorData(Bitmap bitmap, bool setChildren)
        //{
        //    var cData = new Color[bitmap.Width, bitmap.Height];
        //    for (int i = 0; i < bitmap.Width; i++)
        //    {
        //        for (int j = 0; j < bitmap.Height; j++)
        //        {
        //            cData[i, j] = bitmap.GetPixel(i, j);
        //        }
        //    }
        //    SetColorData(cData, setChildren);
        //}

        //public void SetColorData(Color[,] colorData, bool setChildren)
        //{
        //    if (colorData.GetLength(0) != RegionWidth || colorData.GetLength(1) != RegionHeight)
        //        throw new InvalidOperationException("Invalid color data dimensions.");
        //    ColorData = colorData;
        //    if (!setChildren)
        //        return;

        //    foreach (var c in ChildRegions)
        //    {
        //        var cData = new Color[c.RegionWidth, c.RegionHeight];

        //        for (int x = 0; x < c.RegionWidth; x++)
        //            for (int y = 0; y < c.RegionHeight; y++)
        //                cData[x, y] = colorData[x + c.RelativeOffsetX, y + c.RelativeOffsetY];

        //        c.SetColorData(cData, true);
        //    }
        //}

        public void AddChild(ImageRegion child)
        {
            child.Parent = this;
            _childRegions.Add(child);
            SetImage(FullImage);
        }

        void SetImage(Bitmap image)
        {
            FullImage = image;
            foreach (var c in _childRegions)
                c.SetImage(image);
        }

        public ReadOnlyCollection<ImageRegion> ChildRegions { get; set; } 

        public void Scale(double scale, bool scaleChildren = false)
        {
            RelativeOffsetX = (int)(scale * RelativeOffsetX);
            RelativeOffsetY = (int)(scale * RelativeOffsetY);
            RegionWidth = (int)(scale * RegionWidth);
            RegionHeight = (int)(scale * RegionHeight);

            if (scaleChildren)
                foreach (var c in ChildRegions)
                    c.Scale(scale, true);
        }
    }
}
