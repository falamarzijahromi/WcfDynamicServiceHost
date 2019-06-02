namespace DynamicServiceHost.Host.Tests.TestTypes.Abstracts
{
    public interface IAssertor
    {
        void AssertInvokation(string methodName, object[] methodParams);
    }
}