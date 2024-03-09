using BlazorStore.Dto;
using BlazorStore.Service.IService;
using Stripe.Checkout;

namespace BlazorStore.Service
{
    public class StripeService : IPaymentService<Session>
    {
        private readonly IConfiguration _config;

        public StripeService(IConfiguration config)
        {
            _config = config;
        }

        public Session Checkout(OrderDto Order)
        {
            var baseUrl = _config.GetSection("AppUrls")["BaseServerUrl"];

            var options = new SessionCreateOptions
            {
                SuccessUrl = $"{baseUrl}/orderconfirmation?entityId={Order.OrderHeader.Id}",
                CancelUrl = $"{baseUrl}/summary",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in Order.OrderDetails)
            {
                options.LineItems.Add(
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.ProductName
                            }
                        },
                        Quantity = item.Count
                    }
                );
            }

            var service = new SessionService();
            var session = service.Create(options);

            return session;
        }
    }
}