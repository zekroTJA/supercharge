using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Crawler
{
    class SchedulerTask
    {
        public Action Action;
        public DateTime ExecuteAt;
        public DateTime LastExecution;
    }

    class Scheduler
    {
        private Timer timer;
        private readonly List<SchedulerTask> tasks = new List<SchedulerTask>();
        private readonly TimeSpan actionCooldown;

        private int _rollingActionId = 0;
        private int RollingActionId => _rollingActionId++;

        public Scheduler(TimeSpan actionCooldown)
        {
            this.actionCooldown = actionCooldown;
        }

        public void Start(TimeSpan startAt, TimeSpan elapseDuration)
        {
            timer = new Timer(new TimerCallback(OnTimerElapsed), null, startAt, elapseDuration);
        }

        public Scheduler DoAt(Action action, DateTime at)
        {
            tasks.Add(new SchedulerTask
            {
                Action = action,
                ExecuteAt = at
            });
            return this;
        }

        public Scheduler DoAt(Action action, ICollection<DateTime> at)
        {
            at.ToList().ForEach(dt => DoAt(action, dt));
            return this;
        }

        private void OnTimerElapsed(object v)
        {
            var now = DateTime.Now;
            tasks.ForEach(t => ExecIfIsTime(t, now));
        }

        private void ExecIfIsTime(SchedulerTask task, DateTime now)
        {
            if (now.Hour != task.ExecuteAt.Hour || now.Minute != task.ExecuteAt.Minute)
                return;

            if (now - task.LastExecution < actionCooldown)
                return;

            task.LastExecution = now;
            task.Action();
        }
    }
}
