/*---------------------------------------------------------------------------
*	Namespace   : Cgp.Components
*   Assembly    : Components.dll
*   
*   Class       : ImageResizeUtility
*  
*	Description : Utility class that provides static methods for resizing of images.
*
*   Requirement key :  R_2331  
*
*	Copyright (C) 2013 CONTAL OK s.r.o. All Rights Reserved.
-----------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Drawing.Drawing2D;

namespace Cgp.Components
{
    /// <summary>
    /// Provides static methods for resizing of images.
    /// </summary>
    ///
    /// <devdoc>Req_key : R_2331 </devdoc>
    public static  class ImageResizeUtility
    {
        #region Fields
        /// <summary>
        /// Modes available for the resize method of images.
        /// </summary> 
        public enum ResizeMode
        {
            /// <summary>
            /// Resize images to the dimension nearest to the target resolution, keeping asspect ratio.
            /// Images can become smaller when using this option.
            /// </summary>
            Normal,
            /// <summary>
            /// Resize images to the exact dimensions of the target resolution, keeping asspect ratio and cropping parts that can't
            /// fit in the picture.
            /// </summary>
            Crop,
            /// <summary>
            /// Resize images to the exact dimensions of the target resolution, keeping asspect ratio and filling up the image
            /// with bars when some parts remain empty.
            /// </summary>
            Fill
        }
        #endregion
        #region Scaling algorithms
        /// <summary>
        /// Resizes Image to the dimension nearest to the Target dimension, keeping asspect ratio. 
        /// Target Image can become smaller when using this algorithm. 
        /// </summary>
        /// <param name="imgToResize">Source Image to resize.</param>
        /// <param name="width">Target width.</param>
        /// <param name="height">Target hight.</param>
        /// <param name="orientation">True, if Target image to match the Source image orientation.</param>
        /// <returns>Size of Target thumbnail.</returns>
        public static Size ResizeToTargetNormal(Image imgToResize, int width, int height, bool orientation)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;
            int targetWidth = width;
            int targetHeight = height;
           
            // Supplied image is landscape, while the target dimension is portrait OR
            // supplied image is in portrait, while the target dimension is in landscape.
            // Switch target dimension to match the source image orientation.
            if (orientation && ((sourceWidth > sourceHeight && targetWidth < targetHeight) || (sourceWidth < sourceHeight && targetWidth > targetHeight)) )
            {
                targetWidth = height;
                targetHeight = width;
            }

            float ratio = 0;
            float ratioWidth = ((float)targetWidth / (float)sourceWidth);
            float ratioHeight = ((float)targetHeight / (float)sourceHeight);

            if (ratioHeight < ratioWidth)
                ratio = ratioHeight;
            else
                ratio = ratioWidth;
      
            // Return thumbnail size.
            return new Size((int)(sourceWidth * ratio), (int)(sourceHeight * ratio));
        }
        
        /// <summary>
        /// Resize Image to the exact dimensions of the target resolution, keeping asspect ratio and cropping parts that can't
        /// fit in the picture.
        /// </summary>
        /// <param name="imgToResize">Source Image to resize.</param>
        /// <param name="width">Target width.</param>
        /// <param name="height">Target heigth.</param>
        /// <param name="orientation">True, if Target image to match the Source image orientation.</param>
        /// <returns>Coordinates of Target thumbnail.</returns>
        public static Rectangle ResizeToTargetCrop(Image imgToResize, int width, int height, bool orientation)
        {

             int sourceWidth = imgToResize.Width;
             int sourceHeight = imgToResize.Height;

             int targetWidth = width;
             int targetHeight = height;
                        
             if (orientation && ((sourceWidth > sourceHeight && targetWidth < targetHeight) || (sourceWidth < sourceHeight && targetWidth > targetHeight)) )
             {
                 targetWidth = height;
                 targetHeight = width;
             }

             float ratio = 0;
             float ratioWidth = ((float)targetWidth / (float)sourceWidth);
             float ratioHeight = ((float)targetHeight / (float)sourceHeight);

             if (ratioHeight > ratioWidth)
                 ratio = ratioHeight;
             else
                 ratio = ratioWidth;

             int destWidth = (int)(sourceWidth * ratio);
             int destHeight = (int)(sourceHeight * ratio);
             int startX = 0;
             int startY = 0;
             // Crop dimensions if necessary   
             if (destWidth > targetWidth)
                 startX = 0 - ((destWidth - targetWidth) / 2);
             if (destHeight > targetHeight)
                 startY = 0 - ((destHeight - targetHeight) / 2);
             // Return thumbnail coordinates.
             return new Rectangle(startX,startY,(int)(destWidth), (int)(destHeight));
        }
        /// <summary>
        /// Resize images to the exact dimensions of the target resolution, keeping asspect ratio and filling up the image
        /// with bars when some parts remain empty.
        /// </summary>
        /// <param name="imgToResize">Source Image to resize.</param>
        /// <param name="width">Target width.</param>
        /// <param name="height">Target height.</param>
        /// <param name="orientation">True, if Target image to match the Source image orientation.</param>
        /// <returns> Coordinates of Target thumbnail.</returns>
        public static Rectangle ResizeToTargetFill(Image imgToResize, int width, int height, bool orientation)
        {

            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            int targetWidth = width;
            int targetHeight = height;

            if (orientation && ((sourceWidth > sourceHeight && targetWidth < targetHeight) || (sourceWidth < sourceHeight && targetWidth > targetHeight)) )
            {
                targetWidth = height;
                targetHeight = width;
            }

            float ratio = 0;
            float ratioWidth = ((float)targetWidth / (float)sourceWidth);
            float ratioHeight = ((float)targetHeight / (float)sourceHeight);

            if (ratioHeight < ratioWidth)
                ratio = ratioHeight;
            else
                ratio = ratioWidth;

            int destWidth = (int)(sourceWidth * ratio);
            int destHeight = (int)(sourceHeight * ratio);
            int startX = 0;
            int startY = 0;

            if (destWidth < targetWidth)
                startX = 0 + ((targetWidth - destWidth) / 2);
            if (destHeight < targetHeight)
                startY = 0 + ((targetHeight - destHeight) / 2);
            // Return thumbnail coordinates.
            return new Rectangle(startX, startY, (int)(destWidth), (int)(destHeight));
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Gets the resized image.
        /// </summary>
        /// <param name="imgToResize">Source Image to resize.</param>
        /// <param name="thumbnailWidth">Target thumbnail width.</param>
        /// <param name="thumbnailHight">Target thumbnail hight.</param>
        /// <param name="orientation">True, if Target image to match the Source image orientation.</param>
        /// <returns>The thumbnail Image.</returns>
        /// <exception cref="System.ArgumentException">Thrown when imgToResize is null.</exception>
        public static Image GetResizedImage(Bitmap imgToResize, int thumbnailWidth, int thumbnailHight, ResizeMode resizeMode, bool orientation)
        {
            if (imgToResize == null)
                throw new ArgumentException("Parameter cannot be null", "imgToResize");

            Size newSize;
            Rectangle newCoordinates =  new Rectangle(0,0,0,0);
            switch (resizeMode)
            {
                case ResizeMode.Normal:
                    newSize = ResizeToTargetNormal(imgToResize, thumbnailWidth, thumbnailHight, orientation);
                    newCoordinates.X = 0;
                    newCoordinates.Y = 0;
                    newCoordinates.Width = newSize.Width;
                    newCoordinates.Height = newSize.Height;
                    break;
                case ResizeMode.Crop:
                    newCoordinates = ResizeToTargetCrop(imgToResize, thumbnailWidth, thumbnailHight, orientation);
                    break;
                case ResizeMode.Fill:
                    newCoordinates = ResizeToTargetFill(imgToResize, thumbnailWidth, thumbnailHight, orientation);
                    break;
                default:
                    newSize = ResizeToTargetNormal(imgToResize, thumbnailWidth, thumbnailHight, orientation);
                    newCoordinates.X = 0;
                    newCoordinates.Y = 0;
                    newCoordinates.Width = newSize.Width;
                    newCoordinates.Height = newSize.Height;
                    break;
            }

            // The logic is to create a blank image and draw the source image onto it
            Bitmap newImage = new Bitmap(newCoordinates.Width, newCoordinates.Height);
            using (Graphics gr = Graphics.FromImage((System.Drawing.Image)newImage))
            {
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.CompositingQuality = CompositingQuality.HighQuality;

                gr.DrawImage(imgToResize, newCoordinates.X, newCoordinates.Y, newCoordinates.Width, newCoordinates.Height);
            }
            return (Image)newImage;
        }
               
        /// <summary>
        /// Saves image as JPG file with highest quality.
        /// </summary>
        /// <param name="imgToSave">Image to save.</param>
        /// <param name="path">The path and name of the file to save.</param>
        public static void SaveAsJPG(Image imgToSave, string path)
        {
            int quality = 90; //90 is the magic setting - It has excellent quality.
            //Setting the quality of the picture
            EncoderParameter qualityParam =
                new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            // Get an ImageCodecInfo object that represents the JPG codec.
            ImageCodecInfo imageCodec = GetEncoderInfo("image/jpeg");

            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            myEncoderParameters.Param[0] = qualityParam;
            // Save the bitmap as a JPG file with given quality level
            imgToSave.Save(path, imageCodec, myEncoderParameters);
        }
        #endregion
        #region Private Functions
        /// <summary>
        /// Gets the imageCode info on the basis of mimeType.
        /// </summary>
        /// <param name="mimeType">string data type</param>
        /// <returns>ImageCodecInfo data type</returns>
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            for (int i = 0; i < encoders.Length; i++)
            {
                if (encoders[i].MimeType == mimeType)
                {
                    return encoders[i];
                }
            }
            return null;
        }
        #endregion
    }
}
