using Castle.DynamicProxy;
using Xunit.Abstractions;

namespace Flowstate.DynamicBuilder.Tests.UnitTests
{
    public class DynamicBuilderFactory
    {
        // TODO: Explore strict convention vs hooks related to Build method

        public static TBuilder Create<TBuilder, TTarget>(ITestOutputHelper output)
            where TBuilder : class
            where TTarget : class, new()
        {
            var proxyGenerator = new ProxyGenerator();
            var proxy = proxyGenerator.CreateInterfaceProxyWithoutTarget<TBuilder>(
                new DynamicBuilderInterceptor<TBuilder, TTarget>(output));

            return proxy;
        }

        private class DynamicBuilderInterceptor<TBuilder, TTarget> : IInterceptor
            where TBuilder : class
            where TTarget : class, new()
        {
            private readonly Dictionary<string, object> _propertyValues;
            private readonly ITestOutputHelper _output;

            private const string With = nameof(With);
            private const string Build = nameof(Build);

            public DynamicBuilderInterceptor(ITestOutputHelper output)
            {
                _propertyValues = new();
                _output = output;
            }

            public void Intercept(IInvocation invocation)
            {
                _output.WriteLine($"Method: '{invocation.Method.Name}'");

                if (invocation.Method.Name.StartsWith(With))
                {
                    var propertyName = invocation.Method.Name[With.Length..];
                    _propertyValues[propertyName] = invocation.Arguments[0];

                    invocation.ReturnValue = invocation.Proxy;
                    return;
                }

                if (invocation.Method.Name == Build)
                {
                    var properties = typeof(TTarget).GetProperties();
                    var configuredProperties = properties
                        .Where(x => _propertyValues.ContainsKey(x.Name))
                        .ToArray();

                    var target = new TTarget();

                    foreach (var property in configuredProperties)
                        property.SetValue(target, _propertyValues[property.Name]);

                    invocation.ReturnValue = target;
                    return;
                }

                throw new Exception(
                    "Convention was not respected. TODO: Validate convention to prevent this path in the first place.");
            }
        }
    }
}