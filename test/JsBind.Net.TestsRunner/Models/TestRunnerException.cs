using System;

namespace JsBind.Net.TestsRunner.Models
{
    public class TestRunnerException : Exception
    {
        public TestRunnerException(string message) : base(message)
        {
        }
    }
}
