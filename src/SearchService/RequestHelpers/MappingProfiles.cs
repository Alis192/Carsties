using AutoMapper;
using Contracts;
using SearchService;

namespace Namespace;
public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<AuctionCreated, Item>();
    }
}
