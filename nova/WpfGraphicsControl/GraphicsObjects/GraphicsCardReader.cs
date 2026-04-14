using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Cgp.NCAS.WpfGraphicsControl;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.WpfGraphicsControl
{
    public class GraphicsCardReader : GraphicsSymbol, ILiveObject
    {
        private Dictionary<string, SymbolParemeter> _symbols;
        private Storyboard _storyboard = new Storyboard();
        private TimeSpan _duration = new TimeSpan(0, 0, 0, 0, 200);
        private string _state;
        private bool _isEnable = true;

        public Guid guid { get; set; }

        public GraphicsCardReader(Canvas canvas, Dictionary<string, SymbolParemeter> symbols)
            : base(canvas)
        {
            _symbols = symbols;
            string key = SymbolType.CardReader + State.Offline.ToString().ToLower();

            if (_symbols.ContainsKey(key))
                Source = _symbols[key].ImageSource;

            if (Source == null)
                Source = GraphicsScene.NoSource;

            //configure animation
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 0.7;
            animation.To = 1.0;
            animation.AutoReverse = true;
            animation.Duration = new Duration(_duration);
            animation.RepeatBehavior = RepeatBehavior.Forever;
            Storyboard.SetTargetProperty(animation, new PropertyPath(OpacityProperty));
            _storyboard.Children.Add(animation);
        }

        public void SetOnlineState(OnlineState state)
        {
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                _state = state.ToString();
                string key = SymbolType.CardReader + state.ToString().ToLower();

                if (!_symbols.ContainsKey(key))
                    return;

                Source = _symbols[key].ImageSource;
                StartAnimation(false);
            }));
        }

        public bool Enable
        {
            get { return _isEnable; }
            set
            {
                _isEnable = value;
                //string key = SymbolType.CardReader + State.Offline.ToString().ToLower();

                string key = SymbolType.CardReader + State.Unknown.ToString().ToLower();

                if (_symbols.ContainsKey(key))
                    Source = _symbols[key].ImageSource;

                StartAnimation(false);
                _state = State.Unknown.ToString();
            }
        }

        public void ChangeState(byte state)
        {
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                switch ((State)state)
                {
                    case State.sabotage:
                    case State.Tamper:
                        StartAnimation(true);
                        _state = State.Tamper.ToString();
                        break;

                    case State.Unknown:
                        SetOnlineState(OnlineState.Offline);
                        StartAnimation(false);
                        break;

                    default:
                        SetOnlineState(OnlineState.Online);
                        StartAnimation(false);
                        break;
                }

                string key = SymbolType.CardReader + ((State)state).ToString().ToLower();

                if (!_symbols.ContainsKey(key))
                    return;

                Source = _symbols[key].ImageSource; 
            }));
        }

        public void StartAnimation(bool start)
        {
            if (start)
                _storyboard.Begin(this, true);
            else
                _storyboard.Stop(this);
        }

        public void SetObjectGuid(Guid id)
        {
            guid = id;
        }

        public Guid GetObjectGuid()
        {
            return guid;
        }
        
        public void ShowInfo()
        {
        }

        public object GetState()
        {
            return _state;
        }

        public string GetObjectTypeName()
        {
            return "Card reader";
        }

        public ObjectType GetObjectType()
        {
            return ObjectType.CardReader;
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
            return new SerializableCardReader(this);
        }
    }

    [Serializable()]
    public class SerializableCardReader : SerializableObject, IGraphicsDeserializeObject
    {
        public Guid guid { get; set; }
        public SerializableText label { get; set; }

        public SerializableCardReader(GraphicsCardReader obj)
        {
            Serialize(obj);
            guid = obj.guid;

            if (obj.GetLabel() != null)
                label = new SerializableText(obj.GetLabel());
        }

        public GraphicsCardReader GetGraphicsCardReader(Canvas canvas, Dictionary<string, SymbolParemeter> symbols)
        {
            GraphicsCardReader obj = new GraphicsCardReader(canvas, symbols);
            Deserialize(obj);
            obj.guid = guid;

            if (label != null)
            {
                Text text = label.GetText(canvas);
                obj.SetLabel(text);
                text.LiveObject = obj;
            }

            obj.LoadDefaulParameters();
            canvas.Children.Add(obj);
            return obj;
        }

        public IGraphicsObject Deserialize(Canvas canvas, Dictionary<string, SymbolParemeter> symbols)
        {
            return GetGraphicsCardReader(canvas, symbols);
        }
    }
}
