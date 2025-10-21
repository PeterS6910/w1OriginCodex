using System.Collections.Generic;
using System.Windows.Controls;
using Contal.Cgp.NCAS.WpfGraphicsControl;

namespace Cgp.NCAS.WpfGraphicsControl
{
    public interface IGraphicsDeserializeObject
    {
        IGraphicsObject Deserialize(Canvas canvas, Dictionary<string, SymbolParemeter> symbols);
    }
}
