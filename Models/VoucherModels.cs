namespace FlightactivityAPI.Models
{
    public class VoucherCode
    {
        public string Code { get; set; } = "";
        public int Points { get; set; }

        public static VoucherCode[] DefaultVouchers = new VoucherCode[] {
            new VoucherCode { Code = "FLY50",      Points = 50  },
            new VoucherCode { Code = "BONUS100",   Points = 100 },
          };
    }
}
