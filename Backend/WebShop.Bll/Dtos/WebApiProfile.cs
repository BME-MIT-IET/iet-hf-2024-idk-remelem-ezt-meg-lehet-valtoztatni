using AutoMapper;

namespace WebShop.Bll.Dtos;

public class WebApiProfile : Profile
{
    public WebApiProfile()
    {
        CreateMap<Dal.Entities.User, Bll.Dtos.UserIn>().ReverseMap();
        CreateMap<Dal.Entities.User, Bll.Dtos.UserOut>();
        CreateMap<Dal.Entities.Order, Bll.Dtos.OrderIn>().ReverseMap();
        CreateMap<Dal.Entities.Order, Bll.Dtos.OrderOut>();
        CreateMap<Dal.Entities.OrderItem, Bll.Dtos.OrderItemIn>().ReverseMap();
        CreateMap<Dal.Entities.OrderItem, Bll.Dtos.OrderItemOut>();
        CreateMap<Dal.Entities.Product, Bll.Dtos.ProductIn>().ReverseMap();
        CreateMap<Dal.Entities.Product, Bll.Dtos.ProductOut>();
        CreateMap<Dal.Entities.Category, Bll.Dtos.Category>().ReverseMap();
    }
}
