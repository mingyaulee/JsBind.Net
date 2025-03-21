using System;
using System.Collections.Generic;
using JsBind.Net;

namespace TestBindings.WebAssembly.BindingTestLibrary
{
    public class ComplexTestBoundClass : ObjectBindingBase
    {
        public NestedClass NestedItem { get; set; }
        public NestedClass[] NestedItemsArray { get; set; }
        public IEnumerable<NestedClass> NestedItemsEnumerable { get; set; }
        public NestedClassList NestedItemsList { get; set; }

        public sealed class NestedClass : IEquatable<NestedClass>
        {
            public string Property { get; set; }

            public bool Equals(NestedClass other)
            {
                return Property == other.Property;
            }

            public override bool Equals(object obj)
            {
                return obj is NestedClass other && Equals(other);
            }

            public override int GetHashCode()
            {
                return Property?.GetHashCode() ?? 0;
            }
        }

        public class NestedClassList : List<NestedClass> { }
    }
}
