using Contal.IwQuick;

namespace Contal.IwQuick.Data
{
    public class VersionedStreamingContext
    {
        private int _thresholdVersion;

        public int ThresholdVersion
        {
            get { return _thresholdVersion; }
        }
         
        public VersionedStreamingContext(int i_iThresholdVersion)
        {
            Validator.CheckIntegerRange(i_iThresholdVersion,1,int.MaxValue);

            _thresholdVersion = i_iThresholdVersion;
        }
    }
}
