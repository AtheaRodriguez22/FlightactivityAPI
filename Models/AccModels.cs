namespace FlightactivityAPI.Models
{
    public class LoyaltyAccount
    {
        public Guid AccountID { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Contact { get; set; } = "";
        public string FlightNumber { get; set; } = "";
        public List<string> PointsHistory { get; set; } = new List<string>();
        public List<string> UsedVouchers { get; set; } = new List<string>();

        public string GetTier()
        {
            if (Points >= 2000) return "Platinum";
            if (Points >= 1200) return "Gold";
            if (Points >= 800) return "Silver";
            if (Points >= 500) return "Bronze";
            return "Unranked";
        }

        public string GetTierProgress()
        {
            if (Points >= 2000) return "Woah! You have reached the highest tier: Platinum!";
            if (Points >= 1200) return $"Platinum: {2000 - Points} pts to go (need 2000)";
            if (Points >= 800) return $"Gold:     {1200 - Points} pts to go (need 1200)";
            if (Points >= 500) return $"Silver:   {800 - Points} pts to go (need 800)";
            return $"Bronze:   {500 - Points} pts to go (need 500)";
        }

        public int Points { get; set; }
    
    }
}
