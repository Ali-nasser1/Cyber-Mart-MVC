using AutoMapper;
using CyberMart.DataAccess.Models;
using CyberMart.ViewModels;

namespace CyberMart.MappingProfiles
{
	public class ProductProfile : Profile
	{
		public ProductProfile()
		{
			CreateMap<ProductViewModel, Product>().ReverseMap();
		}
	}
}
