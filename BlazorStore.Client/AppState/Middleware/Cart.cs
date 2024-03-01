using BlazorStore.Client.AppState.Cart;
using Fluxor;
namespace BlazorStore.Client.AppState.FluxorMiddleware
{
    public class Cart : Middleware
    {
        private readonly IDispatcher _dispatcher;

        public Cart(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public override void AfterInitializeAllMiddlewares()
        {
            _dispatcher.Dispatch(new CartInitilized());
        }

        public override void AfterDispatch(object action)
        {
            var actionName = action.GetType().Name;
            if (actionName == "AddedToCart" || actionName == "RemovedFromCart")
            {
                _dispatcher.Dispatch(new CartPersisted());
            }
        }
    }
}