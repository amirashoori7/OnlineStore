using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsStore.Models;
using System.Linq;

namespace SportsStore.Controllers {

    public class SeedController : Controller {
        private DataContext context;

        public SeedController(DataContext ctx) => context = ctx;

        public IActionResult Index() {
            ViewBag.Count = context.Products.Count();
            return View(context.Products
                .Include(p => p.Category).OrderBy(p => p.Id).Take(20));
        }
            
        [HttpPost]
        public IActionResult CreateSeedData(int count) {
            ClearData();
            if (count > 0) {
                context.Database.SetCommandTimeout(System.TimeSpan.FromMinutes(10));
                context.Database
                    .ExecuteSqlCommand("DROP PROCEDURE IF EXISTS CreateSeedData");
                context.Database.ExecuteSqlCommand($@"
                    CREATE PROCEDURE CreateSeedData
	                    @RowCount decimal
                    AS
	                  BEGIN
	                  SET NOCOUNT ON
                      DECLARE @i INT = 1;
	                  DECLARE @catId BIGINT;
	                  DECLARE @CatCount INT = @RowCount / 10;
	                  DECLARE @pprice DECIMAL(5,2);
	                  DECLARE @rprice DECIMAL(5,2);
	                  BEGIN TRANSACTION
		                WHILE @i <= @CatCount
			              BEGIN
				            INSERT INTO Categories (Name, Description)
				            VALUES (CONCAT('Category-', @i), 
                                             'Test Data Category');
				            SET @catId = SCOPE_IDENTITY();
				            DECLARE @j INT = 1;
				            WHILE @j <= 10
					        BEGIN
						   SET @pprice = RAND()*(500-5+1);
						   SET @rprice = (RAND() * @pprice) 
                                                   + @pprice;
						   INSERT INTO Products (Name, CategoryId, 
                                                    PurchasePrice, RetailPrice)
						   VALUES (CONCAT('Product', @i, '-', @j), 
                                                 @catId, @pprice, @rprice)
						   SET @j = @j + 1
					          END
		                    SET @i = @i + 1
		                    END
	                    COMMIT
                    END");
                context.Database.BeginTransaction();
                context.Database
                    .ExecuteSqlCommand($"EXEC CreateSeedData @RowCount = {count}");
                context.Database.CommitTransaction();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ClearData() {
            context.Database.SetCommandTimeout(System.TimeSpan.FromMinutes(10));
            context.Database.BeginTransaction();
            context.Database.ExecuteSqlCommand("DELETE FROM Orders");
            context.Database.ExecuteSqlCommand("DELETE FROM Categories");
            context.Database.CommitTransaction();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult CreateProductionData()
        {
            ClearData();

            context.Categories.AddRange(new Category[] {
                new Category
                {
                    Name = "Engine Parts",
                    Description = "Engine Building Blocks",
                    Products = new Product[]
                    {
                        new Product
                        {
                            Name="Cam Shaft", Description="Toyota Crolla Cam Shaft",
                            PurchasePrice=700, RetailPrice=840
                        },
                        new Product
                        {
                            Name="Crank Shaft", Description="Toyota Crolla Crank Shaft",
                            PurchasePrice=700, RetailPrice=840
                        },
                        new Product
                        {
                            Name="Valve", Description="BMW 330 Valves",
                            PurchasePrice=300, RetailPrice=330
                        },
                    }


                },
                new Category
                {
                    Name="GearBox", Description="AT GearBox Parts",
                    Products = new Product []
                    {
                        new Product
                        {
                            Name="AT Control Unit", Description="Mazda 3 new 2010 AT GearBox CU",
                            PurchasePrice=900, RetailPrice=1100
                        },
                        new Product
                        {
                            Name="Full AT GearBox", Description="Mercedes C200 Full AT GearBox",
                            PurchasePrice=10000, RetailPrice=12000
                        },
                        new Product
                        {
                            Name="Full AT GearBox", Description="Nissan GTR Full AT GearBox",
                            PurchasePrice=11000, RetailPrice=13000
                        }
                    }
                },
                new Category
                {
                    Name="Intake", Description="Intake System Parts",
                    Products =new Product []
                    {
                        new Product
                        {
                            Name="K&N Standard Air Filter", Description="Audi A9 K&N Standard LifeLong Filters",
                            PurchasePrice=110, RetailPrice=125
                        },
                        new Product
                        {
                            Name="K&N Standard Air Filter", Description="Mazda 3 K&N Standard LifeLong Filters",
                            PurchasePrice=110, RetailPrice=125
                        },
                        new Product
                        {
                            Name="K&N Standard Air Filter", Description="Mazda 2 K&N Standard LifeLong Filters",
                            PurchasePrice=110, RetailPrice=125
                        },
                        new Product
                        {
                            Name="K&N Standard Air Filter", Description="Mitsubishi Lancer EVO K&N Standard LifeLong Filters",
                            PurchasePrice=110, RetailPrice=125
                        },
                        new Product
                        {
                            Name="K&N Standard Air Filter", Description="Mitsubishi Lancer K&N Standard LifeLong Filters",
                            PurchasePrice=110, RetailPrice=125
                        }
                    }
                }
            });

            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
