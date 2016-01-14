using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BenchmarkDotNet;
using BenchmarkDotNet.Tasks;

namespace Benchmarking
{
    [BenchmarkTask]
    public class PropertyGetterReflectionBenchmark
    {
        readonly PropertyInfo _cached;
        readonly Delegate _delegate;
        Func<object> _lambda;
        readonly Func<object> _typedDelegate;
        public String Property { get; set; }
        String _toSet;

        public PropertyGetterReflectionBenchmark()
        {
            Property = "a";

            _cached = GetType().GetProperties().First();
            _delegate = _cached.GetMethod.CreateDelegate(typeof(Func<String>), this);
            _typedDelegate = (Func<Object>)_cached.GetMethod.CreateDelegate(typeof(Func<Object>), this);

            CreateLambda(this, "Property");
        }

        void CreateLambda(object value, string propertyName)
        {
            var property = Expression.Property(Expression.Constant(value), propertyName);
            var expression = Expression.Lambda<Func<Object>>(property);
            _lambda = expression.Compile();
        }

        [Benchmark]
        public void UncachedPropertyInfoGet()
        {
            _toSet = (string) GetType().GetProperties().First().GetValue(this);
        }

        [Benchmark]
        public void CachedPropertyInfoGet()
        {
            _toSet = (string) _cached.GetValue(this);
        }

        [Benchmark]
        public void DynamicInvokeDelegateCreate()
        {
            _toSet = (string) _delegate.DynamicInvoke();
        }

        [Benchmark]
        public void InvokeDelegateCreate()
        {
            _toSet = (string) _typedDelegate.Invoke();
        }

        [Benchmark]
        public void CachedExpression()
        {
            _toSet = (string) _lambda();
        }

        [Benchmark]
        public void NativeAssignment()
        {
            _toSet = Property;
        }
    }
}