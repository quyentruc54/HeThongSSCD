using System.Runtime.Serialization;

namespace NovaAlert.Entities
{
    [DataContract]
    public class TaskChangedEventArgs:BaseEventArgs
    {
        [DataMember]
        public int Id { get; private set; }
        [DataMember]
        public Task Task { get; set; }

        public TaskChangedEventArgs(int id, Task task)
        {
            this.Id = id;
            this.Task = task;
        }
    }
}
