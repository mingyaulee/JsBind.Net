using System.Threading.Tasks;

namespace JsBind.Net.Tests.Infrastructure
{
    public interface ITestRunner
    {
        Task RunTests();
        Task GetTestCoverageInfo();
    }
}
