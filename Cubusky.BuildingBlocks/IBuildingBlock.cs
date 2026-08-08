namespace Cubusky.BuildingBlocks;

public interface IOperation<TConformance>;
public interface IBroadcast<in TConformance>;
public interface IBuildingBlock<TConformance> : IOperation<TConformance>, IBroadcast<TConformance>;
