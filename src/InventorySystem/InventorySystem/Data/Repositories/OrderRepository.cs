using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using InventorySystem.Core.Models;

namespace InventorySystem.Data.Repositories {
    public class OrderRepository {
        private readonly InventoryDbContext _context;

        public OrderRepository() {
            _context = new InventoryDbContext();
        }

        //  Handle - Search, Filter, and Pagination
        public (List<Order> Orders, int TotalCount) GetOrders(
            string searchOrderId,
            string dateFilter,
            int pageNumber,
            int pageSize) {
            var query = _context.Orders
                                .Include(o => o.OrderItems) // Loads the list of items
                                .ThenInclude(i => i.Product) // Loads the product Name inside the item
                                .AsNoTracking();

            // Search by id
            if (int.TryParse(searchOrderId, out int id)) {
                query = query.Where(o => o.Id == id);
            }

            // Filter by date
            DateTime today = DateTime.Today;
            if (dateFilter == "Today") {
                query = query.Where(o => o.OrderDate >= today);
            }
            else if (dateFilter == "Last 7 Days") {
                query = query.Where(o => o.OrderDate >= today.AddDays(-7));
            }
            else if (dateFilter == "Last 30 Days") {
                query = query.Where(o => o.OrderDate >= today.AddDays(-30));
            }

            // Pagination and sorting - newest first
            int totalCount = query.Count();

            var list = query.OrderByDescending(o => o.OrderDate)
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

            return (list, totalCount);
        }
    }
}