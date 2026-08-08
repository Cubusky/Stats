namespace Cubusky.BuildingBlocks;

public interface IOperator<TConformance>
{
    void Perform<TOperation>(in TOperation operation)
        where TOperation : struct, IOperation<TConformance>;
}

public static class OperatorExtensions
{
    public static void Perform<TConformance, TOperation>(this IOperator<TConformance> @operator, in TOperation operation)
        where TOperation : struct, IOperation<TConformance>
    {
        @operator.Perform(operation);
    }
}