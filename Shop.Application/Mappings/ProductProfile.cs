using Shop.Application.DTOs;
using Shop.Domain;
using AutoMapper;

namespace Shop.Application.Mappings;
public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>();

        CreateMap<CreateProductDto, Product>();
    }
}