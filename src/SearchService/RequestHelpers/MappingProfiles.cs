using AutoMapper;
using Contracts;
using SearchService;

namespace SearchService;
public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<AuctionCreated, Item>();
        CreateMap<AuctionUpdated, Item>();
    }
}
