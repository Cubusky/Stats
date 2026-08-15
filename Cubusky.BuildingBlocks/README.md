<!--# BuildingBlocks

BuildingBlocks is an extremely minimal library consisting only of interfaces and a dependency on Chickensoft.Sync in order to extend Sync with the command pattern. (I think? NEED TO CHECK!!!) Implemented correctly, it allows you to add type-constrained commands onto objects, as if you were extending them with custom-made methods.

ADD THE WORD _REACTIVE_ IN THERE!

## Architectural Safety

Everything follows type-safety. This allows you to build tailor-made commands that may only be invoked on types that they conform to.

For example, an interface can expose the _ability_ to invoke a method without actually exposing its underlying data. Consider the following:

-->

# Building Blocks






## Implementation
Implementing BuildingBlocks for one type can look like quite a bit of code, but it is very simple and always the same. The following is a complete implementation of BuildingBlocks for a class called `MyClass`:

```csharp
using Chickensoft.Collections;
using Chickensoft.Sync;
using Cubusky.BuildingBlocks;

public interface IMyClass : IAutoObject<MyClass.Binding>, IOperator<MyClass>;

public class MyClass : IMyClass, IPerform<PerformOp<MyClass>>
{
  protected SyncSubject Subject { get; }

  public MyClass()
  {
    Subject = new(this);
  }

  private static readonly Blackboard _operationCallbacks = new Blackboard();

  public static void Set<TOperation>(OperationCallback<MyClass>, TOperation> operation)
    where TOperation : struct, IOperation<MyClass>
  {
    _operationCallbacks.Set(operation);
  }

  private readonly BoxlessQueue<IOperation<MyClass>> _operationQueue = new();
  private Broadcaster? _operationBroadcaster;
  private Broadcaster OperationBroadcaster => _operationBroadcaster ??= new(Subject);

  void IOperator<MyClass>.Perform<TOperation>(in TOperation operation)
  {
    _operationQueue.Enqueue(operation);
    Subject.Perform(new PerformOp<MyClass>(this, _operationCallbacks, OperationBroadcaster));
  }

  void IPerform<PerformOp<MyClass>>.Perform(in PerformOp<MyClass> op)
  {
    _operationQueue.Dequeue(op);
  }

  public class Broadcaster : IBroadcaster<MyClass>
  {
    protected SyncSubject Subject { get; }

    internal Broadcaster(SyncSubject subject) => Subject = subject;

    void IBroadcaster<MyClass>.Broadcast<TBroadcast>(in TBroadcast broadcast)
    {
        Subject.Broadcast(broadcast);
    }
  }

  public Binding Bind() => new(Subject);
  public void ClearBindings() => Subject.ClearBindings();
  public void Dispose()
  {
    GC.SuppressFinalize(this);
    Subject.Dispose();
  }

  public class Binding : SyncBinding, IBinding<MyClass, Binding>
  {
    internal Binding(ISyncSubject subject) : base(subject) {}

    Binding IBinding<MyClass, Binding>.On<TBroadcast>(Callback<TBroadcast> callback, Condition<TBroadcast>? condition)
    {
        AddCallback(callback, condition);
        return this;
    }
  }
}
```

Let's break it down:
- `Blackboard _operationCallbacks` is a static field that stores operation callbacks, a.k.a methods, for `MyClass`.
- The 2 `Perform` methods are responsible for executing your operations based on [Sync]'s [no reentrancy]-policy.
- `Broadcaster` can only be created internally and is used to broadcast messages to all subscribers of `MyClass`.
- `Binding` can only be created internally and is used to listen for broadcasts from `MyClass`.



## Extendable and Overridable


#### Why a base class doesn't work