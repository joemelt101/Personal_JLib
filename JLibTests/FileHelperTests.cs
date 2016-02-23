using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.IO;
using JLib.IO.JFile;
using System.Diagnostics;

namespace JLibTests
{
    [TestClass]
    public class FileHelperTests
    {
        [TestMethod]
        public void ResizingImageAppliesProperBorderWhenImageIsTallerThanWide()
        {
            Bitmap bitmap = new Bitmap(100, 200);

            bitmap = (Bitmap)FileHelper.ResizeImage(bitmap, new Size(500, 500), Color.White);

            int midy = bitmap.Height / 2;

            Assert.IsTrue(Color.White.ToArgb() == bitmap.GetPixel(0, midy).ToArgb());
        }

        [TestMethod]
        public void ResizingImageAppliesProperBorderWhenImageIsWiderThanTall()
        {
            Bitmap bitmap = new Bitmap(200, 100);

            bitmap = (Bitmap)FileHelper.ResizeImage(bitmap, new Size(500, 500), Color.White);

            int midx = bitmap.Width / 2;

            Assert.IsTrue(Color.White.ToArgb() == bitmap.GetPixel(midx, 0).ToArgb());
        }

        [TestMethod]
        public void ResizingImagePlacesOldBitmapInTheMiddle()
        {
            Bitmap bitmap = new Bitmap(100, 50);
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.Black);

            bitmap = (Bitmap)FileHelper.ResizeImage(bitmap, new Size(500, 500), Color.White);

            int midx = bitmap.Width / 2;
            int midy = bitmap.Height / 2;

            Color c = bitmap.GetPixel(midx, midy);

            Assert.IsTrue(Color.Black.ToArgb() == bitmap.GetPixel(midx, midy).ToArgb());
        }

        [TestMethod]
        public void ResizingImageLinesUpLeftAndRightPerfectlyWhenImageIsOddlyShapedAndWiderThanTall()
        {
            Bitmap bitmap = new Bitmap(101, 53);
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.Black);

            bitmap = (Bitmap)FileHelper.ResizeImage(bitmap, new Size(500, 500), Color.White);

            int midx = bitmap.Width / 2;
            int midy = bitmap.Height / 2;

            Color c = bitmap.GetPixel(midx, midy);

            //ensure left and right side are not white
            Assert.IsFalse((Color.White.ToArgb() == bitmap.GetPixel(0, midy).ToArgb()) && (Color.White.ToArgb() == bitmap.GetPixel(499, midy).ToArgb()));
        }

        [TestMethod]
        public void ResizingImageLinesUpTopAndBottomPerfectlyWhenImageIsOddleShapedAndTallerThanWide()
        {
            Bitmap bitmap = new Bitmap(53, 101);
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.Black);

            bitmap = (Bitmap)FileHelper.ResizeImage(bitmap, new Size(500, 500), Color.White);

            int midx = bitmap.Width / 2;
            int midy = bitmap.Height / 2;

            Color c = bitmap.GetPixel(midx, midy);

            //ensure left and right side are not white
            Assert.IsFalse((Color.White.ToArgb() == bitmap.GetPixel(midx, 0).ToArgb()) && (Color.White.ToArgb() == bitmap.GetPixel(midx, 499).ToArgb()));
        }

        [TestMethod]
        public void ResizingImageWithoutMaintainingAspectRatioLinesUpEdgesPerfectly()
        {
            int diffThreshhold = 3000000;

            Bitmap bitmap = new Bitmap(53, 101);
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.Black);

            bitmap = (Bitmap)FileHelper.ResizeImage(bitmap, new Size(10000, 10000), Color.White, false);

            //check top left and bottom right to ensure they are black
            int topLeftDiff = Math.Abs(bitmap.GetPixel(0, 0).ToArgb() - Color.Black.ToArgb());
            int bottomRightDiff = Math.Abs(bitmap.GetPixel(bitmap.Width - 1, bitmap.Height - 1).ToArgb() - Color.Black.ToArgb());
            Assert.IsTrue(topLeftDiff < diffThreshhold && bottomRightDiff < diffThreshhold);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResizingImageReportsErrorWhenNullPassedInForImage()
        {
            FileHelper.ResizeImage(null, new Size(1, 1), Color.White, false);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ResizingImageReportsInvalidArgumentErrorWhenPassedAnEmptySize()
        {
            FileHelper.ResizeImage(new Bitmap(10, 10), new Size(0, 0), Color.White, false);
        }
    }
}
