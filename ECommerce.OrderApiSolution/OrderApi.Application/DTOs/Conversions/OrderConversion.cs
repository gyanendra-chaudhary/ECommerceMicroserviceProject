using OrderApi.Domain.Entities;

namespace OrderApi.Application.DTOs.Conversions
{
    public static class OrderConversion
    {
        public static Order ToEntity(OrderDTO orderDto) => new ()
        {
            Id = orderDto.Id,
            ClientId = orderDto.ClientId,
            ProductId = orderDto.ProductId,
            OrderdDate = orderDto.OrderdDate,
            PurchaseQuantity = orderDto.PurchaseQuantity
        };

        public static (OrderDTO?, IEnumerable<OrderDTO>?) FromEntity(Order? order, IEnumerable<Order>? orders)
        {
            if (order is null && orders is null)
            {
                return (null, null);
            }
            if (order is not null || orders is null)
            {
                return (new OrderDTO(order!.Id, order.ProductId, order.ClientId, order.PurchaseQuantity, order.OrderdDate), null);
            }
            if(orders is not null && order is null)
            {
                return (null, orders.Select(o => new OrderDTO(o.Id, o.ProductId, o.ClientId, o.PurchaseQuantity, o.OrderdDate)));
            }
            return (null, null);        
        }
    }
}
