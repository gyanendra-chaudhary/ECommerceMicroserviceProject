using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using Polly.Registry;
using System.Net.Http.Json;

namespace OrderApi.Application.Services
{
    public class OrderService(IOrder orderInterface, HttpClient httpClient, ResiliencePipelineProvider<string> resiliencePipeline) : IOrderService
    {
        // GET PRODUCT
        public async Task<ProductDTO> GetProduct(int productId)
        {
            // Call product api using HttpClient
            // Redirect this call to the API Gateway since product api is not response to outsiders
            var getProduct = await httpClient.GetAsync($"/api/products/{productId}");
            if (!getProduct.IsSuccessStatusCode)
                return null!;

            var product = await getProduct.Content.ReadFromJsonAsync<ProductDTO>();
            return product!;
        }
        // GET USER
        public async Task<AppUserDTO> GetUser(int userId)
        {
            // Call user api using HttpClient
            // Redirect this call to the API Gateway since user api is not response to outsiders
            var getUser = await httpClient.GetAsync($"/api/users/{userId}");
            if (!getUser.IsSuccessStatusCode)
                return null!;
            var user = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
            return user!;
        }

        // GET ORDER DETAILS BY ID
        public async Task<OrderDetailsDTO> GetOrderDetails(int orderId)
        {
            // Prepare order
            var order = await orderInterface.GetByIdAsync(orderId);
            if (order == null || order.Id <= 0)
                return null!;

            // Get retry pipeline
            var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");


            // Prepare product
            var productDto = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));

            // Prepare Client
            var appUserDto = await retryPipeline.ExecuteAsync(async token => await GetUser(order.ClientId));


            // Populate order Details
            return new OrderDetailsDTO(order.Id,
                                       productDto.Id,
                                       appUserDto.Id,
                                       appUserDto.Name,
                                       appUserDto.Email,
                                       appUserDto.Address,
                                       appUserDto.TelephoneNumber,
                                       productDto.Name,
                                       order.PurchaseQuantity,
                                       productDto.Price,
                                       productDto.Price * order.PurchaseQuantity,
                                       order.OrderdDate);

        }


        // GET ORDERS BY CLIENT ID
        public async Task<IEnumerable<OrderDTO>> GetOrdersByClientId(int clientId)
        {
            // Get all Client's orders

            var orders = await orderInterface.GetOrdersAsync(o => o.ClientId == clientId);
            if (!orders.Any())
                return null!;

            // Convert from entity to DTO
            var (_, _orders)= OrderConversion.FromEntity(null, orders);
            return _orders!;


        }
    }
}
