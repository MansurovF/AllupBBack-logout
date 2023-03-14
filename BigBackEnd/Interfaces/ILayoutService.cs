using BigBackEnd.Models;
using BigBackEnd.ViewModels.BasketViewModels;

namespace BigBackEnd.Interfaces
{
    public interface ILayoutService
    {
        Task<IDictionary<string,string>> GetSettings();
        Task<IEnumerable<Category>> GetCategories();
        Task<List<BasketVM>> GetBasket();
    }
}
