using System.ServiceProcess;

namespace AnalyticTool.Advitech
{
    partial class AnalyticToolAdvitechService : ServiceBase
    {
        public AnalyticToolAdvitechService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Core.Singleton.Start();
        }

        protected override void OnStop()
        {
            Core.Singleton.Stop();
        }
    }
}
