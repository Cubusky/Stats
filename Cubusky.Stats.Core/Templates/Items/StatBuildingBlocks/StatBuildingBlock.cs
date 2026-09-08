using Cubusky.BuildingBlocks;
using Cubusky.Stats.Core;/* USING_NUMERICS */

namespace MY_NAMESPACE.MY_STAT;

public readonly record struct STAT_BUILDING_BLOCK<MY_VALUE>(MY_VALUE Value)
    : IBuildingBlock<MY_STAT<MY_VALUE>>/* TYPE_CONSTRAINT */
{
    static STAT_BUILDING_BLOCK()
    {
        MY_STAT<MY_VALUE>.Set<STAT_BUILDING_BLOCK<MY_VALUE>>(Callback);
    }

    public static void Callback(in MY_STAT<MY_VALUE> my_stat, in STAT_BUILDING_BLOCK<MY_VALUE> stat_building_block, in IBroadcaster<MY_STAT<MY_VALUE>> broadcaster)
    {
        if (my_stat.TrySet(stat_building_block.Value, out _))
        {
            broadcaster.Broadcast(new STAT_BUILDING_BLOCK<MY_VALUE>(my_stat.Value));
        }
    }
}
