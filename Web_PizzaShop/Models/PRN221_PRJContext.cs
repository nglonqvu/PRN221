using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Web_PizzaShop.Models
{
    public partial class PRN221_PRJContext : DbContext
    {
        public PRN221_PRJContext()
        {
        }

        public PRN221_PRJContext(DbContextOptions<PRN221_PRJContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CakeBasis> CakeBases { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<ContractDetail> ContractDetails { get; set; } = null!;
        public virtual DbSet<Ingredient> Ingredients { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<Pizza> Pizzas { get; set; } = null!;
        public virtual DbSet<PizzaIngredient> PizzaIngredients { get; set; } = null!;
        public virtual DbSet<PizzaOption> PizzaOptions { get; set; } = null!;
        public virtual DbSet<PizzaOrder> PizzaOrders { get; set; } = null!;
        public virtual DbSet<Review> Reviews { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; } = null!;
        public virtual DbSet<ShoppingCartItem> ShoppingCartItems { get; set; } = null!;
        public virtual DbSet<Size> Sizes { get; set; } = null!;
        public virtual DbSet<Supplier> Suppliers { get; set; } = null!;
        public virtual DbSet<SupplierContract> SupplierContracts { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserToken> UserTokens { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
            optionsBuilder.UseSqlServer(config.GetConnectionString("PRN221_DB"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CakeBasis>(entity =>
            {
                entity.Property(e => e.CakeBase).HasMaxLength(250);

                entity.Property(e => e.PriceBase).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<ContractDetail>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Contract_details");

                entity.Property(e => e.ContractId).HasColumnName("Contract_Id");

                entity.Property(e => e.IngredientId).HasColumnName("Ingredient_Id");

                entity.HasOne(d => d.Contract)
                    .WithMany()
                    .HasForeignKey(d => d.ContractId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Contract_details_SupplierContract");

                entity.HasOne(d => d.Ingredient)
                    .WithMany()
                    .HasForeignKey(d => d.IngredientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Contract_details_Ingredients");
            });

            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasIndex(e => e.UserId, "IX_Orders_UserId");

                entity.Property(e => e.AddressLine1).HasMaxLength(100);

                entity.Property(e => e.City).HasMaxLength(50);

                entity.Property(e => e.Country).HasMaxLength(50);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("date")
                    .HasColumnName("Created_at");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("date")
                    .HasColumnName("Deleted_at");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.OrderTotal).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PhoneNumber).HasMaxLength(25);

                entity.Property(e => e.State).HasMaxLength(10);

                entity.Property(e => e.ZipCode).HasMaxLength(10);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Orders__UserId__5441852A");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasIndex(e => e.OrderId, "IX_OrderDetails_OrderId");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OrderDeta__Order__52593CB8");
            });

            modelBuilder.Entity<Pizza>(entity =>
            {
                entity.HasIndex(e => e.CategoriesId, "IX_Pizzas_CategoriesId");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("date")
                    .HasColumnName("Created_at");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("date")
                    .HasColumnName("Deleted_at");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Categories)
                    .WithMany(p => p.Pizzas)
                    .HasForeignKey(d => d.CategoriesId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Pizzas__Categori__571DF1D5");
            });

            modelBuilder.Entity<PizzaIngredient>(entity =>
            {
                entity.HasIndex(e => e.IngredientId, "IX_PizzaIngredients_IngredientId");

                entity.HasIndex(e => e.PizzaId, "IX_PizzaIngredients_PizzaId");

                entity.HasOne(d => d.Ingredient)
                    .WithMany(p => p.PizzaIngredients)
                    .HasForeignKey(d => d.IngredientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PizzaIngr__Ingre__5535A963");

                entity.HasOne(d => d.Pizza)
                    .WithMany(p => p.PizzaIngredients)
                    .HasForeignKey(d => d.PizzaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PizzaIngr__Pizza__5629CD9C");
            });

            modelBuilder.Entity<PizzaOption>(entity =>
            {
                entity.HasKey(e => e.OptionId);

                entity.ToTable("Pizza_Option");

                entity.HasOne(d => d.CakeBase)
                    .WithMany(p => p.PizzaOptions)
                    .HasForeignKey(d => d.CakeBaseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Pizza_Option_CakeBases");

                entity.HasOne(d => d.Pizza)
                    .WithMany(p => p.PizzaOptions)
                    .HasForeignKey(d => d.PizzaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Pizza_Option_Pizzas");

                entity.HasOne(d => d.Size)
                    .WithMany(p => p.PizzaOptions)
                    .HasForeignKey(d => d.SizeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Pizza_Option_Sizes");
            });

            modelBuilder.Entity<PizzaOrder>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Pizza_Order");

                entity.Property(e => e.Note).HasMaxLength(250);

                entity.HasOne(d => d.CakeBase)
                    .WithMany()
                    .HasForeignKey(d => d.CakeBaseId)
                    .HasConstraintName("FK_Pizza_Order_CakeBases");

                entity.HasOne(d => d.Order)
                    .WithMany()
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Pizza_Order_Orders");

                entity.HasOne(d => d.Pizza)
                    .WithMany()
                    .HasForeignKey(d => d.PizzaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Pizza_Order_Pizzas");

                entity.HasOne(d => d.Size)
                    .WithMany()
                    .HasForeignKey(d => d.SizeId)
                    .HasConstraintName("FK_Pizza_Order_Sizes");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasIndex(e => e.PizzaId, "IX_Reviews_PizzaId");

                entity.HasIndex(e => e.UserId, "IX_Reviews_UserId");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Title).HasMaxLength(100);

                entity.HasOne(d => d.Pizza)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.PizzaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Reviews__PizzaId__59063A47");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Reviews__UserId__59FA5E80");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(256);
            });

            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                entity.HasIndex(e => e.UserId, "IX_ShoppingCart_UserId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ShoppingCarts)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ShoppingC__UserI__5BE2A6F2");
            });

            modelBuilder.Entity<ShoppingCartItem>(entity =>
            {
                entity.HasOne(d => d.ShoppingCart)
                    .WithMany(p => p.ShoppingCartItems)
                    .HasForeignKey(d => d.ShoppingCartId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ShoppingCartItems_ShoppingCarts");
            });

            modelBuilder.Entity<Size>(entity =>
            {
                entity.Property(e => e.PriceSize).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Size1)
                    .HasMaxLength(50)
                    .HasColumnName("Size");
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("Supplier");
            });

            modelBuilder.Entity<SupplierContract>(entity =>
            {
                entity.ToTable("SupplierContract");

                entity.HasIndex(e => e.IngredientId, "IX_SupplierContract_IngredientId");

                entity.HasIndex(e => e.SupplierId, "IX_SupplierContract_SupplierId");

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.SupplierContracts)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("FK__SupplierC__Suppl__5DCAEF64");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.CreatedAt)
                    .HasColumnType("date")
                    .HasColumnName("Created_at");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.HasMany(d => d.Roles)
                    .WithMany(p => p.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "UserRole",
                        l => l.HasOne<Role>().WithMany().HasForeignKey("RoleId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__UserRoles__RoleI__5EBF139D"),
                        r => r.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__UserRoles__UserI__5FB337D6"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId").HasName("PK__UserRole__AF2760ADC2AAD719");

                            j.ToTable("UserRoles");

                            j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                        });
            });

            modelBuilder.Entity<UserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name })
                    .HasName("PK__UserToke__8CC49841610AC1CC");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserTokens)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UserToken__UserI__60A75C0F");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
