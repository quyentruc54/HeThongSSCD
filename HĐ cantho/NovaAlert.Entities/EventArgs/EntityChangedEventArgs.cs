namespace NovaAlert.Entities
{
    public class EntityChangedEventArgs : System.EventArgs
    {
        public int Id { get; private set; }
        public EntityChangedEventArgs(int id)
        {
            this.Id = id;
        }
    }
}
