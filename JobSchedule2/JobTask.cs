using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HuaQuant.JobSchedule2
{
    internal class JobTask
    {
        private JobProcess process;
        private Task<bool> task = null;
        internal Task<bool> InnerTask => this.task;
        private IJob runningJob;
        private Dictionary<IJob, bool> succeedDict = new Dictionary<IJob, bool>();
        internal Dictionary<IJob, bool> SucceedDict => this.succeedDict;
        private bool completed = true;
        internal bool Completed => this.completed;
        private bool succeed = false;
        internal bool Succeed => this.succeed;
        internal JobTask(JobProcess process)
        {
            this.process = process;
            foreach (IJob job in this.process.Jobs)
            {
                this.succeedDict.Add(job, true);
            }
        }
       
        internal void Start()
        {
            if (this.process.Jobs.Count <= 0)
            {
                Console.WriteLine("此作业进程没有包含作业。");
                return;
            }
            this.completed = false;
            this.succeed = false;
            this.task = new Task<bool>(run,this.process.CancelToken);
            this.task.ContinueWith(task => {
                this.completed = task.IsCompleted;
                this.succeed = task.Result;
                this.process.OnTaskCompleted(this);
            });
            this.process.OnTaskStart();
            this.task.Start();
        }
        private bool run()
        {
            bool ret = true;
            try
            {
                foreach (IJob job in this.process.Jobs)
                {
                    this.runningJob = job;
                    if (job.ShowDetail) Console.WriteLine("在时间{0},开始作业<{1}>的执行...", DateTime.Now, job.Name);
                    bool succeed = job.Execute(this.process.CancelToken);
                    if (job.ShowDetail)
                    {
                        if (succeed) Console.WriteLine("在时间{0},作业<{1}>顺利完成。", DateTime.Now, job.Name);
                        else Console.WriteLine("在时间{0},作业<{1}>未能正常完成。", DateTime.Now, job.Name);
                    }
                    ret = ret && succeed;
                    this.succeedDict[job] = succeed;
                }
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException)
                {
                    Console.WriteLine("在时间{0},{1}作业被取消", DateTime.Now, this.runningJob.Name);
                    if (this.process.Jobs.Count > 1) Console.WriteLine("作业<{1}>所在的作业队列也被取消。", this.runningJob.Name);

                }
                else
                {
                    Console.WriteLine("在时间{0},作业<{1}>发生异常:{2}", DateTime.Now, this.runningJob.Name, ex.Message);
                    if (this.process.Jobs.Count > 1) Console.WriteLine("作业<{1}>所在的作业队列被中断。", this.runningJob.Name);
                }
            }
            return ret;
        }
    }
}
