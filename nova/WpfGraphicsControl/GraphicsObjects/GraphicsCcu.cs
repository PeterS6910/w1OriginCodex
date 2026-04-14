using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.WpfGraphicsControl;

namespace Cgp.NCAS.WpfGraphicsControl
{
    public class GraphicsCcu : GraphicsSymbol, ILiveObject
    {
        private readonly Dictionary<string, SymbolParemeter> _symbols;
        private string _state;
        private bool _isEnable = true;
        private readonly Storyboard _storyboard = new Storyboard();
        private CCUOnlineState _onlineState;

        public Guid CcuGuid { get; set; }
        public SymbolType SymbolType { get; set; }

        public GraphicsCcu(Canvas canvas, Dictionary<string, SymbolParemeter> symbols, SymbolType symbolType)
            : base(canvas)
        {
            _symbols = symbols;
            SymbolType = symbolType;
            string key = SymbolType + State.Offline.ToString().ToLower();

            if (_symbols.ContainsKey(key))
                Source = _symbols[key].ImageSource;

            if (Source == null)
                Source = GraphicsScene.NoSource;

            //configure animation
            var animation = new DoubleAnimation
            {
                From = 0.2,
                To = 1.0,
                AutoReverse = true,
                Duration = new Duration(new TimeSpan(0, 0, 0, 0, 200)),
                RepeatBehavior = RepeatBehavior.Forever
            };

            Storyboard.SetTargetProperty(animation, new PropertyPath(OpacityProperty));
            _storyboard.Children.Add(animation);
        }

        public void StartAnimation()
        {
            _storyboard.Stop(this);
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
                string key = SymbolType + State.Unknown.ToString().ToLower();

                if (_symbols.ContainsKey(key))
                    Source = _symbols[key].ImageSource;

                _state = State.Unknown.ToString();
            }
        }

        public void SetObjectGuid(Guid id)
        {
            CcuGuid = id;
        }

        public Guid GetObjectGuid()
        {
            return CcuGuid;
        }

        public object GetState()
        {
            return _state;
        }

        public string GetObjectTypeName()
        {
            return "CCU";
        }

        public ObjectType GetObjectType()
        {
            return ObjectType.CCU;
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

        public void ChangeState(byte state)
        {
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                StopAnimation();

                if (((State) state) == State.Unknown)
                {
                    state = (byte) CCUOnlineState.Offline;
                    _onlineState = CCUOnlineState.Offline;
                }

                _onlineState = (CCUOnlineState)state;
                _state = _onlineState.ToString();
                ChangeSource();
            }));
        }

        public void ChangeAlarmState(byte state)
        {
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                if (((State) state) == State.Alarm)
                {
                    _state = State.Tamper.ToString();
                    StartAnimation();
                }
                else
                {
                    _state = _onlineState.ToString();
                    StopAnimation();
                }

                ChangeSource();
            }));
        }

        private void ChangeSource()
        {
            string key = SymbolType + _state.ToLower();

            if (!_symbols.ContainsKey(key))
                return;

            Source = _symbols[key].ImageSource;
        }

        public override object Serialize()
        {
            return new SerializableCcu(this);
        }
    }

    [Serializable()]
    public class SerializableCcu : SerializableObject, IGraphicsDeserializeObject
    {
        public Guid guid { get; set; }
        public SerializableText label { get; set; }
        public SymbolType SymbolType { get; set; }

        public SerializableCcu(GraphicsCcu obj)
        {
            Serialize(obj);
            guid = obj.CcuGuid;
            SymbolType = obj.SymbolType;

            if (obj.GetLabel() != null)
                label = new SerializableText(obj.GetLabel());
        }

        private GraphicsCcu GetGraphicsCcu(Canvas canvas, Dictionary<string, SymbolParemeter> symbols, SymbolType symbolType)
        {
            var obj = new GraphicsCcu(canvas, symbols, symbolType);
            Deserialize(obj);
            obj.CcuGuid = guid;

            if (label != null)
            {
                var text = label.GetText(canvas);
                obj.SetLabel(text);
                text.LiveObject = obj;
            }

            obj.LoadDefaulParameters();
            canvas.Children.Add(obj);
            return obj;
        }

        public IGraphicsObject Deserialize(Canvas canvas, Dictionary<string, SymbolParemeter> symbols)
        {
            return GetGraphicsCcu(canvas, symbols, SymbolType);
        }
    }
}
