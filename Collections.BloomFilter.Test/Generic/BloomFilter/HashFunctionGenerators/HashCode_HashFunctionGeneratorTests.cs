using System;
using System.Collections.Generic;
using System.Collections.Generic.BloomFilter.HashFunctionGenerators;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Test.Generic.BloomFilter.HashFunctionGenerators
{
    public abstract class HashCode_HashFunctionGeneratorTests<ElementT>
        : IHashFunctionGeneratorTests<ElementT, HashCode_HashFunctionGenerator<ElementT>>
    {
        protected override int MaxBitsGeneratable { get { return 32; } }
    }

    #region Concrete Test Classes

    #region Type Tests

    public class int_HashCode_HashFunctionGeneratorTests
        : HashCode_HashFunctionGeneratorTests<int>
    {
        protected override int ElementA { get { return 0; } }
    }

    public class char_HashCode_HashFunctionGeneratorTests
        : HashCode_HashFunctionGeneratorTests<char>
    {
        protected override char ElementA { get { return 'a'; } }
    }

    public class string_HashCode_HashFunctionGeneratorTests
        : HashCode_HashFunctionGeneratorTests<string>
    {
        protected override string ElementA { get { return "a"; } }
    }

    public class object_HashCode_HashFunctionGeneratorTests
        : HashCode_HashFunctionGeneratorTests<object>
    {
        protected override object ElementA { get { return 0; } }
    }

    public class dynamic_HashCode_HashFunctionGeneratorTests
        : HashCode_HashFunctionGeneratorTests<dynamic>
    {
        protected override dynamic ElementA { get { return 0; } }
    }

    public class struct_HashCode_HashFunctionGeneratorTests
        : HashCode_HashFunctionGeneratorTests<struct_HashCode_HashFunctionGeneratorTests.TestStruct>
    {
        protected override TestStruct ElementA { get { return new TestStruct() { Member = 'a' }; } }

        public struct TestStruct
        {
            public char Member;
        }
    }

    public class class_HashCode_HashFunctionGeneratorTests
        : HashCode_HashFunctionGeneratorTests<class_HashCode_HashFunctionGeneratorTests.TestClass>
    {
        protected override TestClass ElementA { get { return new TestClass() { Property = 'a' }; } }

        public class TestClass
        {
            public char Property { get; set; }

            public override string ToString()
            {
                return Property.ToString();
            }
        }
    }

    public class enum_HashCode_HashFunctionGeneratorTests
        : HashCode_HashFunctionGeneratorTests<enum_HashCode_HashFunctionGeneratorTests.TestEnum>
    {
        protected override TestEnum ElementA { get { return TestEnum.ElementA; } }

        public enum TestEnum
        {
            ElementA = 0
        }
    }

    #endregion

    #endregion


}
