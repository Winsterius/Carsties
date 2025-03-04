namespace AuctionService.Entities;

public class Auction
{
    public Guid Id { get; set; }

    public int ReservePrice { get; set; } = 0;

    public string Seller { get; set; }

    public string Winner { get; set; }

    public int? SoldMount { get; set; }

    public int? CurrentHighBit { get; set; }

    public DateTime CreateAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdateAt { get; set; } = DateTime.UtcNow;

    public DateTime AuctionEnd { get; set; }

    public Status Status { get; set; }

    public Item Item{ get; set; }
}