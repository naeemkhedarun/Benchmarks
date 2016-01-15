using System;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet;
using BenchmarkDotNet.Tasks;
using Castle.DynamicProxy;
using LightInject.Interception;
using NProxy.Core;
using IInterceptor = LightInject.Interception.IInterceptor;

namespace Benchmarking
{
    [BenchmarkTask]
    public class DynamicProxyCreationBenchmark
    {
        private readonly Type _proxyType;
        private readonly Type _dynamicProxyType;
        private readonly NProxyInterceptor _nProxyInterceptor;
        private readonly IProxyTemplate _nproxyTemplate;

        public DynamicProxyCreationBenchmark()
        {
            var proxyBuilder = new ProxyBuilder();
            var proxyGenerator = new ProxyGenerator();
            var proxyFactory = new ProxyFactory();
            _proxyType = proxyBuilder.GetProxyType(new ProxyDefinition(typeof(TestClass), true).Implement(() => new LightInjectInterceptor()));
            _dynamicProxyType = proxyGenerator.ProxyBuilder.CreateClassProxyType(typeof(TestClass), Type.EmptyTypes, ProxyGenerationOptions.Default);
            _nProxyInterceptor = new NProxyInterceptor();
            _nproxyTemplate = proxyFactory.GetProxyTemplate(typeof(TestClass), Enumerable.Empty<Type>());
        }

        [Benchmark]
        public TestClass NoProxyConstructor()
        {
            return new TestClass();
        }


        [Benchmark]
        public TestClass NoProxyActivator()
        {
            return (TestClass)Activator.CreateInstance(typeof(TestClass));
        }

        [Benchmark]
        public TestClass LightInjectCreation()
        {
            return (TestClass)Activator.CreateInstance(_proxyType);
        }

        [Benchmark]
        public TestClass DynamicProxyCreation()
        {
            return (TestClass)Activator.CreateInstance(_dynamicProxyType);
        }

        [Benchmark]
        public TestClass NProxyCreation()
        {
            return (TestClass) _nproxyTemplate.CreateProxy(_nProxyInterceptor);
        }
    }

    [BenchmarkTask]
    public class DynamicProxyCallBenchmark
    {
        private readonly TestClass _lightInjectInstance;
        private readonly TestClass _dynamicProxyInstance;
        private readonly TestClass _nproxyInstance;
        private readonly TestClass _instance;

        public DynamicProxyCallBenchmark()
        {
            var creator = new DynamicProxyCreationBenchmark();

            _lightInjectInstance = creator.LightInjectCreation();
            _dynamicProxyInstance = creator.DynamicProxyCreation();
            _nproxyInstance = creator.NProxyCreation();
            _instance = creator.NoProxyConstructor();
        }

        [Benchmark]
        public void LightInjectCallMethod()
        {
            _lightInjectInstance.CallMethod();
        }

        [Benchmark]
        public void DynamicProxyCallMethod()
        {
            _dynamicProxyInstance.CallMethod();
        }

        [Benchmark]
        public void NProxyCallMethod()
        {
            _nproxyInstance.CallMethod();
        }

        [Benchmark]
        public void NoProxyCallMethod()
        {
            _instance.CallMethod();
        }
    }

    public class NProxyInterceptor : IInvocationHandler
    {
        public object Invoke(object target, MethodInfo methodInfo, object[] parameters)
        {
            return methodInfo.Invoke(target, parameters);
        }
    }

    public class TestClass
    {
        public void CallMethod() { }
    }

    public class LightInjectInterceptor : IInterceptor
    {
        public object Invoke(IInvocationInfo invocationInfo)
        {
            return invocationInfo.Proceed();
        }
    }

    public class DynamicProxyInterceptor : Castle.DynamicProxy.IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
        }
    }
}