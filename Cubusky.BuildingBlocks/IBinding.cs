using Chickensoft.Sync;

namespace Cubusky.BuildingBlocks;

public interface IBinding<out TConformance, out TBinding> : IDisposable
    where TBinding : IBinding<TConformance, TBinding>
{
    TBinding On<TBroadcast>(Callback<TBroadcast> callback, Condition<TBroadcast>? condition = null)
        where TBroadcast : struct, IBroadcast<TConformance>;
}