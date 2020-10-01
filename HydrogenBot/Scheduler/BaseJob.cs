using System.Threading.Tasks;
using Quartz;

namespace HydrogenBot.Scheduler
{
    public abstract class BaseJob : IJob
    {
        public static int SecondsInterval => 30;
        
        public abstract Task Execute(IJobExecutionContext context);
    }
}
