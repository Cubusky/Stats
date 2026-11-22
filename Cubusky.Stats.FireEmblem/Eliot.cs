using Cubusky.BuildingBlocks;
using Cubusky.Stats.Stat;

namespace Cubusky.Stats.FireEmblem;

public class Eliot
{
    private Stat<int> score = 5;

    public void Test()
    {
        score.Perform(new IncreaseMax<int>(5));
    }
}
