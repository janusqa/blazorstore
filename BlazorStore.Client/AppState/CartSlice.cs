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
            var tempCart = state.Cart.ToDictionary(c => c.Key, c => c.Value);
            tempCart[action.CartItem.ProductPriceId] = action.CartItem;
            return state with { Cart = tempCart };
        }

        [ReducerMethod]
        public static CartState RemoveFromCartReducer(CartState state, RemovedFromCart action)
        {
            var tempCart = state.Cart.ToDictionary(c => c.Key, c => c.Value);
            tempCart.Remove(action.EntityId);
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
            return state with { Updating = action.IsUpdated };
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
                await _localStorage.SetItemAsync<Dictionary<int, CartItemDto>>(SD.cartKey, _state.Value.Cart);
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
    }

    // ********************
    // Actions
    // ********************
    public record AddedToCart(CartItemDto CartItem);
    public record RemovedFromCart(int EntityId);
    public record CartPersisted();
    public record CartUpdating(bool IsUpdating);
    public record CartUpdated(bool IsUpdated);
}