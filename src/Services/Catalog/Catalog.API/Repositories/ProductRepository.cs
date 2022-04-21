using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.API.Repositories
{
    public class ProductRepository: IProductRepository
    {
        private readonly ICatalogContext _context;

        public ProductRepository(ICatalogContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task CreateProduct(Product product)
        {
            await _context.Products.InsertOneAsync(product);
        }

        public async Task<bool> DeleteProduct(string productId)
        {
            FilterDefinition<Product> filterDefinition = Builders<Product>.Filter.Eq(p => p.Id, productId);
            DeleteResult deleteResult = await _context.Products.DeleteOneAsync(filterDefinition);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

        public async Task<Product> GetProduct(string productId)
        {
            return await _context.Products.Find(p => p.Id == productId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProductByName(string productName)
        {
            FilterDefinition<Product> filterDefinition = Builders<Product>.Filter.Eq(p => p.Name, productName);
            return await _context.Products.Find(filterDefinition).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _context.Products.Find(p => true).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategory(string category)
        {
            FilterDefinition<Product> filterDefinition = Builders<Product>.Filter.Eq(p => p.Category, category);
            return await _context.Products.Find(filterDefinition).ToListAsync();
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            var updateRersult = await _context.Products.ReplaceOneAsync(filter: g => g.Id == product.Id, replacement: product);
            return updateRersult.IsAcknowledged && updateRersult.ModifiedCount > 0;
        }
    }
}
