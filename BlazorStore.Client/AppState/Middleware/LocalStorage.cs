using BlazorStore.Client.AppState.Cart;
using Fluxor;
namespace BlazorStore.Client.AppState.FluxorMiddleware
{
    public class LocalStorage : Middleware
    {
        private readonly IDispatcher _dispatcher;

        public LocalStorage(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
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