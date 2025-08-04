using ECommerce.SharedLibrary.Logs;
using ECommerce.SharedLibrary.Responses;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace ProductApi.Infrastructure.Repositories
{
    public class ProductRepository(ProductDbContext context) : IProduct
    {
        public async Task<Response> CreateAsync(Product entity)
        {
            try
            {
                // check if the product already exist
                var existingProduct = await GetByAsync(_ => _.Name!.Equals(entity.Name));
                if (existingProduct is not null && !string.IsNullOrEmpty(existingProduct.Name))
                    return new Response(false, $"Product with the name {entity.Name} already exists");

                // add the product to the database
                var currentEntity = context.Products.Add(entity).Entity;
                await context.SaveChangesAsync();
                if (currentEntity is not null && currentEntity.Id > 0)
                    return new Response(true, $"Product with the name {currentEntity.Name} added successfully");
                else
                    return new Response(false, $"Error occured while adding {entity.Name}");
            }
            catch (Exception ex)
            {
                // Log the origional exception
                LogException.LogExceptions(ex);
                // displayy scary-free message to the client
                return new Response(false, "Error occured adding new product");
            }
        }

        public Task<Response> DeleteAsync(Product entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<Product?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Response> UpdateAsync(Product entity)
        {
            throw new NotImplementedException();
        }
    }
}
