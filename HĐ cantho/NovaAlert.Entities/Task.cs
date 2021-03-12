using System;

namespace NovaAlert.Entities
{
    public class Task
    {
        public eTask CurrentTask { get; set; }
        public eTaskLevel Level { get; set; }
        public eTaskResult Result { get; set; }
        public DateTime CreatedDate { get; set; }

        public Task()
        {
            CurrentTask = eTask.None;
            Level = eTaskLevel.None;
            Result = eTaskResult.None;
        }
    }
}
