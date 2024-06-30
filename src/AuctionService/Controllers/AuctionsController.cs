using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase 
{
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;

    public AuctionsController(AuctionDbContext context, IMapper mapper, IPublishEndpoint publishEndpoint) 
    {
        _context = context;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }


    /// <summary>
    /// This method retrieves all auctions from the database that were updated after a specified date, orders them by the make of the item, maps them to a list of AuctionDto objects, and returns the list. If no date is specified, it retrieves all auctions.
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string date)
    {

        var query = _context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();    

        if(!string.IsNullOrEmpty(date))
        {
            //Will be true only when UpdatedAt is later than the input date. This means the auction was updated after the provided date.
            query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
        }


        return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id) 
    {
        var auction = await _context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null) return NotFound();

        return _mapper.Map<AuctionDto>(auction);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto) 
    {
        var auction = _mapper.Map<Auction>(auctionDto); 
        
        // TODO: Add current user as seller 
        auction.Seller = User.Identity.Name;
        
        //EF tracks it in the memory
        _context.Auctions.Add(auction);

        // Maps from Auction to AuctionDto
        var newAuction = _mapper.Map<AuctionDto>(auction);


        //Mapping AuctionDto instance to AuctionCreated and publishing it to message bus
        await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));

        var result = await _context.SaveChangesAsync() > 0;

        if(!result) return BadRequest("Could not save changes to the DB");

        return CreatedAtAction(nameof(GetAuctionById), new {auction.Id}, _mapper.Map<AuctionDto>(auction));
    }


    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto) 
    {
        var auction = await _context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null) return NoContent();

        if (auction.Seller != User.Identity.Name) return Forbid(); //403

        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;
        auction.AuctionEnd = DateTime.UtcNow.AddYears(1); 

        //TO DO: Map from Auction to UpdateAuction
        await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));

        var result = await _context.SaveChangesAsync() > 0;

        if (result) return Ok();

        return BadRequest("Problem saving");
    }   


    
    // [HttpPut]
    // public async Task<ActionResult> UpdateAuctionDate() 
    // {

    // }


    [Authorize]
    [HttpDelete("{id}")]    
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await _context.Auctions.FindAsync(id);

        if (auction == null) return NotFound();

        if (auction.Seller != User.Identity.Name) return Forbid();

        _context.Auctions.Remove(auction);

        await _publishEndpoint.Publish<AuctionDeleted>(new {Id = auction.Id.ToString()});

        var result = await _context.SaveChangesAsync() > 0;

        if(!result) return BadRequest("Could not update DB");

        return Ok();
    }

}