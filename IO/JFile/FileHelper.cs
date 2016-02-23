using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace JLib.IO.JFile
{
    public static class FileHelper
    {
        public static IEnumerable<Image> ResizeImages(IEnumerable<Image> imageList, Size newSize, Color marginColor, bool preserveAspectRatio = true)
        {
            List<Image> returnList = new List<Image>();

            foreach (var image in imageList)
            {
                var img = ResizeImage(image, newSize, Color.White);
                returnList.Add(img);
            }

            return returnList;
        }

        public static void ResizeImagesInFolder(string sourceDirectory, string outputDirectory, Size newSize, Color marginColor, bool preserveAspectRatio = true)
        {
            if (Directory.Exists(sourceDirectory) == false)
            {
                throw new ArgumentException("Invalid source directory supplied.");
            }

            if (Directory.Exists(outputDirectory) == false)
            {
                //create it
                Directory.CreateDirectory(outputDirectory);
            }

            var imageFileLocations = Directory.EnumerateFiles(sourceDirectory);

            foreach (var imageFileLocation in imageFileLocations)
            {
                Image originalImage = Image.FromFile(imageFileLocation);
                Image resizedImage = ResizeImage(originalImage, newSize, marginColor);

                //save each image
                string filename = Path.GetFileNameWithoutExtension(imageFileLocation) + ".jpg";

                if (outputDirectory.EndsWith("\\") == false)
                {
                    outputDirectory += "\\";
                }

                resizedImage.Save(outputDirectory + filename, ImageFormat.Jpeg);
            }
        }

        public static Image ResizeImage(Image image, Size newSize, Color marginColor, bool preserveAspectRatio = true)
        {
            //validate inputs
            if (image == null)
            {
                throw new ArgumentNullException("The image parameter cannot be null.");
            }

            if (newSize.Width <= 0 || newSize.Height <= 0)
            {
                throw new ArgumentException("You must have a non-zero size.");
            }

            //figure out x and y position
            int x = 0, y = 0;

            //size of the nested image
            int newWidth = newSize.Width;
            int newHeight = newSize.Height;

            if (image.Size.Width < newSize.Width)
            {
                //expanding the image
                //this could cause problems on the edges if the aspect ratio is not maintained
                //to fix, increase newWidth and newHeight by a small percentage to cover the gap
                //determined via trial and error. See Test Name: ResizingImageWithoutMaintainingAspectRatioLinesUpEdgesPerfectly
                newWidth = (int)(newWidth * 1.03f);
                newHeight = (int)(newHeight * 1.03f);
            }

            if (preserveAspectRatio)
            {
                int originalWidth = image.Width;
                int originalHeight = image.Height;

                float percentWidth = (float)newSize.Width / (float)originalWidth;
                float percentHeight = (float)newSize.Height / (float)originalHeight;

                if (percentHeight < percentWidth)
                {
                    //the height will hit the boundary before the width
                    newHeight = newSize.Height;
                    newWidth = (int)(originalWidth * percentHeight);

                    //center the image horizontally
                    x = Math.Abs(newSize.Width - newWidth) / 2;
                }
                else if (percentHeight > percentWidth)
                {
                    //the width will hit the boundary before the height
                    newHeight = (int)(originalHeight * percentWidth);
                    newWidth = newSize.Width;

                    //center the image vertically
                    y = Math.Abs(newSize.Height - newHeight) / 2;
                }
            }

            //Save image according to size specifications
            Bitmap canvas = new Bitmap(newSize.Width, newSize.Height);

            Graphics g = Graphics.FromImage(canvas);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            g.Clear(marginColor);
            g.DrawImage(image, new Rectangle(x, y, newWidth, newHeight));
            g.Dispose();

            return canvas;
        }
    }
}
