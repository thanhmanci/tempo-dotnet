using System.Threading.Tasks;

namespace App3
{
    public interface ISqlRepository
    {
        Task Persist(string message);
    }
}