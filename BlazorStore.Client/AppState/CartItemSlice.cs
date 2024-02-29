using BlazorStore.Dto;
using Fluxor;

namespace BlazorStore.Client.AppState.CartItem
{
    // ********************
    // State
    // ********************
    [FeatureState]
    public record CartItemState
    {
        public CartItemDto CartItem { get; init; } = new();
    }

    // ********************
    // Reducers
    // ********************
    public static class Reducers
    {
        [ReducerMethod]
        public static CartItemState CartItemReducer(CartItemState state, CartItemSelected action)
        {
            return state with { CartItem = action.CartItem };
        }
    }

    // ********************
    // Effects
    // ********************
    public class Effects
    {

    }

    // ********************
    // Actions
    // ********************
    public record CartItemSelected(CartItemDto CartItem);
}