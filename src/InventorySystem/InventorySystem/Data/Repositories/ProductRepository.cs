using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using InventorySystem.Core.Models;
using InventorySystem.Data;

namespace InventorySystem.Data.Repositories {
    public class ProductRepository {
        private readonly InventoryDbContext _context;

        public ProductRepository() {
            _context = new InventoryDbContext();
        }

        // Read - Fetch every product to show in big list
        public List<Product> GetAll() {
            return _context.Products.AsNoTracking().ToList();
        }

        // Editing - useful, if want to change specific item
        public Product? GetById(int id) {
            return _context.Products.FirstOrDefault(p => p.Id == id);
        }

        // Add
        public void Add(Product product) {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        // Update
        public void Update(Product product) {
            var existingProduct = _context.Products.Find(product.Id);
            if (existingProduct != null) {
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Category = product.Category;
                existingProduct.Brand = product.Brand;
                existingProduct.Stock = product.Stock;
                existingProduct.ImageUrl = product.ImageUrl;

                _context.SaveChanges();
            }
        }

        // Delete
        public void Delete(int id) {
            var product = _context.Products.Find(id);
            if (product != null) {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }

        // Calculate total pages
        public int GetTotalCount() {
            return _context.Products.Count();
        }

        // Get category
        public List<string> GetCategories() {
            return _context.Products
                           .Select(p => p.Category)
                           .Distinct()
                           .OrderBy(c => c)
                           .ToList();
        }

        // Handle - Search, Filter, and Pagination
        public (List<Product> Products, int TotalCount) GetProducts(
            string searchTerm,
            string category,
            int pageNumber,
            int pageSize) {
            
            var query = _context.Products.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm)) {
                // Find products where name or brand contains the search text
                query = query.Where(p => p.Name.Contains(searchTerm) ||
                                         p.Brand.Contains(searchTerm));
            }

            // Apply Category Filter - if user selected one
            if (!string.IsNullOrWhiteSpace(category) && category != "All Categories") {
                query = query.Where(p => p.Category == category);
            }

            // If we have 1000 items but only 5 match "Samsung", this will be 5.
            int totalCount = query.Count();

            // Pagination
            var items = query.OrderByDescending(p => p.Id)
                             .Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize)
                             .ToList();

            return (items, totalCount);
        }
    }
}