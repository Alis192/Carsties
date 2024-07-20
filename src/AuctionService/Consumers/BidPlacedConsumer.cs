using AuctionService.Data;
using Contracts;
using MassTransit;

namespace AuctionService
{
    public class BidPlacedConsumer : IConsumer<BidPlaced>
    {
        private readonly AuctionDbContext _dbContext;

        // Correct constructor with no return type
        public BidPlacedConsumer(AuctionDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Method with correct return type
        public async Task Consume(ConsumeContext<BidPlaced> context)
        {
            System.Console.WriteLine("--> Consuming Bid placed");

            var auction = await _dbContext.Auctions.FindAsync(Guid.Parse(context.Message.AuctionId));

            if (auction.CurrentHighBid == null 
                || (context.Message.BidStatus.Contains("Accepted")
                && context.Message.Amount > auction.CurrentHighBid))
            {
                auction.CurrentHighBid = context.Message.Amount;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
