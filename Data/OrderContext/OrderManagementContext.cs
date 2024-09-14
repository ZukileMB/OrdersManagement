using Microsoft.EntityFrameworkCore;
using OrdersManagement.Models;

namespace OrdersManagement.Data.OrderContext
{
    public class OrderManagementContext : DbContext
    {
        public OrderManagementContext(DbContextOptions<OrderManagementContext> contextOptions) : base(contextOptions)
        {

        }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Customer> customers { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<OrderHeaderStatus> orderHeaders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .HasKey(o => o.OrderNumber);

            modelBuilder.Entity<OrderLine>()
                .HasKey(ol => ol.LineNumber);

            modelBuilder.Entity<OrderLine>()
                .HasOne(ol => ol.Order)
                .WithMany(o => o.OrderLines)
                .HasForeignKey(ol => ol.OrderNumber);
        }
    }
}
