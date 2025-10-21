namespace Contal.Cgp.NCAS.CCU
{
    public class FileOptionsEventsFromAutonomousRun
    {
        private readonly string _fileName;
        private readonly int _eventsCount;
        private readonly long _fileSize;

        public string FileName
        {
            get { return _fileName; }
        }

        public int EventsCount
        {
            get { return _eventsCount; }
        }

        public long FileSize
        {
            get { return _fileSize; }
        }

        public FileOptionsEventsFromAutonomousRun(string fileName, int eventsCount, long fileSize)
        {
            _fileName = fileName;
            _eventsCount = eventsCount;
            _fileSize = fileSize;
        }
    }
}
