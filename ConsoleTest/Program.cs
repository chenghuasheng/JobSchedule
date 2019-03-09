using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using HuaQuant.JobSchedule;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            JobSchedule schedule = new JobSchedule();
            IJob job = new TestJob();
            //ITrigger trigger = new RepeatTrigger(new TimeSpan(0, 0, 5), DateTime.Parse("11:00:00"), DateTime.Parse("11:08:00"));
            //ITrigger trigger = new RepeatTrigger(new TimeSpan(0, 0, 5), DateTime.Parse("12:50:00"), 10);
            //ITrigger trigger = new SingleTrigger(DateTime.Parse("12:47:00"));
            //ITrigger trigger = new SingleTrigger(DateTime.Parse("12:59:00"),5, new TimeSpan(0, 0, 5));
            ITrigger trigger = new SingleTrigger(DateTime.Parse("12:59:00"), DateTime.Parse("19:59:00"), 5, new TimeSpan(0, 0, 5));
            ITrigger trigger2 = new SingleTrigger(DateTime.Parse("12:59:00"), DateTime.Parse("19:59:00"), 5, new TimeSpan(0, 0, 5));
            schedule.Add(job, trigger);
            IJob job2 = new TestJob("test2",new IJob[] { job});
            
            schedule.Start();
            Thread.Sleep(1000);
            schedule.Add(job2, trigger2);
            schedule.ClearExpiredJobs();
            Console.ReadKey();
            schedule.ClearExpiredJobs();

            schedule.Stop();
            Console.ReadKey();
        }
    }
    public class TestJob : Job,IJob
    {
        public TestJob() : base("TestJob", null, true) { }
        public TestJob(string name,IEnumerable<IJob> needJobs) : base(name, needJobs, true) { }
        public override bool Execute()
        {
            Console.WriteLine("Job <{0}> run a time", this.Name);
            return false;
        }
    }
}
