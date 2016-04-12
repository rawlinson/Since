using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using Since.ImmutabilityAnalyzer;

namespace Since.ImmutabilityAnalyzer.Test
{
    [TestClass]
    public class UnitTest : CodeFixVerifier
    {

        //No diagnostics expected to show up
        [TestMethod]
        public void TestMethod1()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestMethod]
        public void VerifySIA1001()
        {
            var test = @"
    namespace TestNamespace
    {
        class BaseClass { }

        [Immutable]
        class TypeName : BaseClass
        {   
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "SIA1001",
                Message = "Type 'TypeName' is marked immutable but base class 'BaseClass' is not.",
                Severity = DiagnosticSeverity.Error,
                Locations = null
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void VerifySIA1002_PublicField()
        {
            var test = @"
    namespace TestNamespace
    {
        [Immutable]
        class TypeName
        {
            public int MutableInt;
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "SIA1001",
                Message = "Type 'TypeName' is marked immutable but member 'MutableInt' can be written to.",
                Severity = DiagnosticSeverity.Error,
                Locations = null
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestMethod]
        public void VerifySIA1002_PrivateField()
        {
            var test = @"
    namespace TestNamespace
    {
        [Immutable]
        class TypeName
        {
            private int MutableInt;
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "SIA1001",
                Message = "Type 'TypeName' is marked immutable but member 'MutableInt' can be written to.",
                Severity = DiagnosticSeverity.Error,
                Locations = null
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SinceImmutabilityAnalyzer();
        }
    }
}