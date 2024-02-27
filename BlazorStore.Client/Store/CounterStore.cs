using Fluxor;


namespace BlazorStore.Client.Store.Counter
{
    // ********************
    // State
    // ********************
    [FeatureState]
    public record CounterState
    {
        public int CurrentCount { get; init; }
    }

    // ********************
    // Reducers
    // ********************
    public static class Reducers
    {
        [ReducerMethod]
        public static CounterState CounterIncrementedReducer(CounterState state, CounterIncremented action)
        {
            return state with { CurrentCount = state.CurrentCount + action.Step };
        }

    }

    // ********************
    // Actions
    // ********************
    public record CounterIncremented(int Step = 1);

}