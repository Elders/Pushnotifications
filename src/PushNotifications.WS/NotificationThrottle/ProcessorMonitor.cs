using System;
using System.Linq;
using System.Management;

namespace PushNotifications.WS.NotificationThrottle
{
    public class ProcessorMonitor
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(ProcessorMonitor));

        CircularBuffer<int> stats;
        TimeSpan interval;
        DateTime lastInit;

        public ProcessorMonitor(int size, TimeSpan interval)
        {
            this.interval = interval;
            lastInit = DateTime.UtcNow;
            stats = new CircularBuffer<int>(size);
        }

        public int Usage()
        {
            CollectInfo();

            var values = stats.GetValues();

            if (values.Count == 0)
                return 0;
            else
            {
                var sum = values.Sum();

                return sum / values.Count;
            }
        }

        public void CollectInfo()
        {
            if ((DateTime.UtcNow - lastInit > interval))
            {
                stats.Clear();
                lastInit = DateTime.UtcNow;
            }

            var usage = GetProcessorUsage();
            if (usage != -1)
                stats.Write(usage);
        }

        private int GetProcessorUsage()
        {
            try
            {
                var select = new SelectQuery("Win32_PerfFormattedData_PerfOS_Processor");
                var searcher = new ManagementObjectSearcher(select);

                ManagementObjectCollection collection = searcher.Get();
                ManagementObject queryObj = collection.Cast<ManagementObject>().First();

                return 100 - Convert.ToInt32(queryObj["PercentIdleTime"]);
            }
            catch (Exception ex)
            {
                log.Error("Could not get cpu status", ex);
            }

            return -1;
        }
    }
}