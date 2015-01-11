﻿using System.Text;
using Cake.Core.Scripting.CodeGen;
using Cake.Core.Tests.Fixtures;
using Xunit;

namespace Cake.Core.Tests.Unit.Scripting.CodeGen
{
    public sealed class PropertyAliasGeneratorTests
    {
        public sealed class TheGenerateMethod
        {
            [Fact]
            public void Should_Throw_If_Method_Is_Null()
            {
                // Given, When
                var result = Record.Exception(() => PropertyAliasGenerator.Generate(null));

                // Then
                Assert.IsArgumentNullException(result, "method");
            }

            [Fact]
            public void Should_Throw_If_Declaring_Type_Is_Not_Static()
            {
                // Given
                var method = GetType().GetMethod("Should_Throw_If_Declaring_Type_Is_Not_Static");

                // When
                var result = Record.Exception(() => PropertyAliasGenerator.Generate(method));

                // Then
                Assert.IsType<CakeException>(result);
                Assert.Equal("The type 'Cake.Core.Tests.Unit.Scripting.CodeGen.PropertyAliasGeneratorTests+TheGenerateMethod' is not static.",
                    result.Message);
            }

            [Fact]
            public void Should_Throw_If_Method_Is_Not_An_Extension_Method()
            {
                // Given
                var method = typeof(PropertyAliasGeneratorFixture).GetMethod("NotAnExtensionMethod");

                // When
                var result = Record.Exception(() => PropertyAliasGenerator.Generate(method));

                // Then
                Assert.IsType<CakeException>(result);
                Assert.Equal("The method 'NotAnExtensionMethod' is not an extension method.",
                    result.Message);
            }

            [Fact]
            public void Should_Throw_If_Method_Is_Not_An_Cake_Property_Alias()
            {
                // Given
                var method = typeof(PropertyAliasGeneratorFixture).GetMethod("NotAScriptMethod");

                // When
                var result = Record.Exception(() => PropertyAliasGenerator.Generate(method));

                // Then
                Assert.IsType<CakeException>(result);
                Assert.Equal("The method 'NotAScriptMethod' is not a property alias.",
                    result.Message);
            }

            [Fact]
            public void Should_Throw_If_Property_Alias_Have_More_Than_One_Argument()
            {
                // Given
                var method = typeof(PropertyAliasGeneratorFixture).GetMethod("PropertyAliasWithMoreThanOneMethod");

                // When
                var result = Record.Exception(() => PropertyAliasGenerator.Generate(method));

                // Then
                Assert.IsType<CakeException>(result);
                Assert.Equal("The property alias 'PropertyAliasWithMoreThanOneMethod' has an invalid signature.",
                    result.Message);
            }

            [Fact]
            public void Should_Throw_If_Property_Alias_Do_Not_Have_A_Cake_Context_As_First_Parameter()
            {
                // Given
                var method = typeof(PropertyAliasGeneratorFixture).GetMethod("PropertyAliasWithoutContext");

                // When
                var result = Record.Exception(() => PropertyAliasGenerator.Generate(method));

                // Then
                Assert.IsType<CakeException>(result);
                Assert.Equal("The property alias 'PropertyAliasWithoutContext' has an invalid signature.",
                    result.Message);
            }

            [Fact]
            public void Should_Throw_If_Method_Is_Generic()
            {
                // Given
                var method = typeof(PropertyAliasGeneratorFixture).GetMethod("GenericScriptMethod");

                // When
                var result = Record.Exception(() => PropertyAliasGenerator.Generate(method));

                // Then
                Assert.IsType<CakeException>(result);
                Assert.Equal("The property alias 'GenericScriptMethod' cannot be generic.",
                    result.Message);
            }

            [Fact]
            public void Should_Throw_If_Property_Alias_Returns_Void()
            {
                // Given
                var method = typeof(PropertyAliasGeneratorFixture).GetMethod("PropertyAliasReturningVoid");

                // When
                var result = Record.Exception(() => PropertyAliasGenerator.Generate(method));

                // Then
                Assert.IsType<CakeException>(result);
                Assert.Equal("The property alias 'PropertyAliasReturningVoid' cannot return void.",
                    result.Message);
            }

            [Fact]
            public void Should_Generate_Correct_Code_For_Valid_Property_Alias()
            {
                // Given
                const string expected = "public System.Int32 PropertyAliasReturningInteger{get{return " +
                                        "Cake.Core.Tests.Fixtures.PropertyAliasGeneratorFixture.PropertyAliasReturningInteger(GetContext());}}";

                var method = typeof(PropertyAliasGeneratorFixture).GetMethod("PropertyAliasReturningInteger");

                // When
                var result = PropertyAliasGenerator.Generate(method);

                // Then
                Assert.Equal(expected, result);
            }

            [Fact]
            public void Should_Generate_Cached_Code_For_Cached_Property_Alias_Returning_Reference_Type()
            {
                // Given
                var expected = new StringBuilder();
                expected.Append("private System.String _PropertyAliasReturningCachedString;\n");
                expected.Append("public System.String PropertyAliasReturningCachedString{get{");
                expected.Append("if(_PropertyAliasReturningCachedString==null){_PropertyAliasReturningCachedString=");
                expected.Append("Cake.Core.Tests.Fixtures.PropertyAliasGeneratorFixture.PropertyAliasReturningCachedString");
                expected.Append("(GetContext());}return _PropertyAliasReturningCachedString;}}");

                var method = typeof(PropertyAliasGeneratorFixture).GetMethod("PropertyAliasReturningCachedString");

                // When
                var result = PropertyAliasGenerator.Generate(method);

                // Then
                Assert.Equal(expected.ToString(), result);
            }

            [Fact]
            public void Should_Generate_Cached_Code_For_Cached_Property_Alias_Returning_Value_Type()
            {
                // Given
                var expected = new StringBuilder();
                expected.Append("private System.Boolean? _PropertyAliasReturningCachedBoolean;\n");
                expected.Append("public System.Boolean PropertyAliasReturningCachedBoolean{get{");
                expected.Append("if(_PropertyAliasReturningCachedBoolean==null){_PropertyAliasReturningCachedBoolean=");
                expected.Append("Cake.Core.Tests.Fixtures.PropertyAliasGeneratorFixture.PropertyAliasReturningCachedBoolean");
                expected.Append("(GetContext());}return _PropertyAliasReturningCachedBoolean.Value;}}");

                var method = typeof(PropertyAliasGeneratorFixture).GetMethod("PropertyAliasReturningCachedBoolean");

                // When
                var result = PropertyAliasGenerator.Generate(method);

                // Then
                Assert.Equal(expected.ToString(), result);
            }
        }
    }
}