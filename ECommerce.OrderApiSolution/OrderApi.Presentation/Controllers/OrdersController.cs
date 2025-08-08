using ECommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;

namespace OrderApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController(IOrder orderInterface, IOrderService orderService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
        {
            var orders = await orderInterface.GetAllAsync();
            if (!orders.Any())
                return NotFound("No orders found.");

            var (_, list) = OrderConversion.FromEntity(null, orders);
            if (list is null || !list.Any())
                return NotFound("No orders found.");
            return Ok(list);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            var order = await orderInterface.GetByIdAsync(id);
            if (order is null)
                return NotFound($"Order with ID {id} not found.");
            var (_order, _) = OrderConversion.FromEntity(order, null);
            return Ok(_order);
        }

        [HttpGet("client/{clientId:int}")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetClientOrders(int clientId)
        {
            if (clientId <= 0)
                return BadRequest("Client ID must be greater than zero.");
            var orders = await orderService.GetOrdersByClientId(clientId);
            if (!orders.Any())
                return NotFound($"No orders found for client with ID {clientId}.");
            return Ok(orders);
        }

        [HttpGet("details/{orderId:int}")]
        public async Task<ActionResult<OrderDetailsDTO>> GetOrderDetails(int orderId)
        {
            if (orderId <= 0)
                return BadRequest("Order ID must be greater than zero.");
            var orderDetails = await orderService.GetOrderDetails(orderId);
            return orderDetails.OrderId < 0 ? NotFound($"Order details for ID {orderId} not found.") : Ok(orderDetails);
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateOrder(OrderDTO orderDTO)
        {
            // Create model state if all data annotations are valid
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = OrderConversion.ToEntity(orderDTO);
            var result = await orderInterface.CreateAsync(order);
            return result.Flag ? Ok(result) : BadRequest(result);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateOrder(OrderDTO orderDTO)
        {
            // Create model state if all data annotations are valid
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var order = OrderConversion.ToEntity(orderDTO);
            var result = await orderInterface.UpdateAsync(order);
            return result.Flag ? Ok(result) : BadRequest(result);
        }

        [HttpDelete]
        public async Task<ActionResult<Response>> DeleteOrder(OrderDTO orderDTO)
        {
            // Convert from dto to entity
            var order = OrderConversion.ToEntity(orderDTO);
            var result = await orderInterface.DeleteAsync(order);
            return result.Flag ? Ok(result) : BadRequest(result);
        }
    }
}
