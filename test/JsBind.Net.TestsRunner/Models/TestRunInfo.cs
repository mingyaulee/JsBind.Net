using System.Collections.Generic;

namespace JsBind.Net.TestsRunner.Models;

public class TestRunInfo
{
    public string TestRunId { get; set; }
    public string Status { get; set; }
    public int Duration { get; set; }
    public IEnumerable<TestInfo> Tests { get; set; }
}
