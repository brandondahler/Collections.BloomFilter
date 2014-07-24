using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Generic.BloomFilter.HashFunctionGenerators;
using System.Collections.Test.Generic.BloomFilter;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Test.Generic.BloomFilter.HashFunctionGenerators
{
    public abstract class KeyedHashAlgorithm_HashFunctionGeneratorTests<ElementT, KeyedHashAlgorithmT>
        : HashAlgorithm_HashFunctionGeneratorTests<ElementT, KeyedHashAlgorithmT, KeyedHashAlgorithm_HashFunctionGenerator<ElementT, KeyedHashAlgorithmT>>
        where KeyedHashAlgorithmT : KeyedHashAlgorithm, new()
    {
        protected override int MaxBitsGeneratable { get { return int.MaxValue; } }
    }


    public abstract class KeyedHashAlgorithmTypeBase_KeyedHashAlgorithm_HashFunctionGeneratorTests<KeyedHashAlgorithmT>
        : KeyedHashAlgorithm_HashFunctionGeneratorTests<int, KeyedHashAlgorithmT>
        where KeyedHashAlgorithmT : KeyedHashAlgorithm, new()
    {
        protected override int ElementA { get { return 0; } }
    }

    public abstract class ElementTypeBase_KeyedHashAlgorithm_HashFunctionGeneratorTests<ElementT>
        : KeyedHashAlgorithm_HashFunctionGeneratorTests<ElementT, HMACSHA512>
    {

    }

    #region Concrete Test Classes

    #region KeyedHashAlgorithm Tests

    public class HMACSHA1_KeyedHashAlgorithm_HashFunctionGeneratorTests
        : KeyedHashAlgorithmTypeBase_KeyedHashAlgorithm_HashFunctionGeneratorTests<HMACSHA1>
    {

    }

    public class HMACSHA256_KeyedHashAlgorithm_HashFunctionGeneratorTests
        : KeyedHashAlgorithmTypeBase_KeyedHashAlgorithm_HashFunctionGeneratorTests<HMACSHA256>
    {

    }

    public class HMACSHA384_KeyedHashAlgorithm_HashFunctionGeneratorTests
        : KeyedHashAlgorithmTypeBase_KeyedHashAlgorithm_HashFunctionGeneratorTests<HMACSHA384>
    {

    }

    public class HMACSHA512_KeyedHashAlgorithm_HashFunctionGeneratorTests
        : KeyedHashAlgorithmTypeBase_KeyedHashAlgorithm_HashFunctionGeneratorTests<HMACSHA512>
    {

    }


    public class HMACMD5_KeyedHashAlgorithm_HashFunctionGeneratorTests
        : KeyedHashAlgorithmTypeBase_KeyedHashAlgorithm_HashFunctionGeneratorTests<HMACMD5>
    {

    }

    public class HMACRIPEMD160_KeyedHashAlgorithm_HashFunctionGeneratorTests
        : KeyedHashAlgorithmTypeBase_KeyedHashAlgorithm_HashFunctionGeneratorTests<HMACRIPEMD160>
    {

    }
    
    #endregion

    #region Type Tests

    public class int_KeyedHashAlgorithm_HashFunctionGeneratorTests
    : ElementTypeBase_KeyedHashAlgorithm_HashFunctionGeneratorTests<int>
    {
        protected override int ElementA { get { return 0; } }
    }

    public class char_KeyedHashAlgorithm_HashFunctionGeneratorTests
        : ElementTypeBase_KeyedHashAlgorithm_HashFunctionGeneratorTests<char>
    {
        protected override char ElementA { get { return 'a'; } }
    }

    public class string_KeyedHashAlgorithm_HashFunctionGeneratorTests
        : ElementTypeBase_KeyedHashAlgorithm_HashFunctionGeneratorTests<string>
    {
        protected override string ElementA { get { return "a"; } }
    }

    public class object_KeyedHashAlgorithm_HashFunctionGeneratorTests
        : ElementTypeBase_KeyedHashAlgorithm_HashFunctionGeneratorTests<object>
    {
        protected override object ElementA { get { return 0; } }
    }

    public class dynamic_KeyedHashAlgorithm_HashFunctionGeneratorTests
        : ElementTypeBase_KeyedHashAlgorithm_HashFunctionGeneratorTests<dynamic>
    {
        protected override dynamic ElementA { get { return 0; } }
    }

    public class struct_KeyedHashAlgorithm_HashFunctionGeneratorTests
        : ElementTypeBase_KeyedHashAlgorithm_HashFunctionGeneratorTests<struct_KeyedHashAlgorithm_HashFunctionGeneratorTests.TestStruct>
    {
        protected override TestStruct ElementA { get { return new TestStruct() { Member = 'a' }; } }

        [Serializable]
        public struct TestStruct
        {
            public char Member;
        }
    }

    public class class_KeyedHashAlgorithm_HashFunctionGeneratorTests
        : ElementTypeBase_KeyedHashAlgorithm_HashFunctionGeneratorTests<class_KeyedHashAlgorithm_HashFunctionGeneratorTests.TestClass>
    {
        protected override TestClass ElementA { get { return new TestClass() { Property = 'a' }; } }

        [Serializable]
        public class TestClass
        {
            public char Property { get; set; }
        }
    }

    public class enum_KeyedHashAlgorithm_HashFunctionGeneratorTests
        : ElementTypeBase_KeyedHashAlgorithm_HashFunctionGeneratorTests<enum_KeyedHashAlgorithm_HashFunctionGeneratorTests.TestEnum>
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
