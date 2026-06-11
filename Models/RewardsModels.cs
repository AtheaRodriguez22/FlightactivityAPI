namespace FlightactivityAPI.Models
{
    public class RewardOption

    {
        public int RewardId { get; set; }
        public string Name { get; set; } = "";
        public int Cost { get; set; }

        public static RewardOption[] DefaultRewards = {
            new RewardOption { RewardId = 1, Name = "KFC",              Cost = 2000 },
            new RewardOption { RewardId = 2, Name = "Jollibee",         Cost = 3000 },
            new RewardOption { RewardId = 3, Name = "Wendy's",          Cost = 4000 },
            new RewardOption { RewardId = 4, Name = "McDonald's",       Cost = 4000 },
            new RewardOption { RewardId = 5, Name = "Flight Discount",  Cost = 10000 },
            new RewardOption { RewardId = 6, Name = "Business Class",   Cost = 10000 },
        };
    }
}
