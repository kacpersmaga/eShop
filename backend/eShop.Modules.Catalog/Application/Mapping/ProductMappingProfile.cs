using AutoMapper;
using eShop.Modules.Catalog.Application.Dtos;
using eShop.Modules.Catalog.Domain.Aggregates;

namespace eShop.Modules.Catalog.Application.Mapping;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<CreateProductDto, Product>()
            .ConvertUsing((src, _) => Product.Create(
                name: src.Name,
                price: src.Price,
                category: src.Category,
                description: src.Description,
                imagePath: null
            ));
        
        CreateMap<UpdateProductDto, Product>()
            .ForAllMembers(opts => opts.Ignore());
        
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name.Value))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description.Value))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.Value))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Price.Currency))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Value))
            .ForMember(dest => dest.ImageUri, opt => opt.MapFrom<ImageUriResolver>());
    }
}