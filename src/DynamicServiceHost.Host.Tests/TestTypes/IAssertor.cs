namespace DynamicServiceHost.Host.Tests.TestTypes
{
    public interface IAssertor
    {
        void AssertInvokation(string methodName, object[] methodParams);
    }
}