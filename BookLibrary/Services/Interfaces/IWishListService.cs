using Services.DTO;
using Services.Filters;

namespace Services.Interfaces
{
    public interface IWishListService: IService<WishListDTO, IFilter>
    {
    }
}
