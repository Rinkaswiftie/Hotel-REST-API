using HotelAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HotelAPI.Data
{
    public class HotelierDBConText : DbContext
    {
        public HotelierDBConText(DbContextOptions<HotelierDBConText> options) : base(options)
        {

        }

        public DbSet<Hotel>? hotel { get; set; }
        public DbSet<User>? Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var valueComparer = new ValueComparer<ICollection<Role>>(
                            (c1, c2) => c1.SequenceEqual(c2),
                            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                            c => (ICollection<Role>)c.ToHashSet());

            modelBuilder.Entity<User>()
            .Property(e => e.UserRoles)
            .HasConversion(
             v => string.Join(',', v),
             v => 
             v.Split(',', StringSplitOptions.RemoveEmptyEntries)
             .Select(r => Enum.Parse<Role>(r)).ToList())
             .Metadata.SetValueComparer(valueComparer);
        }
    }
}