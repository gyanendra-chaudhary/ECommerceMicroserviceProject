using ProductApi.Domain.Entities;

namespace ProductApi.Application.DTOs.Conversions
{
    public static class ProductConversion
    {
        public static Product ToEntity(ProductDto product) => new()
        {
            Id = product.Id,
            Name = product.Name,
            Quantity = product.Quantity,
            Price = product.Price
        };

        public static (ProductDto?, IEnumerable<ProductDto?>) FromEntity(Product product, IEnumerable<Product>? products)
        {
            // return single
            if (product is not null || products is null)
            {
                var singleProduct = new ProductDto(product!.Id, product.Name!, product.Quantity, product.Price);
                return (singleProduct, null);
            }
            if (products is not null || product is null)
            {
                var _products = products!.Select(p => new ProductDto(p.Id, p.Name, p.Quantity, p.Price)).ToList();
                return (null, _products);
            }
        }
    }
}
