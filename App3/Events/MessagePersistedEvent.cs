namespace App3
{
    public class MessagePersistedEvent : IEvent
    {
        public string Message { get; set; }
    }

    public interface IEvent
    {

    }
}
