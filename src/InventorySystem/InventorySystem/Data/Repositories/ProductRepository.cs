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

        // Skip - jumps over the previous pages. 
        // Take - grabs the next 10 items.
        public List<Product> GetProductsByPage(int pageNumber, int pageSize) {
            return _context.Products
                           .AsNoTracking()
                           .OrderBy(p => p.Id) // SQL needs an order to page correctly
                           .Skip((pageNumber - 1) * pageSize)
                           .Take(pageSize)
                           .ToList();
        }
    }
}