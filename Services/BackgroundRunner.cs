using System;
using System.Threading.Tasks;
using System.Timers;
using Butler.Properties;

namespace Butler
{
    public static class BackgroundRunner
    {
        public static void Init()
        {
            QueryProvider.Instance.Load();

            var timer = new Timer();
            timer.Elapsed += (sender, e) => { Run(() => { Repository.Instance.Refresh(); }); };
            timer.Interval = Settings.Default.Interval*1000;
            timer.AutoReset = true;
            timer.Start();
        }

        public static void Run(Action action)
        {
            Task.Factory.StartNew(action);
        }
    }
}