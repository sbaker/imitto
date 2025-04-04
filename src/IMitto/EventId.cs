namespace IMitto;

    public abstract class EventId
    {
        public static readonly EventId Empty = new EventId<object?>(null);

        public static EventId<T> New<T>(T data) => new(data);

        public static implicit operator EventId(bool value)
        {
            return new EventId<bool>(value);
        }

        public static implicit operator EventId(int value)
        {
            return new EventId<int>(value);
        }

        public static implicit operator EventId(uint value)
        {
            return new EventId<uint>(value);
        }

        public static implicit operator EventId(long value)
        {
            return new EventId<long>(value);
        }

        public static implicit operator EventId(ulong value)
        {
            return new EventId<ulong>(value);
        }

        public static implicit operator EventId(float value)
        {
            return new EventId<float>(value);
        }

        public static implicit operator EventId(double value)
        {
            return new EventId<double>(value);
        }

        public static implicit operator EventId(DateTime value)
        {
            return new EventId<DateTime>(value);
        }

        public static implicit operator EventId(Guid value)
        {
            return new EventId<Guid>(value);
        }

        public static implicit operator EventId(string value)
        {
            return new EventId<string>(value);
        }
    }

    public class EventId<T>(T value) : EventId, IEquatable<EventId<T>>
    {
        private static readonly IEqualityComparer<T> Comparer = EqualityComparer<T>.Default;

        public T Value { get; } = value;

        public static implicit operator EventId<T>(T value)
        {
            return new EventId<T>(value);
        }

        public static implicit operator T(EventId<T> value)
        {
            return value.Value;
        }

        /// <summary>
        /// Returns a value that indicates whether the values of two <see cref="EventId" /> objects are equal.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>
        /// true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.
        /// </returns>
        public static bool operator ==(EventId<T> left, EventId<T> right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Returns a value that indicates whether two <see cref="EventId" /> objects have different values.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>
        /// true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.
        /// </returns>
        public static bool operator !=(EventId<T> left, EventId<T> right)
        {
            return !Equals(left, right);
        }

        /// <inheritdoc />
        public override string? ToString()
        {
            if (Equals(Value, default(T)))
            {
                return base.ToString();
            }

            return Convert.ToString(Value);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;

            return ((IEquatable<EventId<T>>)this).Equals((EventId<T>)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
            => Value is null ? base.GetHashCode() : Comparer.GetHashCode(Value);

        /// <inheritdoc />
        bool IEquatable<EventId<T>>.Equals(EventId<T>? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Comparer.Equals(Value, other.Value);
        }
    }