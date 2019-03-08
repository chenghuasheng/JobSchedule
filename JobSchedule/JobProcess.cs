using System;
using System.Threading;


namespace HuaQuant.JobSchedule
{
    public class JobProcess
    {
        private Job curJob = null;
        private bool finished = false;
        public bool IsFinished => this.finished;
        public bool IsRunning
        {
            get {
                if (this.thread != null && this.thread.ThreadState == ThreadState.Running) return true;
                else return false;
            }
        }

        private Thread thread = null;
        public JobProcess(Job job)
        {
            this.curJob = job;
        }
        public void Start()
        {
            if (!this.IsRunning) {
                this.thread = new Thread(new ThreadStart(this.Run));
                this.thread.Start();
            }
        }
        public void Stop(bool block=false)
        {
            if (this.thread!= null && 
                this.thread.ThreadState != ThreadState.Aborted &&
                this.thread.ThreadState != ThreadState.Stopped){
                this.thread.Abort();
                if (block)
                {
                    this.thread.Join();
                }
            }
        }
        private void Run()
        {
            this.finished = false;
            if (this.curJob.ShowDetail) Console.WriteLine("在时间{0},开始作业<{1}>的执行...", DateTime.Now, this.curJob.Name);

            bool ret = this.curJob.Execute();
            if (ret)
            {
                this.finished = true;
                this.curJob.IncrementFrequency();
                if (this.curJob.ShowDetail) Console.WriteLine("在时间{0},作业<{1}>顺利完成。", DateTime.Now, this.curJob.Name);
            }
            else
            {
                if (this.curJob.ShowDetail) Console.WriteLine("在时间{0},作业<{1}>未能正常完成。", DateTime.Now, this.curJob.Name);
            }
        }
    }
}
