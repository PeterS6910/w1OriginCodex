using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.Client;

namespace Contal.Cgp.NCAS.Client
{
    public partial class APBZConflictDialog : CgpTranslateForm
    {
        private readonly IModel _model;

        public enum ResultAction
        {
            ToggleAccessMode,
            RemoveFromOtherZones,
            RemoveFromThisZone
        }

        public interface IModel
        {
            IEnumerable<string> CardReadersNormalAccess { get; }
            IEnumerable<string> AntiPassBackZonesNormalAccess { get; }

            IEnumerable<string> CardReadersAccessPermitted { get; }
            IEnumerable<string> AntiPassBackZonesAccessPermitted { get; }

            IEnumerable<string> CardReadersAccessInterrupted { get; }
            IEnumerable<string> AntiPassBackZonesAccessInterrupted { get; }

            ResultAction ResultAction { set; }
        }

        public class DummyModel : IModel
        {
            public IEnumerable<string> CardReadersNormalAccess
            {
                get { return Enumerable.Empty<string>(); }
            }

            public IEnumerable<string> AntiPassBackZonesNormalAccess
            {
                get { return Enumerable.Empty<string>(); }
            }

            public IEnumerable<string> CardReadersAccessPermitted
            {
                get
                {
                    return Enumerable.Empty<string>();
                }
            }

            public IEnumerable<string> AntiPassBackZonesAccessPermitted
            {
                get { return Enumerable.Empty<string>(); }
            }

            public IEnumerable<string> CardReadersAccessInterrupted
            {
                get
                {
                    return Enumerable.Empty<string>();
                }
            }

            public IEnumerable<string> AntiPassBackZonesAccessInterrupted
            {
                get { return Enumerable.Empty<string>(); }
            }

            public ResultAction ResultAction
            {
                set
                {

                }
            }
        }

        public APBZConflictDialog()
        {
            _model = new DummyModel();
            InitializeComponent();
        }

        public APBZConflictDialog(IModel model)
            : base(NCASClient.LocalizationHelper)
        {
            _model = model;
            InitializeComponent();
        }

        private void _bOK_Click(object sender, EventArgs e)
        {
            if (_rbToggleAccessMode.Checked)
                _model.ResultAction = ResultAction.ToggleAccessMode;
            else
                if (_rbRemoveFromThisZone.Checked)
                    _model.ResultAction = ResultAction.RemoveFromThisZone;
                else
                    if (_rbRemoveFromOtherZones.Checked)
                        _model.ResultAction = ResultAction.RemoveFromOtherZones;
        }

        private void APBZConflictDialog_Load(object sender, EventArgs e)
        {
            foreach (var cardReaderName in _model.CardReadersNormalAccess)
                _lbCardReadersNormalAccess.Items.Add(cardReaderName);

            foreach (var antiPassBackZoneName in _model.AntiPassBackZonesNormalAccess)
                _lbConflictingZonesNormalAccess.Items.Add(antiPassBackZoneName);

            foreach (var cardReaderName in _model.CardReadersAccessPermitted)
                _lbCardReadersAccessPermitted.Items.Add(cardReaderName);

            foreach (var antiPassBackZoneName in _model.AntiPassBackZonesAccessPermitted)
                _lbConflictingZonesAccessPermitted.Items.Add(antiPassBackZoneName);

            foreach (var cardReaderName in _model.CardReadersAccessInterrupted)
                _lbCardReadersAccessInterrupted.Items.Add(cardReaderName);

            foreach (var antiPassBackZoneName in _model.AntiPassBackZonesAccessInterrupted)
                _lbConflictingZonesAccessInterrupted.Items.Add(antiPassBackZoneName);
        }
    }
}
