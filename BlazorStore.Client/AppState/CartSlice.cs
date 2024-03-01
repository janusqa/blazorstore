using Blazored.LocalStorage;
using BlazorStore.Common;
using BlazorStore.Dto;
using Fluxor;

namespace BlazorStore.Client.AppState.Cart
{
    // ********************
    // State
    // ********************
    [FeatureState]
    public record CartState
    {
        public Dictionary<int, CartItemDto> Cart { get; init; } = [];
        public bool Updating { get; init; } = false;
        public bool Updated { get; init; } = false;
    }

    // ********************
    // Reducers
    // ********************
    public static class Reducers
    {
        [ReducerMethod]
        public static CartState AddToCartReducer(CartState state, AddedToCart action)
        {
            var tempCart = FastDeepCloner.DeepCloner.Clone(state.Cart);
            var cartItem = FastDeepCloner.DeepCloner.Clone(action.CartItem);

            if (tempCart is not null && cartItem is not null)
            {
                var key = action.CartItem.ProductPriceId;

                if (tempCart.ContainsKey(key))
                {
                    tempCart[key] = cartItem;
                }
                else
                {
                    tempCart.TryAdd(key, cartItem);
                }

                return state with { Cart = tempCart };
            }
            else
            {
                return state;
            }
        }

        [ReducerMethod]
        public static CartState RemoveFromCartReducer(CartState state, RemovedFromCart action)
        {
            var tempCart = state.Cart.ToDictionary(c => c.Key, c => c.Value);
            tempCart.Remove(action.CartItemKey);
            return state with { Cart = tempCart };
        }

        [ReducerMethod]
        public static CartState CartUpdatingReducer(CartState state, CartUpdating action)
        {
            return state with { Updating = action.IsUpdating };
        }

        [ReducerMethod]
        public static CartState CartUpdatedReducer(CartState state, CartUpdated action)
        {
            return state with { Updated = action.IsUpdated };
        }

        [ReducerMethod]
        public static CartState CartFetchedReducer(CartState state, CartFetched action)
        {
            return state with { Cart = action.Cart, Updating = false, Updated = true };
        }
    }

    // ********************
    // Effects
    // ********************
    public class Effects
    {
        private readonly ILocalStorageService _localStorage;
        private readonly IState<CartState> _state;

        public Effects(ILocalStorageService localStorage, IState<CartState> state)
        {
            _localStorage = localStorage;
            _state = state;
        }

        [EffectMethod(typeof(CartPersisted))]
        public async Task CartPersistedReducer(IDispatcher dispatcher)
        {
            try
            {
                dispatcher.Dispatch(new CartUpdating(true));
                dispatcher.Dispatch(new CartUpdated(false));
                await _localStorage.SetItemAsync(SD.cartKey, _state.Value.Cart);
                dispatcher.Dispatch(new CartUpdated(true));
            }
            catch
            {
                dispatcher.Dispatch(new CartUpdated(false));
            }
            finally
            {
                dispatcher.Dispatch(new CartUpdating(false));
            }
        }

        [EffectMethod(typeof(CartRemoved))]
        public async Task CartRemovedReducer(IDispatcher dispatcher)
        {
            try
            {
                dispatcher.Dispatch(new CartUpdating(true));
                dispatcher.Dispatch(new CartUpdated(false));
                await _localStorage.RemoveItemAsync(SD.cartKey);
                dispatcher.Dispatch(new CartUpdated(true));
            }
            catch
            {
                dispatcher.Dispatch(new CartUpdated(false));
            }
            finally
            {
                dispatcher.Dispatch(new CartUpdating(false));
            }
        }

        [EffectMethod(typeof(CartInitilized))]
        public async Task CartInitilizedReducer(IDispatcher dispatcher)
        {
            try
            {
                dispatcher.Dispatch(new CartUpdating(true));
                dispatcher.Dispatch(new CartUpdated(false));
                var cart = await _localStorage.GetItemAsync<Dictionary<int, CartItemDto>>(SD.cartKey);
                if (cart is not null)
                {
                    dispatcher.Dispatch(new CartFetched(cart));
                }
                else
                {
                    dispatcher.Dispatch(new CartUpdated(false));
                    dispatcher.Dispatch(new CartUpdating(false));
                }
            }
            catch
            {
                dispatcher.Dispatch(new CartUpdated(false));
                dispatcher.Dispatch(new CartUpdating(false));
            }
        }
    }

    // ********************
    // Actions
    // ********************
    public record AddedToCart(CartItemDto CartItem);
    public record RemovedFromCart(int CartItemKey);
    public record CartPersisted();
    public record CartRemoved();
    public record CartInitilized();
    public record CartFetched(Dictionary<int, CartItemDto> Cart);
    public record CartUpdating(bool IsUpdating);
    public record CartUpdated(bool IsUpdated);

}