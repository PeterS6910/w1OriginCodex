using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Definitions;
using Contal.Cgp.NCAS.WpfGraphicsControl;

namespace Cgp.NCAS.WpfGraphicsControl
{
    public class GraphicsDoorEnvironment : GraphicsSymbol, ILiveObject
    {
        private const string LEFT = "Left";
        private const string RIGHT = "Right";
        private readonly Dictionary<string, SymbolParemeter> _symbols;
        private string _rotatory = "Right";
        private bool _rotatoryToLeft;
        private readonly Storyboard _storyboard = new Storyboard();
        private readonly Duration _durationSlow = new Duration(new TimeSpan(0, 0, 0, 0, 500));
        private readonly Duration _durationFast = new Duration(new TimeSpan(0, 0, 0, 0, 200));
        private byte _state;
        private bool _isEnable = true;

        public Guid guid { get; set; }

        public bool RotatoryToLeft 
        {
            get { return _rotatoryToLeft; }
            set
            {
                _rotatoryToLeft = value;
                _rotatory = value ? LEFT : RIGHT;
            }
        }

        public GraphicsDoorEnvironment(Canvas canvas, Dictionary<string, SymbolParemeter> symbols, bool rotatoryToLeft)
            : base(canvas)
        {
            _symbols = symbols;
            RotatoryToLeft = rotatoryToLeft;

            SymbolParemeter symbolParemeter;
            _symbols.TryGetValue("DoorEnviroment" + _rotatory + State.Unknown.ToString().ToLower(), 
                out symbolParemeter);

            Source = symbolParemeter != null 
                ? symbolParemeter.ImageSource 
                : GraphicsScene.NoSource;

            //configure animation
            var animation = new DoubleAnimation
            {
                From = 0.2,
                To = 1.0,
                AutoReverse = true,
                Duration = _durationSlow,
                RepeatBehavior = RepeatBehavior.Forever
            };

            Storyboard.SetTargetProperty(animation, new PropertyPath(OpacityProperty));
            _storyboard.Children.Add(animation);
        }

        public void StartAnimation(Duration duration)
        {
            _storyboard.Stop(this);
            _storyboard.Children[0].Duration = duration;
            _storyboard.Begin(this, true);
        }

        public void StopAnimation()
        {
            _storyboard.Stop(this);
        }

        public bool Enable
        {
            get { return _isEnable; }
            set
            {
                _isEnable = value;
                //string key = "DoorEnviroment" + _rotatory + State.Unknown.ToString().ToLower();

                //if (_symbols.ContainsKey(key))
                //    Source = _symbols[key].ImageSource;

                StopAnimation();
                _state = (byte)DoorEnvironmentState.Unknown;
            }
        }

        public void ChangeState(byte state)
        {
            var doorEnvironmentState = (DoorEnvironmentState) state;

            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {  
                _state = state;
                string key = "DoorEnviroment" + _rotatory + doorEnvironmentState.ToString().ToLower();

                if (doorEnvironmentState == DoorEnvironmentState.Sabotage)
                    key = "DoorEnviroment" + _rotatory + DoorEnvironmentState.Intrusion.ToString().ToLower();

                if (!_symbols.ContainsKey(key))
                    return;

                Source = _symbols[key].ImageSource;

                switch (doorEnvironmentState)
                {
                    case DoorEnvironmentState.Sabotage:
                    case DoorEnvironmentState.Intrusion:
                        StartAnimation(_durationFast);
                        break;

                    case DoorEnvironmentState.AjarPrewarning:
                    case DoorEnvironmentState.Ajar:
                        StartAnimation(_durationSlow);
                        break;

                    default:
                        StopAnimation();
                        break;
                }
            }));
        }

        public void SetObjectGuid(Guid id)
        {
            guid = id;
        }

        public Guid GetObjectGuid()
        {
            return guid;
        }

        #region ImyLiveObject Members


        public void ShowInfo()
        {
        }

        #endregion

        #region ImyLiveObject Members


        public object GetState()
        {
            return ((DoorEnvironmentState) _state).ToString();
        }

        #endregion

        #region ImyLiveObject Members


        public string GetObjectTypeName()
        {
            return "Door Environment";
        }

        public ObjectType GetObjectType()
        {
            return ObjectType.DoorEnvironment;
        }

        #endregion

        #region ImyLiveObject Members


        public bool isEnable()
        {
            return Enable;
        }

        void ILiveObject.Enable(bool enable)
        {
            Enable = enable;
        }

        public void SetLabel(Text label)
        {
            Label = label;
        }

        public Text GetLabel()
        {
            return Label;
        }

        public override object Serialize()
        {
            return new SerializableDoorEnvironment(this);
        }

        #endregion
    }

    [Serializable()]
    public class SerializableDoorEnvironment : SerializableObject, IGraphicsDeserializeObject
    {
        public Guid guid { get; set; }
        public bool RotatoryToLeft { get; set; }
        public SerializableText label { get; set; }

        public SerializableDoorEnvironment(GraphicsDoorEnvironment obj)
        {
            Serialize(obj as UIElement);
            guid = obj.guid;
            RotatoryToLeft = obj.RotatoryToLeft;
            Categories = (obj as ILiveObject).GetCategories().ToList();

            if (obj.GetLabel() != null)
                label = new SerializableText(obj.GetLabel());
        }

        public GraphicsDoorEnvironment GetGraphicsDoorEnvironment(Canvas canvas, Dictionary<string, SymbolParemeter> symbols)
        {
            GraphicsDoorEnvironment obj = new GraphicsDoorEnvironment(canvas, symbols, RotatoryToLeft);
            Deserialize(obj as UIElement);
            obj.guid = guid;
            (obj as ILiveObject).SetCategories(Categories);
            obj.LoadDefaulParameters();
            canvas.Children.Add(obj);

            if (label != null)
            {
                Text text = label.GetText(canvas);
                obj.SetLabel(text);
                text.LiveObject = obj;
            }

            return obj;
        }

        public IGraphicsObject Deserialize(Canvas canvas, Dictionary<string, SymbolParemeter> symbols)
        {
            return GetGraphicsDoorEnvironment(canvas, symbols);
        }
    }
}
