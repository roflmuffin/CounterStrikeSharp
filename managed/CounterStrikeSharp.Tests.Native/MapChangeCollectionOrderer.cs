using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: TestCollectionOrderer("NativeTestsPlugin.MapChangeCollectionOrderer", "NativeTestsPlugin")]

namespace NativeTestsPlugin;

public class MapChangeCollectionOrderer : ITestCollectionOrderer
{
    private readonly DefaultTestCollectionOrderer _default = new();

    public IEnumerable<ITestCollection> OrderTestCollections(IEnumerable<ITestCollection> testCollections)
    {
        return _default.OrderTestCollections(testCollections).OrderBy(collection => collection.DisplayName == "MapChange" ? 1 : 0);
    }
}
