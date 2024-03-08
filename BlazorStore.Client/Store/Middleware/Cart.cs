using BlazorStore.Client.AppState.Cart;
using Fluxor;
namespace BlazorStore.Client.AppState.FluxorMiddleware
{
    public class Cart : Middleware
    {
        private readonly IDispatcher _dispatcher;
        private readonly IState<CartState> _cartState;

        public Cart(IDispatcher dispatcher, IState<CartState> cartState)
        {
            _dispatcher = dispatcher;
            _cartState = cartState;
        }

        public override void AfterInitializeAllMiddlewares()
        {
            _dispatcher.Dispatch(new CartInitilized());
        }

        public override void AfterDispatch(object action)
        {
            var actionName = action.GetType().Name;
            if (actionName == nameof(AppState.Cart.ItemUpserted) || actionName == nameof(AppState.Cart.ItemRemoved))
            {
                _dispatcher.Dispatch(new CartPersisted());
            }
        }
    }
}