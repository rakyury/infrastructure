﻿using System;
using System.Globalization;
using Spark;
using Spark.Resources;
using Xunit;

/* Copyright (c) 2015 Spark Software Ltd.
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

namespace Test.Spark
{
    // ReSharper disable NotResolvedInText
    namespace UsingVerify
    {
        public class WhenCheckingForTrueCondition
        {
            [Fact]
            public void TrueDoesNotThrow()
            {
                Verify.True(true, "paramName", "Custom Message");
            }

            [Fact]
            public void FalseThrowsArgumentException()
            {
                var expectedEx = new ArgumentException("Custom Message", "paramName");
                var actualEx = Assert.Throws<ArgumentException>(() => Verify.True(false, "paramName", "Custom Message"));

                Assert.Equal(expectedEx.Message, actualEx.Message);
            }
        }

        public class WhenCheckingForFalseCondition
        {
            [Fact]
            public void FalseDoesNotThrow()
            {
                Verify.False(false, "paramName", "Custom Message");
            }

            [Fact]
            public void TrueThrowsArgumentException()
            {
                var expectedEx = new ArgumentException("Custom Message", "paramName");
                var actualEx = Assert.Throws<ArgumentException>(() => Verify.False(true, "paramName", "Custom Message"));

                Assert.Equal(expectedEx.Message, actualEx.Message);
            }
        }

        public class WhenCheckingTypeDerivesFromBase
        {
            [Fact]
            public void BaseTypeFoundDoesNotThrow()
            {
                Verify.TypeDerivesFrom(typeof(Exception), typeof(ArgumentException), "paramName");
            }

            [Fact]
            public void BaseTypeNotFoundThrowsArgumentException()
            {
                var expectedEx = new ArgumentException(Exceptions.TypeDoesNotDeriveFromBase.FormatWith(typeof(Exception), typeof(Object)), "paramName");
                var actualEx = Assert.Throws<ArgumentException>(() => Verify.TypeDerivesFrom(typeof(Exception), typeof(Object), "paramName"));

                Assert.Equal(expectedEx.Message, actualEx.Message);
            }
        }

        public class WhenCheckingForEquality
        {
            [Fact]
            public void EqualValueDoesNotThrow()
            {
                Verify.Equal(Guid.Empty, Guid.Empty, "paramName");
            }

            [Fact]
            public void NotEqualThrowsArgumentException()
            {
                var actual = Guid.NewGuid();
                var expectedEx = new ArgumentOutOfRangeException("paramName", actual, Exceptions.ArgumentNotEqualToValue.FormatWith(Guid.Empty));
                var actualEx = Assert.Throws<ArgumentOutOfRangeException>(() => Verify.Equal(Guid.Empty, actual, "paramName"));

                Assert.Equal(expectedEx.Message, actualEx.Message);
            }
        }

        public class WhenCheckingForInequality
        {
            [Fact]
            public void NotEqualValueDoesNotThrow()
            {
                Verify.NotEqual(Guid.Empty, Guid.NewGuid(), "paramName");
            }

            [Fact]
            public void EqualThrowsArgumentException()
            {
                var expectedEx = new ArgumentException(Exceptions.ArgumentEqualToValue.FormatWith(Guid.Empty), "paramName");
                var actualEx = Assert.Throws<ArgumentException>(() => Verify.NotEqual(Guid.Empty, Guid.Empty, "paramName"));

                Assert.Equal(expectedEx.Message, actualEx.Message);
            }
        }

        public class WhenCheckingForGreaterThan
        {
            [Fact]
            public void NullValuesThrowArgumentOutOfRangeException()
            {
                var expectedEx = new ArgumentOutOfRangeException("paramName", Exceptions.ArgumentNotGreaterThanValue.FormatWith(String.Empty));
                var actualEx = Assert.Throws<ArgumentOutOfRangeException>(() => Verify.GreaterThan(default(IComparable), null, "paramName"));

                Assert.Equal(expectedEx.Message, actualEx.Message);
            }

            [Fact]
            public void NullActualValueThrowArgumentOutOfRangeException()
            {
                var expectedEx = new ArgumentOutOfRangeException("paramName", Exceptions.ArgumentNotGreaterThanValue.FormatWith(0));
                var actualEx = Assert.Throws<ArgumentOutOfRangeException>(() => Verify.GreaterThan((Comparable)0, null, "paramName"));

                Assert.Equal(expectedEx.Message, actualEx.Message);
            }

            [Fact]
            public void NullExpectedValueDoesNotThrowException()
            {
                Verify.GreaterThan(null, (Comparable)1, "paramName");
            }

            [Fact]
            public void ActualLessThanExpectedThrowsArgumentOutOfRangeException()
            {
                var expectedEx = new ArgumentOutOfRangeException("paramName", 0, Exceptions.ArgumentNotGreaterThanValue.FormatWith(1));
                var actualEx = Assert.Throws<ArgumentOutOfRangeException>(() => Verify.GreaterThan(1, 0, "paramName"));

                Assert.Equal(expectedEx.Message, actualEx.Message);
            }

            [Fact]
            public void ActualEqualToExpectedThrowsArgumentOutOfRangeException()
            {
                var expectedEx = new ArgumentOutOfRangeException("paramName", 1, Exceptions.ArgumentNotGreaterThanValue.FormatWith(1));
                var actualEx = Assert.Throws<ArgumentOutOfRangeException>(() => Verify.GreaterThan(1, 1, "paramName"));

                Assert.Equal(expectedEx.Message, actualEx.Message);
            }

            [Fact]
            public void ActualGreaterThanExpectedDoesNotThrowException()
            {
                Verify.GreaterThan(1, 2, "paramName");
            }

            private class Comparable : IComparable
            {
                private readonly Int32 value;

                private Comparable(Int32 value)
                {
                    this.value = value;
                }

                public Int32 CompareTo(Object obj)
                {
                    return value.CompareTo(obj);
                }

                public override string ToString()
                {
                    return value.ToString(CultureInfo.InvariantCulture);
                }

                public static implicit operator Comparable(Int32 value)
                {
                    return new Comparable(value);
                }
            }
        }

        public class WhenCheckingForGreaterThanOrEqual
        {
            [Fact]
            public void NullValuesThrowArgumentOutOfRangeException()
            {
                var expectedEx = new ArgumentOutOfRangeException("paramName", Exceptions.ArgumentNotGreaterThanOrEqualToValue.FormatWith(String.Empty));
                var actualEx = Assert.Throws<ArgumentOutOfRangeException>(() => Verify.GreaterThanOrEqual(default(IComparable), null, "paramName"));

                Assert.Equal(expectedEx.Message, actualEx.Message);
            }

            [Fact]
            public void NullActualValueThrowArgumentOutOfRangeException()
            {
                var expectedEx = new ArgumentOutOfRangeException("paramName", Exceptions.ArgumentNotGreaterThanOrEqualToValue.FormatWith(0));
                var actualEx = Assert.Throws<ArgumentOutOfRangeException>(() => Verify.GreaterThanOrEqual((Comparable)0, null, "paramName"));

                Assert.Equal(expectedEx.Message, actualEx.Message);
            }

            [Fact]
            public void NullExpectedValueDoesNotThrowException()
            {
                Verify.GreaterThanOrEqual(null, (Comparable)1, "paramName");
            }

            [Fact]
            public void ActualLessThanExpectedThrowsArgumentOutOfRangeException()
            {
                var expectedEx = new ArgumentOutOfRangeException("paramName", 0, Exceptions.ArgumentNotGreaterThanOrEqualToValue.FormatWith(1));
                var actualEx = Assert.Throws<ArgumentOutOfRangeException>(() => Verify.GreaterThanOrEqual(1, 0, "paramName"));

                Assert.Equal(expectedEx.Message, actualEx.Message);
            }

            [Fact]
            public void ActualEqualToExpectedDoesNotThrowException()
            {
                Verify.GreaterThanOrEqual(1, 1, "paramName");
            }

            [Fact]
            public void ActualGreaterThanExpectedDoesNotThrowException()
            {
                Verify.GreaterThanOrEqual(1, 2, "paramName");
            }

            private class Comparable : IComparable
            {
                private readonly Int32 value;

                private Comparable(Int32 value)
                {
                    this.value = value;
                }

                public Int32 CompareTo(Object obj)
                {
                    return value.CompareTo(obj);
                }

                public override string ToString()
                {
                    return value.ToString(CultureInfo.InvariantCulture);
                }

                public static implicit operator Comparable(Int32 value)
                {
                    return new Comparable(value);
                }
            }
        }

        public class WhenCheckingForLessThan
        {
            [Fact]
            public void NullValuesThrowArgumentOutOfRangeException()
            {
                var expectedEx = new ArgumentOutOfRangeException("paramName", Exceptions.ArgumentNotLessThanValue.FormatWith(String.Empty));
                var actualEx = Assert.Throws<ArgumentOutOfRangeException>(() => Verify.LessThan(default(IComparable), null, "paramName"));

                Assert.Equal(expectedEx.Message, actualEx.Message);
            }

            [Fact]
            public void NullActualValueThrowArgumentOutOfRangeException()
            {
                var expectedEx = new ArgumentOutOfRangeException("paramName", Exceptions.ArgumentNotLessThanValue.FormatWith(0));
                var actualEx = Assert.Throws<ArgumentOutOfRangeException>(() => Verify.LessThan((Comparable)0, null, "paramName"));

                Assert.Equal(expectedEx.Message, actualEx.Message);
            }

            [Fact]
            public void ActualGreaterThanExpectedThrowsArgumentOutOfRangeException()
            {
                var expectedEx = new ArgumentOutOfRangeException("paramName", 1, Exceptions.ArgumentNotLessThanValue.FormatWith(0, 1));
                var actualEx = Assert.Throws<ArgumentOutOfRangeException>(() => Verify.LessThan(0, 1, "paramName"));

                Assert.Equal(expectedEx.Message, actualEx.Message);
            }

            [Fact]
            public void ActualEqualToExpectedThrowsArgumentOutOfRangeException()
            {
                var expectedEx = new ArgumentOutOfRangeException("paramName", 1, Exceptions.ArgumentNotLessThanValue.FormatWith(1));
                var actualEx = Assert.Throws<ArgumentOutOfRangeException>(() => Verify.LessThan(1, 1, "paramName"));

                Assert.Equal(expectedEx.Message, actualEx.Message);
            }

            [Fact]
            public void ActualLessThanExpectedDoesNotThrowException()
            {
                Verify.LessThan(2, 1, "paramName");
            }

            private class Comparable : IComparable
            {
                private readonly Int32 value;

                private Comparable(Int32 value)
                {
                    this.value = value;
                }

                public Int32 CompareTo(Object obj)
                {
                    return value.CompareTo(obj);
                }

                public override string ToString()
                {
                    return value.ToString(CultureInfo.InvariantCulture);
                }

                public static implicit operator Comparable(Int32 value)
                {
                    return new Comparable(value);
                }
            }
        }

        public class WhenCheckingForLessThanOrEqual
        {
            [Fact]
            public void NullValuesThrowArgumentOutOfRangeException()
            {
                var expectedEx = new ArgumentOutOfRangeException("paramName", Exceptions.ArgumentNotLessThanOrEqualToValue.FormatWith(String.Empty));
                var actualEx = Assert.Throws<ArgumentOutOfRangeException>(() => Verify.LessThanOrEqual(default(IComparable), null, "paramName"));

                Assert.Equal(expectedEx.Message, actualEx.Message);
            }

            [Fact]
            public void NullActualValueThrowArgumentOutOfRangeException()
            {
                var expectedEx = new ArgumentOutOfRangeException("paramName", Exceptions.ArgumentNotLessThanOrEqualToValue.FormatWith(0));
                var actualEx = Assert.Throws<ArgumentOutOfRangeException>(() => Verify.LessThanOrEqual((Comparable)0, null, "paramName"));

                Assert.Equal(expectedEx.Message, actualEx.Message);
            }

            [Fact]
            public void ActualGreaterThanExpectedThrowsArgumentOutOfRangeException()
            {
                var expectedEx = new ArgumentOutOfRangeException("paramName", 2, Exceptions.ArgumentNotLessThanOrEqualToValue.FormatWith(1));
                var actualEx = Assert.Throws<ArgumentOutOfRangeException>(() => Verify.LessThanOrEqual(1, 2, "paramName"));

                Assert.Equal(expectedEx.Message, actualEx.Message);
            }

            [Fact]
            public void ActualEqualToExpectedDoesNotThrowException()
            {
                Verify.LessThanOrEqual(1, 1, "paramName");
            }

            [Fact]
            public void ActualLessThanExpectedDoesNotThrowException()
            {
                Verify.LessThanOrEqual(2, 1, "paramName");
            }

            private class Comparable : IComparable
            {
                private readonly Int32 value;

                private Comparable(Int32 value)
                {
                    this.value = value;
                }

                public Int32 CompareTo(Object obj)
                {
                    return value.CompareTo(obj);
                }

                public override string ToString()
                {
                    return value.ToString(CultureInfo.InvariantCulture);
                }

                public static implicit operator Comparable(Int32 value)
                {
                    return new Comparable(value);
                }
            }
        }

        public class WhenCheckingForNull
        {
            [Fact]
            public void NotNullValueDoesNotThrow()
            {
                Verify.NotNull((Object)0, "paramName");
            }

            [Fact]
            public void NotNullReferenceDoesNotThrow()
            {
                Verify.NotNull(new Object(), "paramName");
            }

            [Fact]
            public void NotNullNullableDoesNotThrow()
            {
                Verify.NotNull((Int32?)0, "paramName");
            }

            [Fact]
            public void NullReferenceThrowsArgumentNullException()
            {
                var expectedEx = new ArgumentNullException("paramName");
                var actualEx = Assert.Throws<ArgumentNullException>(() => Verify.NotNull(default(Object), "paramName"));

                Assert.Equal(expectedEx.Message, actualEx.Message);
            }

            [Fact]
            public void NullNullableThrowsArgumentNullException()
            {
                var expectedEx = new ArgumentNullException("paramName");
                var actualEx = Assert.Throws<ArgumentNullException>(() => Verify.NotNull(default(Int32?), "paramName"));

                Assert.Equal(expectedEx.Message, actualEx.Message);
            }
        }

        public class WhenCheckingForNullEmptyOrWhiteSpace
        {
            [Fact]
            public void NotEmptyValueDoesNotThrow()
            {
                Verify.NotNullOrWhiteSpace("Value", "paramName");
            }

            [Theory, InlineData(" "), InlineData("\r"), InlineData("\n"), InlineData("\r\n"), InlineData("\t")]
            public void WhiteSpaceValueThrowsArgumentException(String value)
            {
                var expectedEx = new ArgumentException(Exceptions.MustContainOneNonWhitespaceCharacter, "paramName");
                var actualEx = Assert.Throws<ArgumentException>(() => Verify.NotNullOrWhiteSpace(value, "paramName"));

                Assert.Equal(expectedEx.Message, actualEx.Message);
            }

            [Fact]
            public void EmptyValueThrowsArgumentException()
            {
                var expectedEx = new ArgumentException(Exceptions.MustContainOneNonWhitespaceCharacter, "paramName");
                var actualEx = Assert.Throws<ArgumentException>(() => Verify.NotNullOrWhiteSpace(String.Empty, "paramName"));

                Assert.Equal(expectedEx.Message, actualEx.Message);
            }

            [Fact]
            public void NullValueThrowsArgumentNullException()
            {
                var expectedEx = new ArgumentNullException("paramName");
                var ex = Assert.Throws<ArgumentNullException>(() => Verify.NotNullOrWhiteSpace(default(String), "paramName"));

                Assert.Equal(expectedEx.Message, ex.Message);
            }
        }
    }
}
