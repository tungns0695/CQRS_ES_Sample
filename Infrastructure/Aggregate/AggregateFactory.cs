using Infrastructure.Exceptions;
using System.Linq.Expressions;

namespace Infrastructure.Aggregate
{
    internal static class AggregateFactory<T>
    {
        private static readonly Func<T> _constructor = CreateTypeConstructor();

        private static Func<T> CreateTypeConstructor()
        {
            try
            {
                var newExpr = Expression.New(typeof(T));
                var func = Expression.Lambda<Func<T>>(newExpr);
                return func.Compile();
            }
            catch (ArgumentException)
            {
                throw new MissingParameterLessConstructorException(typeof(T));
            }
        }

        public static T CreateAggregate()
        {
            return _constructor();
        }
    }
}