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
        public bool IsLoading { get; init; } = false;
    }

    // ********************
    // Reducers
    // ********************
    public static class Reducers
    {
        [ReducerMethod]
        public static CartState ItemUpsertedReducer(CartState state, ItemUpserted action)
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
        public static CartState ItemRemovedReducer(CartState state, ItemRemoved action)
        {
            var tempCart = FastDeepCloner.DeepCloner.Clone(state.Cart);
            tempCart.Remove(action.CartItemKey);
            return state with { Cart = tempCart };
        }

        [ReducerMethod]
        public static CartState CartFetchedReducer(CartState state, CartFetched action)
        {
            return state with { Cart = action.Cart, IsLoading = false };
        }

        [ReducerMethod]
        public static CartState IsLoadingReducer(CartState state, IsLoading action)
        {
            return state with { IsLoading = action.Loading };
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
        public async Task CartPersistedReducer(IDispatcher _)
        {
            try
            {
                await _localStorage.SetItemAsync(SD.cartKey, _state.Value.Cart);
            }
            catch (Exception)
            {
                // TODO: Set an error message and log error
            }
        }

        [EffectMethod(typeof(CartRemoved))]
        public async Task CartRemovedReducer(IDispatcher dispatcher)
        {
            try
            {
                dispatcher.Dispatch(new IsLoading(true));
                await _localStorage.RemoveItemAsync(SD.cartKey);
                dispatcher.Dispatch(new CartFetched([]));
            }
            catch (Exception)
            {
                dispatcher.Dispatch(new IsLoading(false));
            }
        }

        [EffectMethod(typeof(CartInitilized))]
        public async Task CartInitilizedReducer(IDispatcher dispatcher)
        {
            try
            {
                Dictionary<int, CartItemDto> cart = [];
                dispatcher.Dispatch(new IsLoading(true));
                cart = (await _localStorage.GetItemAsync<Dictionary<int, CartItemDto>>(SD.cartKey)) ?? cart;
                dispatcher.Dispatch(new CartFetched(cart));
            }
            catch (Exception)
            {
                dispatcher.Dispatch(new IsLoading(false));
            }
        }
    }

    // ********************
    // Actions
    // ********************
    public record ItemUpserted(CartItemDto CartItem);
    public record ItemRemoved(int CartItemKey);
    public record CartPersisted();
    public record CartRemoved();
    public record CartInitilized();
    public record CartFetched(Dictionary<int, CartItemDto> Cart);
    public record IsLoading(bool Loading);
}