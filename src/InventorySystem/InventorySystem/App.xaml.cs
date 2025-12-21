using System.Configuration;
using System.Linq;
using System.Windows;
using InventorySystem.Data;        // Need this to see the Database
using InventorySystem.Core.Models; // Need this to see 'User'
using InventorySystem.Services;    // Need this to see 'PasswordHelper'

namespace InventorySystem {
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            // Connect to database
            using (var context = new InventoryDbContext()) {
                context.Database.EnsureCreated();

                // Check if empty
                if (!context.Users.Any()) {
                    var admin = new User
                    {
                        Username = "admin",
                        PasswordHash = PasswordHelper.HashPassword("1234"),
                        Role = "Admin"
                    };

                    context.Users.Add(admin);
                    context.SaveChanges();
                }
            }

            using (var context = new InventoryDbContext()) {
                context.Database.EnsureCreated();
               
                SeedProducts(context);
            }
        }

        private void SeedProducts(InventoryDbContext context) {
            if (context.Products.Any())
                return;

            var products = new List<Product>();

            // Laptops
            products.Add(new Product { Name = "Apple MacBook Pro M4 Pro chip", Price = 249_900, Category = "Laptop", Brand = "Apple", Stock = 45, ImageUrl = "https://encrypted-tbn3.gstatic.com/shopping?q=tbn:ANd9GcQ53BRsNrzX1XXbY5KS4ojYJU7AyNp0yw6dab7uBFVbvXesG1YKZZTpYukzy4WtjT9C1eLg6QF1VVbQCRXUw36bqGPc9dz40L9RSqlz-Tn2iuLpCd6kn6mHXdo" });
            products.Add(new Product { Name = "Galaxy Book4", Price = 68_990, Category = "Laptop", Brand = "Samsung", Stock = 9, ImageUrl = "https://images.samsung.com/is/image/samsung/p6pim/in/feature/165331444/in-feature-galaxy-book-541824934?$FB_TYPE_K_JPG$" });
            products.Add(new Product { Name = "ROG Strix G16 (2025) G614", Price = 139_990, Category = "Laptop", Brand = "Rog", Stock = 12, ImageUrl = "https://dlcdnwebimgs.asus.com/gain/29CDFDE4-51D7-4190-A647-EB1A6531F9A1/w717/h525/fwebp/w273" });

            // Smartphones
            products.Add(new Product { Name = "iPhone 17 Pro", Price = 134_990, Category = "Smartphone", Brand = "Apple", Stock = 5, ImageUrl = "https://www.apple.com/in/iphone/home/images/overview/select/iphone_17pro__0s6piftg70ym_large.jpg" });
            products.Add(new Product { Name = "Galaxy S25 Ultra", Price = 129_999, Category = "Smartphone", Brand = "Samsung", Stock = 70, ImageUrl = "https://images.samsung.com/is/image/samsung/p6pim/in/2501/gallery/in-galaxy-s25-s938-sm-s938bztbins-thumb-544702948?$Q90_330_330_F_PNG$" });
            products.Add(new Product { Name = "OnePlus 15R", Price = 47_999, Category = "Smartphone", Brand = "OnePlus", Stock = 20, ImageUrl = "https://image01-in.oneplus.net/media/202512/12/864571bb797cacb4186dc3f01affc566.png?x-amz-process=image/format,webp/quality,Q_80" });

            // Components (GPU/CPU)
            products.Add(new Product { Name = "GeForce RTX 5090", Price = 2_19_000, Category = "GPU", Brand = "nvidia", Stock = 2, ImageUrl = "https://assets.nvidia.partners/images/png/RTX5090STPL_IN.png" });
            products.Add(new Product { Name = "Intel® Core™ i9-14900K", Price = 86_000, Category = "Processor", Brand = "Intel", Stock = 0, ImageUrl = "https://m.media-amazon.com/images/I/619ytLAytAL._SX679_.jpg" });

            // Accessories (Mouse, Charger, Headphones etc.)
            products.Add(new Product { Name = "MX Master 4", Price = 15_995, Category = "Accessories", Brand = "Logitech", Stock = 50, ImageUrl = "https://resource.logitech.com/w_544,h_466,ar_7:6,c_pad,q_auto,f_auto,dpr_1.0/d_transparent.gif/content/dam/logitech/en/products/mice/mx-master-4/gallery/mx-master-4-graphite-top-angle-gallery-1.png" });
            products.Add(new Product { Name = "ASTRO A50 (Gen 5)", Price = 47_499, Category = "Accessories", Brand = "Logitech", Stock = 7, ImageUrl = "https://resource.logitechg.com/w_386,ar_1.0,c_limit,f_auto,q_auto,dpr_2.0/d_transparent.gif/content/dam/gaming/en/products/a50-gen-5/astro-a50-x-gen-5-white-gallery-1.png?v=1" });
            products.Add(new Product { Name = "WH-1000XM6 Wireless Noise Cancelling Headphones", Price = 49_990, Category = "Accessories", Brand = "Sony", Stock = 25, ImageUrl = "https://sony.scene7.com/is/image/sonyglobalsolutions/WH1000XM6_Primary_image_Midnight_Blue?$primaryshotPreset$&fmt=png-alpha" });
            products.Add(new Product { Name = "WH-1000XM6 Wireless Noise Cancelling Headphones", Price = 49_990, Category = "Accessories", Brand = "Sony", Stock = 25, ImageUrl = "https://sony.scene7.com/is/image/sonyglobalsolutions/WH1000XM6_Primary_image_Midnight_Blue?$primaryshotPreset$&fmt=png-alpha" });
            products.Add(new Product { Name = "WH-1000XM6 Wireless Noise Cancelling Headphones", Price = 49_990, Category = "Accessories", Brand = "Sony", Stock = 25, ImageUrl = "https://sony.scene7.com/is/image/sonyglobalsolutions/WH1000XM6_Primary_image_Midnight_Blue?$primaryshotPreset$&fmt=png-alpha" });
            products.Add(new Product { Name = "WH-1000XM6 Wireless Noise Cancelling Headphones", Price = 49_990, Category = "Accessories", Brand = "Sony", Stock = 25, ImageUrl = "https://sony.scene7.com/is/image/sonyglobalsolutions/WH1000XM6_Primary_image_Midnight_Blue?$primaryshotPreset$&fmt=png-alpha" });
            products.Add(new Product { Name = "WH-1000XM6 Wireless Noise Cancelling Headphones", Price = 49_990, Category = "Accessories", Brand = "Sony", Stock = 25, ImageUrl = "https://sony.scene7.com/is/image/sonyglobalsolutions/WH1000XM6_Primary_image_Midnight_Blue?$primaryshotPreset$&fmt=png-alpha" });
            //products.Add(new Product { Name = "", Price = , Category = "Accessories", Brand = "", Stock = , ImageUrl = "" });
            //products.Add(new Product { Name = "", Price = , Category = "Accessories", Brand = "", Stock = , ImageUrl = "" });
            //products.Add(new Product { Name = "", Price = , Category = "Accessories", Brand = "", Stock = , ImageUrl = "" });
            //products.Add(new Product { Name = "", Price = , Category = "Accessories", Brand = "", Stock = , ImageUrl = "" });
            //products.Add(new Product { Name = "", Price = , Category = "Accessories", Brand = "", Stock = , ImageUrl = "" });
            //products.Add(new Product { Name = "", Price = , Category = "Accessories", Brand = "", Stock = , ImageUrl = "" });

            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
}