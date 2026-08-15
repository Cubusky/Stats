//public abstract record class Stat(int Value)
//{
//    public int Value { get; protected set; } = Value;

//    public static implicit operator int(Stat stat) => stat.Value;
//}

//public record class Health(int Value, int MaxValue) : Stat(Value)
//{
//    public Health(int Value) : this(Value, Value) { }

//    public void Damage(int amount) => Value = Math.Max(0, Value - amount);

//    public void Heal(int amount) => Value = Value > 0
//        ? Math.Min(MaxValue, Value + amount)
//        : Value;
//}

//public record class Strength(int Value) : Stat(Value);
//public record class Skill(int Value) : Stat(Value);
//public record class Speed(int Value) : Stat(Value);
//public record class Luck(int Value) : Stat(Value);
//public record class Defense(int Value) : Stat(Value);
//public record class Resistance(int Value) : Stat(Value);
//public record class Weight(int Value) : Stat(Value);
//public record class Constitution(int Value) : Stat(Value);

//public record class Attack(int Value) : Stat(Value);
//public record class Hit(int Value) : Stat(Value);
//public record class Crit(int Value) : Stat(Value);

//public record class Avoid(int Value) : Stat(Value);
//public record class CritAvoid(int Value) : Stat(Value);