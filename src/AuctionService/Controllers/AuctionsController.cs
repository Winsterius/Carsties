using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase
{
    private readonly AuctionDbContext _context;
    private readonly IMapper _mapper;

    public AuctionsController(AuctionDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
    {
        var auctions = await _context.Auctions
            .Include(x => x.Item)
            .OrderBy(x => x.Item.Make)
            .ToListAsync();

        return _mapper.Map<List<AuctionDto>>(auctions);
    }
    
    
    [HttpGet("id")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auction = await _context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if(auction == null) return NotFound();
        
        return _mapper.Map<AuctionDto>(auction);
    }
    
    [HttpPost()]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDte)
    {
        
        var auction = _mapper.Map<Auction>(auctionDte);

        auction.Seller = "test";
        await _context.Auctions.AddAsync(auction);
        
        var result = await _context.SaveChangesAsync() > 0;


        if(!result) return BadRequest("Could not save changes to the DB");

        return CreatedAtAction(nameof(GetAuctionById), new { auction.Id }, _mapper.Map<AuctionDto>(auction));
        
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<AuctionDto>> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDte)
    {
        
        var auction = await _context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);

        if(auction == null) return NotFound();
        
        // Check seller = username

        auction.Item.Make = updateAuctionDte.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDte.Model ?? auction.Item.Model;
        auction.Item.Color = updateAuctionDte.Color ?? auction.Item.Color;
        auction.Item.Mileage = updateAuctionDte.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = updateAuctionDte.Year ?? auction.Item.Year;

        var result = await _context.SaveChangesAsync() > 0;

        if(!result) return BadRequest("Could not save changes to the DB");

        return Ok();
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {

        var auction = await _context.Auctions.FindAsync(id);

        if(auction == null) return NotFound();

        _context.Auctions.Remove(auction);

        var result = await _context.SaveChangesAsync() > 0;

        if(!result) return BadRequest("Could not save changes to the DB");

        return Ok();
    }
}