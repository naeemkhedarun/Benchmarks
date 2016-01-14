using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BenchmarkDotNet;
using BenchmarkDotNet.Tasks;

namespace Benchmarking
{
    [BenchmarkTask]
    public class PropertySetterReflectionBenchmark
    {
        readonly PropertyInfo _cached;
        readonly Delegate _delegate;
        Action<string> _lambda;
        Action<string> _typedDelegate;
        public String Property { get; set; }

        public PropertySetterReflectionBenchmark()
        {
            _cached = GetType().GetProperties().First();
            _delegate = _cached.SetMethod.CreateDelegate(typeof(Action<String>), this);
            _typedDelegate = (Action<String>) _cached.SetMethod.CreateDelegate(typeof(Action<String>), this);

            CreateLambda(this, "Property");
        }

        void CreateLambda(object value, string propertyName)
        {
            var parameterExpression = Expression.Parameter(typeof (String), "StringName");
            var body = Expression.Assign(Expression.Property(Expression.Constant(value), propertyName), parameterExpression);
            var expression = Expression.Lambda<Action<String>>(body, parameterExpression);
            _lambda = expression.Compile();
        }

        [Benchmark]
        public void UncachedPropertyInfoSet()
        {
            GetType().GetProperties().First().SetValue(this, "a");
        }

        [Benchmark]
        public void CachedPropertyInfoSet()
        {
            _cached.SetValue(this, "b");
        }

        [Benchmark]
        public void DynamicInvokeDelegateCreate()
        {
            _delegate.DynamicInvoke("c");
        }

        [Benchmark]
        public void InvokeDelegateCreate()
        {
            _typedDelegate.Invoke("d");
        }

        [Benchmark]
        public void CachedExpression()
        {
            _lambda("e");
        }

        [Benchmark]
        public void NativeAssignment()
        {
            Property = "f";
        }
    }
}