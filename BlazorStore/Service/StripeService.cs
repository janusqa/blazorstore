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

        public Session Checkout(OrderDto order)
        {
            var baseUrl = _config.GetSection("AppUrls")["BaseServerUrl"];

            var options = new SessionCreateOptions
            {
                SuccessUrl = $"{baseUrl}/orderconfirmation/{order.OrderHeader.Id}",
                CancelUrl = $"{baseUrl}/summary",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in order.OrderDetails)
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

        public bool Cancel(OrderHeaderDto order)
        {
            var options = new Stripe.RefundCreateOptions
            {
                Reason = Stripe.RefundReasons.RequestedByCustomer,
                PaymentIntent = order.PaymentIntentId
            };

            try
            {
                var service = new Stripe.RefundService();
                Stripe.Refund refund = service.Create(options);
                return refund.Status.Equals("succeeded", StringComparison.CurrentCultureIgnoreCase);
            }
            catch (Stripe.StripeException)
            {
                return false;
            }
        }

        public Session GetSession(OrderHeaderDto orderHeader)
        {
            var service = new SessionService();
            var session = service.Get(orderHeader.SessionId);
            return session;
        }

    }
}