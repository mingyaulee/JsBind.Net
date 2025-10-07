using System;

namespace JsBind.Net.TestsRunner.Models;

public class TestRunnerException(string message) : Exception(message)
{
}
