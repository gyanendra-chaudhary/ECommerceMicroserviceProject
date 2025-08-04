using ECommerce.SharedLibrary.Logs;
using ECommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Response> DeleteAsync(Product entity)
        {
            try
            {
                var existingProduct = await GetByIdAsync(entity.Id);
                if (existingProduct is null)
                    return new Response(false, $"Product with the id {entity.Id} does not exist");

                context.Products.Remove(existingProduct);
                await context.SaveChangesAsync();
                return new Response(true, $"Product with the id {entity.Id} deleted successfully");
            }
            catch (Exception ex)
            {
                // Log the origional exception
                LogException.LogExceptions(ex);
                // display scary-free message to the client
                return new Response(false, "Error occured deleting product");


            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                var products = await context.Products.AsNoTracking().ToListAsync();
                return products is not null && products.Any() ? products : null!;
            }
            catch (Exception ex)
            {
                // Log origional Error
                LogException.LogExceptions(ex);
                // display scary-free message to the client
                throw new InvalidOperationException("Error occured retrieving products.");
            }
        }

        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
            try
            {
                var product = await context.Products.Where(predicate).FirstOrDefaultAsync();
                return product is not null ? product : null!;
            }
            catch (Exception ex)
            {
                // Log origional exception
                LogException.LogExceptions(ex);

                // display scary-free message to the client
                throw new InvalidOperationException("Error occured retrieving product by predicate");
            }
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            try
            {
                var product = await context.Products.FindAsync(id);
                return product is not null ? product : null;
            }
            catch (Exception ex)
            {
                // Log Origional Exception
                LogException.LogExceptions(ex);
                // display scary-free message to the client
                throw new Exception("Error occured retrieving product by id");

            }
        }

        public async Task<Response> UpdateAsync(Product entity)
        {
            try
            {
                var existingProduct = await GetByIdAsync(entity.Id);
                if (existingProduct is null)
                    return new Response(false, $"Product with the id {entity.Name} does not exist");
                context.Entry(existingProduct).State = EntityState.Detached;
                context.Products.Update(entity);
                await context.SaveChangesAsync();
                return new Response(true, $"Product with the id {entity.Name} updated successfully");
            }
            catch (Exception ex)
            {
                // Log the origional exception
                LogException.LogExceptions(ex);
                // display scary-free message to the client
                return new Response(false, "Error occured updating product");
            }
        }
    }
}
