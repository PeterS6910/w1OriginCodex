using Contal.Drivers.CardReader;
using Contal.Drivers.ClspDrivers;

namespace Contal.Cgp.NCAS.NodeDataProtocol
{
    public static class ClspDriversExtension
    {
        public static CRNestedCommunicator GetCrCommunicator(this IClspNode clspNode)
        {
            return clspNode.GetParameter<CRNestedCommunicator>(NodeCommunicator.ParamCrCommunicatorNode);
        }
    }   
}
