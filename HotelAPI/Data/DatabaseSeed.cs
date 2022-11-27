
using HotelAPI.Models;

namespace HotelAPI.Data
{
    public class DatabaseSeed
    {
        public static void Seed(HotelierDBConText? context)
        {
            if (context == null) return;
            
            context.Database.EnsureCreated();

            if (context.Users.Count() == 0)
            {
                var users = new List<User>
                {
                    new User{
                        Email = "admin@admin.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("12345678"),
                        Name = "John Doe",
                        DateOfBirth = new DateTime(1995,09,15),
                        UserRoles = new List<Role>{Role.Admin, Role.User},
                        },
                    new User{
                        Email = "common@common.com",
                        Password = BCrypt.Net.BCrypt.HashPassword("12345678"),
                        Name = "Jane Doe",
                        DateOfBirth = new DateTime(2000,04,11),
                        UserRoles = new List<Role>{Role.User}
                        }
                };
                context.Users.AddRange(users);
                context.SaveChanges();
            }

            if (context.hotel.Count() == 0)
            {
                var hotels = new List<Hotel>
                {
                    new Hotel {
                        Name= "The Marbella",
                        Description="The hotel features a full-service restaurant, a large pool area, private cabanas, five hospital-grade recovery suites(complete with revamped recovery spa), gardens and an outdoor bar",
                        IsActive= true,
                        ImageName = ""
                    },
                    new Hotel {
                        Name="Hotel Transylvania",
                        Description ="Welcome to the Hotel Transylvania, Dracula's lavish five-stake resort, where monsters and their families can live it up, free from meddling from the human world.",
                        IsActive= true,
                        ImageName = ""
                    }
                };
                context.hotel.AddRange(hotels);
                context.SaveChanges();
            }

        }
    }

}