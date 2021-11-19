using System.Collections.Generic;
using System.Threading.Tasks;

namespace House.HLL.News.Interfaces
{
    public interface IAutoNewsConsumer
    {
        List<Models.NewsMessage> CurrentNews();
    }
}