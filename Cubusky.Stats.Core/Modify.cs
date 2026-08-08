//using System.Numerics;

//namespace Cubusky.Stats.Core;

//public enum ArithmeticOperation
//{
//    Add,
//    Subtract,
//    Multiply,
//    Divide,
//    Power,
//    Root,
//}

//public delegate TValue Modify<TValue>(TValue baseValue, TValue modifiedValue);

//public class ModifiableMethod<TNumber> where TNumber : INumber<TNumber>
//{
//    public bool AllowNegative { get; init; }

//    public IComparer<ArithmeticOperation> Comparer { get; }

//    private readonly List<Modify<TNumber>> AdditionModifiers = [];
//    private readonly List<Modify<TNumber>> SubtractionModifiers = [];
//    private readonly List<Modify<TNumber>> MultiplicationModifiers = [];
//    private readonly List<Modify<TNumber>> DivisionModifiers = [];
//    private readonly List<Modify<TNumber>> PowerModifiers = [];
//    private readonly List<Modify<TNumber>> RootModifiers = [];

//    private readonly ArithmeticOperation[] _arithmeticOperationsOrdered;

//    public ModifiableMethod(IComparer<ArithmeticOperation>? comparer = null)
//    {
//        Comparer = comparer ?? Comparer<ArithmeticOperation>.Default;
//        _arithmeticOperationsOrdered = Enum.GetValues<ArithmeticOperation>();
//        Array.Sort(_arithmeticOperationsOrdered, Comparer);
//    }

//    private List<Modify<TNumber>> this[ArithmeticOperation operation] => operation switch
//    {
//        ArithmeticOperation.Add => AdditionModifiers,
//        ArithmeticOperation.Subtract => SubtractionModifiers,
//        ArithmeticOperation.Multiply => MultiplicationModifiers,
//        ArithmeticOperation.Divide => DivisionModifiers,
//        ArithmeticOperation.Power => PowerModifiers,
//        ArithmeticOperation.Root => RootModifiers,
//        _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
//    };

//    public void Add(ArithmeticOperation operation, Modify<TNumber> modifier) => this[operation].Add(modifier);
//    public void Remove(ArithmeticOperation operation, Modify<TNumber> modifier) => this[operation].Remove(modifier);

//    public TNumber Modify(TNumber value)
//    {
//        TNumber modifiedValue = value;
//        foreach (var operation in _arithmeticOperationsOrdered)
//        {
//            foreach (var modifier in this[operation])
//            {
//                modifiedValue = modifier(value, modifiedValue);
//            }
//        }
//        return modifiedValue;
//    }
//}
