using System.Collections.Generic;
using JsBind.Net.Tests.Models;

namespace JsBind.Net.Tests.Infrastructure;

public interface ITestFactory
{
    IEnumerable<TestClassInfo> GetAllTests();
}
