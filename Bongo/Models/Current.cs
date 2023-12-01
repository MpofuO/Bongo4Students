namespace Bongo.Models
{
    public static class Current
  
    {
        public static BongoUser User { get; set; }
        public static bool IsAuthenticated => User is not null;
    }
}
