using BlazorStore.Dto;
using Fluxor;

namespace BlazorStore.Client.AppState.ProductSelected
{
    // ********************
    // State
    // ********************
    [FeatureState]
    public record ProductSelectedState
    {
        public int Count { get; init; }
        public int ProductPriceId { get; init; }
        public required ProductPriceDto ProductPrice { get; init; }
    }

    // ********************
    // Reducers
    // ********************
    public static class Reducers
    {
        [ReducerMethod]
        public static ProductSelectedState ProductSelectedReducer(ProductSelectedState state, ProductSelected action)
        {
            return state with { Count = state.Count + 1, ProductPriceId = action.ProductPrice.Id, ProductPrice = action.ProductPrice };
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
    public record ProductSelected(ProductPriceDto ProductPrice);
}