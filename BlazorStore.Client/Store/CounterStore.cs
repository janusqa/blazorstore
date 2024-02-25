using Fluxor;

// State
namespace BlazorStore.Client.Store
{
    [FeatureState]
    public record CounterState(
        int ClickCount
    );


    // Reducers
    public static class Reducers
    {
        [ReducerMethod]
        public static CounterState ReduceIncrementCounterAction(CounterState state, IncrementCounterAction action)
        {
            return action switch
            {
                IncrementCounterAction => new(ClickCount: state.ClickCount + 1),
            };

        }
    }


    // Actions
    public class IncrementCounterAction
    {
    }

}