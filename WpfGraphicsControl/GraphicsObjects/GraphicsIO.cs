using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.WpfGraphicsControl;
using Contal.Cgp.Globals;

namespace Cgp.NCAS.WpfGraphicsControl
{
    public enum IOType
    {
        Input, Output
    }

    public class GraphicsIO : GraphicsSymbol, ILiveObject
    {
        private readonly Dictionary<string, SymbolParemeter> _symbols;
        private readonly Storyboard _storyboard = new Storyboard();
        //private Duration _durationSlow = new Duration(new TimeSpan(0, 0, 0, 0, 500));
        private readonly Duration _durationFast = new Duration(new TimeSpan(0, 0, 0, 0, 200));
        private byte _state;
        private bool _isEnable = true;

        public Guid IdIO { get; set; }
        public IOType IOType { get; set; }
        public SymbolType SymbolType { get; private set; }

        public GraphicsIO(Canvas canvas, Dictionary<string, SymbolParemeter> symbols, SymbolType type)
            : base(canvas)
        {
            _symbols = symbols;
            SymbolType = type;

            //configure animation
            var duration = new TimeSpan(0, 0, 1);
            var animation = new DoubleAnimation
            {
                From = 0.2,
                To = 1.0,
                AutoReverse = true,
                Duration = new Duration(duration),
                RepeatBehavior = RepeatBehavior.Forever
            };

            Storyboard.SetTargetProperty(animation, new PropertyPath(OpacityProperty));
            _storyboard.Children.Add(animation);

            string key = SymbolType + State.Off.ToString().ToLower();

            if (_symbols.ContainsKey(key))
                Source = _symbols[key].ImageSource;

            key = SymbolType + State.Unknown.ToString().ToLower();

            if (_symbols.ContainsKey(key))
                Source = _symbols[key].ImageSource;

            if (Source == null)
                Source = GraphicsScene.NoSource;
        }

        private void ChangeIOSource(string keyA, string keyB)
        {
            if (string.IsNullOrEmpty(keyA) || string.IsNullOrEmpty(keyB))
                return;

            if (_symbols.ContainsKey(SymbolType + keyA))
                Source = _symbols[SymbolType + keyA].ImageSource;
            else if (_symbols.ContainsKey(SymbolType + keyB))
                Source = _symbols[SymbolType + keyB].ImageSource;
            else
                Source = GraphicsScene.NoSource;
        }

        public bool Enable
        {
            get { return _isEnable; }
            set
            {
                _isEnable = value;
                ChangeIOSource(State.Unknown.ToString().ToLower(), State.Unknown.ToString().ToLower());
                StopAnimation();
                
                if (IOType == IOType.Input)
                    _state = (byte)InputState.Unknown;
                else
                    _state = (byte)OutputState.Unknown;
            }
        }

        public void ChangeState(byte state)
        {
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                _state = state;

                if (IOType == IOType.Input)
                {
                    switch ((InputState) state)
                    {
                        case InputState.Alarm:
                            ChangeIOSource(State.On.ToString().ToLower(), State.Alarm.ToString().ToLower());
                            StopAnimation();
                            break;

                        case InputState.Break:
                        case InputState.Short:
                            ChangeIOSource(State.On.ToString().ToLower(), State.Alarm.ToString().ToLower());
                            StartAnimation(_durationFast);
                            break;

                        case InputState.Normal:
                            ChangeIOSource(State.Off.ToString().ToLower(), State.Normal.ToString().ToLower());
                            StopAnimation();
                            break;

                        default:
                            ChangeIOSource(State.Unknown.ToString().ToLower(), State.Unknown.ToString().ToLower());
                            StopAnimation();
                            break;
                    }
                }
                else
                {
                    switch ((OutputState)state)
                    {
                        case OutputState.On:
                            ChangeIOSource(State.On.ToString().ToLower(), State.Alarm.ToString().ToLower());
                            StopAnimation();
                            break;

                        case OutputState.Off:
                            ChangeIOSource(State.Off.ToString().ToLower(), State.Normal.ToString().ToLower());
                            StopAnimation();
                            break;

                        default:
                            ChangeIOSource(State.Unknown.ToString().ToLower(), State.Unknown.ToString().ToLower());
                            StopAnimation();
                            break;
                    }   
                }
            }));
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

        public void SetObjectGuid(Guid id)
        {
            IdIO = id;
        }

        public Guid GetObjectGuid()
        {
            return IdIO;
        }

        public void ShowInfo()
        {
        }

        public object GetState()
        {
            if (IOType == IOType.Input)
                return ((InputState) _state).ToString();
            
            return ((OutputState) _state).ToString();
        }

        public ObjectType GetObjectType()
        {
            if (IOType == IOType.Input)
                return ObjectType.Input;
            
            return ObjectType.Output;
        }

        public string GetObjectTypeName()
        {
            if (IOType == IOType.Input)
                return "Input";
            
            return "Output";
        }

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
            return new SerializableIO(this);
        }
    }

    [Serializable()]
    public class SerializableIO : SerializableObject, IGraphicsDeserializeObject
    {
        public double DisplayWidth { get; set; }
        public double DisplayHeight { get; set; }
        public Guid guid { get; set; }
        public SymbolType SymbolType { get; set; }
        public IOType IOType { get; set; }
        public SerializableText label { get; set; }

        public SerializableIO(GraphicsIO obj)
        {
            Serialize(obj);
            guid = obj.IdIO;
            SymbolType = obj.SymbolType;
            IOType = obj.IOType;

            if (obj.GetLabel() != null)
                label = new SerializableText(obj.GetLabel());
        }

        public GraphicsIO GetGraphicsIO(Canvas canvas, Dictionary<string, SymbolParemeter> symbols)
        {
            var obj = new GraphicsIO(canvas, symbols, SymbolType);
            Deserialize(obj);
            obj.IdIO = guid;
            obj.IOType = IOType;
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
            return GetGraphicsIO(canvas, symbols);
        }
    }
}
