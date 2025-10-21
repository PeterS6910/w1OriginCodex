using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cgp.NCAS.WpfGraphicsControl;
using System.Runtime.Serialization;
using System.Reflection;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.WpfGraphicsControl
{
    public class CanvasSerialization
    {
        private class Binder : SerializationBinder
        {
            private interface ITypeConverter
            {
                void ConvertType(
                    ref string assemblyName,
                    ref string typeName);
            }

            private class CgpGlobalasAssemblyConverter : ITypeConverter
            {
                public void ConvertType(
                    ref string assemblyName,
                    ref string typeName)
                {
                    if (assemblyName.Contains("Cgp.Globals.PC35"))
                    {
                        assemblyName = Assembly.GetAssembly(typeof(SymbolType)).FullName;
                    }
                }
            }

            private readonly ITypeConverter[] _typeConverters;

            public Binder()
            {
                _typeConverters = new ITypeConverter[] { new CgpGlobalasAssemblyConverter() };
            }


            public override Type BindToType(
                string assemblyName,
                string typeName)
            {
                foreach (var typeConverter in _typeConverters)
                {
                    typeConverter.ConvertType(
                        ref assemblyName,
                        ref typeName);
                }

                return Type.GetType(
                    String.Format("{0}, {1}",
                    typeName,
                    assemblyName));
            }
        }

        public CanvasSettings CanvasSettings = new CanvasSettings();
        public Dictionary<Guid, Layer> Layers { get; set; }
        public Dictionary<Category, bool> Categories { get; set; }

        public IEnumerable<Category> GetEnableCategories()
        {
            if (Categories == null)
                return null;

            var enableCategories =
                Categories
                    .Where(kvpCategory => kvpCategory.Value)
                    .Select(kvpCategory => kvpCategory.Key);

            return enableCategories;
        }

        public byte[] SaveToArray(Canvas canvas)
        {
            if (canvas == null)
                return null;

            byte[] data;
            var stream = new MemoryStream();
            var bFormatter = new BinaryFormatter();

            try
            {
                //save canvas settings
                bFormatter.Serialize(stream, CanvasSettings);

                //serialize Layers
                if (Layers != null)
                    bFormatter.Serialize(stream, Layers);

                //save Categories settings
                if (Categories != null)
                    bFormatter.Serialize(stream, Categories);

                foreach (UIElement shape in canvas.Children)
                {
                    try
                    {
                        if (shape == null)
                            continue;

                        var graphicsObject = shape as IGraphicsObject;

                        if (graphicsObject == null)
                            continue;

                        var serializeObject = graphicsObject.Serialize();

                        if (serializeObject != null)
                            bFormatter.Serialize(stream, graphicsObject.Serialize());
                    }
                    catch
                    {
                    }
                }

                data = stream.ToArray();
            }
            catch
            {
                return null;
            }
            finally
            {
                try
                {
                    stream.Close();
                }
                catch
                {
                }   
            }

            return data;
        }

        public bool LoadCanvasSettings(byte[] data)
        {
            if (data == null)
                return false;

            MemoryStream stream = null;
            object objectToSerialize;

            try
            {
                stream = new MemoryStream(data);

                while (stream.Position != stream.Length)
                {
                    var bFormatter = new BinaryFormatter();
                    objectToSerialize = bFormatter.Deserialize(stream);
                    CanvasSettings = objectToSerialize as CanvasSettings;

                    if (CanvasSettings != null)
                        break;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                try
                {
                    stream.Close();
                }
                catch
                {
                }
            }

            return true;
        }

        public bool LoadFromArray(Canvas canvas, byte[] data, Dictionary<string, SymbolParemeter> symbols)
        {
            if (data == null || canvas == null)
                return false;

            if (Layers != null)
                Layers.Clear();

            MemoryStream stream = null;

            try
            {
                stream = new MemoryStream(data);

                while (stream.Position != stream.Length)
                {
                    try
                    {
                        var bFormatter = new BinaryFormatter();
                        bFormatter.Binder = new Binder();
                        var objectToSerialize = bFormatter.Deserialize(stream);

                        var layersDictionary = objectToSerialize as Dictionary<Guid, Layer>;

                        if (layersDictionary != null)
                        {
                            Layers = layersDictionary;
                            continue;
                        }

                        var categoriesDisctionary = objectToSerialize as Dictionary<Category, bool>;

                        if (categoriesDisctionary != null)
                        {
                            Categories = categoriesDisctionary;
                            continue;
                        }

                        var deserializeGraphicsObject = objectToSerialize as IGraphicsDeserializeObject;

                        if (deserializeGraphicsObject != null)
                            deserializeGraphicsObject.Deserialize(canvas, symbols);
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                try
                {
                    stream.Close();
                }
                catch
                {
                }  
            }

            return true;
        }

        public MemoryStream SaveScreen(Canvas background, Canvas canvas)
        {
            try
            {
                var size = new Size(canvas.ActualWidth*canvas.LayoutTransform.Value.M11,
                    canvas.ActualHeight*canvas.LayoutTransform.Value.M11);
                var rtb = new RenderTargetBitmap((int) size.Width, (int) size.Height, 96d, 96d,
                    PixelFormats.Default);

                if (background.Children.Count > 0)
                    rtb.Render(background.Children[0]);

                rtb.Render(canvas);

                BitmapEncoder pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

                var ms = new MemoryStream();
                pngEncoder.Save(ms);
                return ms;
            }
            catch
            {
                return null;
            }
        }
    }

    [Serializable()]
    public class Layer
    {
        public Guid Id { get; private set; }
        public string Name { get; set; }
        public int ZIndex { get; set; }
        public bool Enabled { get; set; }

        public Layer()
        {
            Id = Guid.NewGuid();
            ZIndex = 0;
            Enabled = true;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    [Serializable()]
    public class SerializableColor
    {
        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public SerializableColor(Color color)
        {
            A = color.A;
            R = color.R;
            G = color.G;
            B = color.B;
        }

        public SerializableColor(System.Drawing.Color color)
        {
            A = color.A;
            R = color.R;
            G = color.G;
            B = color.B;
        }

        public Color GetColor()
        {
            return Color.FromArgb(A, R, G, B);
        }
    }

    [Serializable]
    public class CanvasSettings
    {
        public SymbolSize DefaultSymbolSize { get; set; }
        public int ImplicityScaleOfInsertedSymbols { get; set; }
        public double ModelLength { get; set; }
        public Guid? UseTemplateId { get; set; }
        public double defaultLineWidth { get; set; }
        public SerializableColor defaultLineColor { get; set; }
        public SerializableColor defaultBackgroundColor { get; set; }

        public CanvasSettings()
        {
            defaultLineWidth = 1.0;
            defaultLineColor = new SerializableColor(Colors.Black);
            defaultBackgroundColor = new SerializableColor(Color.FromArgb(0,0,0,0));
            DefaultSymbolSize = SymbolSize.Variable;
            ImplicityScaleOfInsertedSymbols = 50;
        }
    }

    public enum SymbolSize
    {
        Variable, Fixed
    }
}
