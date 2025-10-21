using System;
using System.Collections.Generic;
using System.Drawing;

namespace Contal.Cgp.Client.PluginSupport
{
    public interface IPluginMainForm
    {
        string Name { get; }
        Icon Icon { get; }
        Image FormImage { get; }
        EventHandler OnClose { get; set; }

        bool HasAccessView();
        void SpecialAction(List<Object> objects);
        void Show();
        void Close();
    }
}