namespace App3
{
    public interface IRabbitRepository
    {
        void Publish(IEvent evt);
    }
}
