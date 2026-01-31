using Nouhaila.netProjet.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Nouhaila.netProjet.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Roles
                string[] roles = { "Admin", "User", "Courier" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }

                // Admin User
                var adminEmail = "admin@glovo.com";
                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                   var admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail, FullName = "System Admin", EmailConfirmed = true };
                   var result = await userManager.CreateAsync(admin, "Admin123!");
                   if (result.Succeeded) await userManager.AddToRoleAsync(admin, "Admin");
                }

                // Courier User
                var courierEmail = "courier@glovo.com";
                if (await userManager.FindByEmailAsync(courierEmail) == null)
                {
                    var courier = new ApplicationUser { UserName = courierEmail, Email = courierEmail, FullName = "Jean Livreur", EmailConfirmed = true };
                    var result = await userManager.CreateAsync(courier, "Courier123!");
                    if (result.Succeeded) await userManager.AddToRoleAsync(courier, "Courier");
                }

                // Initial Data
                if (!context.Categories.Any())
                {
                    var catFastFood = new Category { Name = "Fast Food", ImageUrl = "https://cdn-icons-png.flaticon.com/512/732/732217.png" };
                    var catAsian = new Category { Name = "Asian", ImageUrl = "https://cdn-icons-png.flaticon.com/512/2252/2252075.png" };
                    var catPizza = new Category { Name = "Pizza & Italian", ImageUrl = "https://cdn-icons-png.flaticon.com/512/1404/1404945.png" };
                    var catIndian = new Category { Name = "Indian", ImageUrl = "https://cdn-icons-png.flaticon.com/512/918/918234.png" };

                    context.Categories.AddRange(catFastFood, catAsian, catPizza, catIndian);
                    await context.SaveChangesAsync();

                    // Restaurants
                    var rMcDo = new Restaurant { Name = "McDonald's", Description = "I'm lovin' it. The classic taste.", ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/4/4b/McDonald%27s_logo.svg", DeliveryTimeRange = "10-20 min", PreparationTimeMinutes = 5, DeliveryFee = 2.50m, Rating = 4.2, Category = catFastFood };
                    var rKFC = new Restaurant { Name = "KFC", Description = "It's Finger Lickin' Good.", ImageUrl = "https://upload.wikimedia.org/wikipedia/en/thumb/b/bf/KFC_logo.svg/1200px-KFC_logo.svg.png", DeliveryTimeRange = "20-30 min", PreparationTimeMinutes = 10, DeliveryFee = 3.00m, Rating = 4.0, Category = catFastFood };
                    var rTacos = new Restaurant { Name = "O'Tacos", Description = "Un Tacos de taille.", ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/e/e0/O%27tacos_Logo.svg/1200px-O%27tacos_Logo.svg.png", DeliveryTimeRange = "25-35 min", PreparationTimeMinutes = 15, DeliveryFee = 2.00m, Rating = 4.4, Category = catFastFood };
                    
                    var rSushi = new Restaurant { Name = "Sushi World", Description = "Fresh Salmon and Tuna.", ImageUrl = "https://img.freepik.com/premium-vector/sushi-logo-design-template_96807-657.jpg", DeliveryTimeRange = "30-40 min", PreparationTimeMinutes = 20, DeliveryFee = 4.50m, Rating = 4.8, Category = catAsian };
                    var rPizza = new Restaurant { Name = "Pizza Heaven", Description = "Wood fired oven pizzas.", ImageUrl = "https://img.freepik.com/premium-vector/pizza-logo-design_9845-319.jpg", DeliveryTimeRange = "25-35 min", PreparationTimeMinutes = 15, DeliveryFee = 3.50m, Rating = 4.6, Category = catPizza };
                    var rIndian = new Restaurant { Name = "Curry House", Description = "Spicy and authentic.", ImageUrl = "https://t4.ftcdn.net/jpg/03/51/97/81/360_F_351978182_aW6d4p1a2Xp4dd9q9a9b9a.jpg", DeliveryTimeRange = "35-45 min", PreparationTimeMinutes = 25, DeliveryFee = 3.00m, Rating = 4.5, Category = catIndian };

                    context.Restaurants.AddRange(rMcDo, rKFC, rTacos, rSushi, rPizza, rIndian);
                    await context.SaveChangesAsync();

                    // Products (Sample)
                    var products = new List<Product>
                    {
                        // McDo
                        new Product { Name = "Big Mac", Price = 5.50m, Description = "Le légendaire burger.", ImageUrl = "https://s7d1.scene7.com/is/image/mcdonalds/t-mcdonalds-Big-Mac-1:1-3-product-tile-desktop?wid=829&hei=515&dpr=off", Restaurant = rMcDo, Type = ProductType.Individual, IsPopular = true },
                        new Product { Name = "Best Of Big Mac", Price = 9.50m, Description = "Menu avec frites et boisson.", ImageUrl = "https://s7d1.scene7.com/is/image/mcdonalds/h-mcdonalds-Best-Of-Big-Mac:1-3-product-tile-desktop?wid=829&hei=515&dpr=off", Restaurant = rMcDo, Type = ProductType.Menu, IsPopular = true },
                        new Product { Name = "McNuggets x9", Price = 6.20m, Description = "9 morceaux de poulet.", ImageUrl = "https://s7d1.scene7.com/is/image/mcdonalds/t-mcdonalds-Chicken-McNuggets-9:1-3-product-tile-desktop?wid=829&hei=515&dpr=off", Restaurant = rMcDo, Type = ProductType.Side },
                        
                        // KFC
                        new Product { Name = "Bucket Tenders 20", Price = 22.00m, Description = "20 Tenders à partager.", ImageUrl = "https://kfc.fr/assets/images/products/bucket-tenders.png", Restaurant = rKFC, Type = ProductType.Menu, IsPopular = true },
                        new Product { Name = "Colonel Original", Price = 6.95m, Description = "Burger au poulet pané.", ImageUrl = "https://kfc.fr/assets/images/products/colonel-original.png", Restaurant = rKFC, Type = ProductType.Individual },

                        // O'Tacos
                        new Product { Name = "Tacos L", Price = 8.00m, Description = "2 Viandes au choix.", Restaurant = rTacos, Type = ProductType.Individual, IsPopular = true },
                        new Product { Name = "Tacos XL", Price = 11.00m, Description = "3 Viandes au choix + frites.", Restaurant = rTacos, Type = ProductType.Individual },

                        // Sushi
                        new Product { Name = "Sushi Set 12pc", Price = 15.00m, Description = "Assortiment Saumon/Thon.", Restaurant = rSushi, Type = ProductType.Menu, IsPopular=true },

                        // Pizza
                        new Product { Name = "Margherita", Price = 10.00m, Description = "Tomate, Mozzarella, Basilic.", Restaurant = rPizza, Type = ProductType.Individual, IsPopular = true },
                        new Product { Name = "Pepperoni", Price = 12.00m, Description = "Tomate, Mozza, Pepperoni.", Restaurant = rPizza, Type = ProductType.Individual },

                         // Indian
                        new Product { Name = "Chicken Tikka Masala", Price = 13.50m, Description = "Poulet, sauce crémeuse épicée.", Restaurant = rIndian, Type = ProductType.Individual, IsPopular = true },
                        new Product { Name = "Cheese Naan", Price = 3.50m, Description = "Pain au fromage.", Restaurant = rIndian, Type = ProductType.Side }
                    };

                    context.Products.AddRange(products);
                    await context.SaveChangesAsync();

                    // Promotions
                    context.Promotions.AddRange(
                        new Promotion { Code = "BIENVENUE", DiscountPercentage = 20, Description = "20% sur votre première commande !", ImageUrl = "intro.jpg", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddMonths(1) },
                         new Promotion { Code = "FREEDELIVERY", DiscountPercentage = 0, Description = "Livraison offerte dès 20€ !", ImageUrl = "delivery.jpg", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(7) }
                    );
                    await context.SaveChangesAsync();

                    // Points for Test Users
                    var adminUser = await userManager.FindByEmailAsync("admin@glovo.com");
                    var courierUser = await userManager.FindByEmailAsync("courier@glovo.com");
                    
                    if (adminUser != null) adminUser.LoyaltyPoints = 500;
                    if (courierUser != null) courierUser.LoyaltyPoints = 250;
                    await context.SaveChangesAsync();

                    // Seed Sample Reviews for McDo
                    context.Reviews.AddRange(
                        new Review { UserId = adminUser?.Id ?? "", RestaurantId = rMcDo.Id, Rating = 5, Comment = "Le Big Mac était incroyablement frais !", CreatedAt = DateTime.UtcNow.AddDays(-2) },
                        new Review { UserId = adminUser?.Id ?? "", RestaurantId = rMcDo.Id, Rating = 4, Comment = "Livraison rapide, mais j'aurais aimé plus de frites.", CreatedAt = DateTime.UtcNow.AddDays(-1) }
                    );
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
