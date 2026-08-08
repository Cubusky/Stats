namespace Cubusky.BuildingBlocks;

public interface IBroadcaster<out TConformance>
{
    void Broadcast<TBroadcast>(in TBroadcast broadcast)
        where TBroadcast : struct, IBroadcast<TConformance>;
}