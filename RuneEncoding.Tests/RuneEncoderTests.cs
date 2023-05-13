/*
 * Copyright 2023 William Swartzendruber
 *
 * To the extent possible under law, the person who associated CC0 with this file has waived all
 * copyright and related or neighboring rights to this file.
 *
 * You should have received a copy of the CC0 legalcode along with this work. If not, see
 * <http://creativecommons.org/publicdomain/zero/1.0/>.
 *
 * SPDX-License-Identifier: CC0-1.0
 */

namespace RuneEncoding.Tests;

public class RuneEncoderTests
{
    private readonly byte[] EmptyBuffer = new byte[0];

    [Fact]
    public void AllCodePointsTransformFallback()
    {
        var encoder = new TestEncoder();

        encoder.Fallback = new TransformTestEncoderFallback();

        for (int value = 0x0000; value <= 0xFFFF; value++)
        {
            var chars = new char[] { (char)value };

            encoder.GetByteCount(chars, true);
            encoder.GetBytes(chars, EmptyBuffer, true);
        }
        for (int value = 0x010000; value <= 0x10FFFF; value++)
        {
            var chars = char.ConvertFromUtf32(value);

            encoder.GetByteCount(chars, true);
            encoder.GetBytes(chars, EmptyBuffer, true);
        }

        encoder.AssertConsistency();

        for (int index = 0x0000; index <= 0xD7FF; index++)
            Assert.True(encoder.EncodeValues[index] == index);
        for (int index = 0xD800; index <= 0xDFFF; index++)
            Assert.True(encoder.EncodeValues[index] == index >> 8);
        for (int index = 0xE000; index <= 0x10FFFF; index++)
            Assert.True(encoder.EncodeValues[index] == index);

        encoder.Reset();
    }

    [Fact]
    public void AllCodePointsPassthruFallback()
    {
        var encoder = new TestEncoder();
        var random = new Random(29_749_555);

        encoder.Fallback = new PassthruTestEncoderFallback();

        for (int value = 0x0000; value <= 0xD7FF; value++)
        {
            var chars = new char[] { (char)value };

            encoder.GetByteCount(chars, true);
            encoder.GetBytes(chars, EmptyBuffer, true);
        }
        for (int value = 0xD800; value <= 0xDFFF; value++)
        {
            var chars = new char[] { (char)(0xD800 + random.Next(0x800)) };

            encoder.GetByteCount(chars, true);
            encoder.GetBytes(chars, EmptyBuffer, true);
        }
        for (int value = 0xE000; value <= 0xFFFF; value++)
        {
            var chars = new char[] { (char)value };

            encoder.GetByteCount(chars, true);
            encoder.GetBytes(chars, EmptyBuffer, true);
        }
        for (int value = 0x010000; value <= 0x10FFFF; value++)
        {
            var chars = char.ConvertFromUtf32(value);

            encoder.GetByteCount(chars, true);
            encoder.GetBytes(chars, EmptyBuffer, true);
        }

        encoder.AssertConsistency();

        for (int index = 0x0000; index <= 0xD7FF; index++)
            Assert.True(encoder.EncodeValues[index] == index);
        for (int index = 0xD800; index <= 0xDFFF; index++)
            Assert.True(encoder.EncodeValues[index] == 0xFFFD);
        for (int index = 0xE000; index <= 0x10FFFF; index++)
            Assert.True(encoder.EncodeValues[index] == index);

        encoder.Reset();
    }

    [Fact]
    public void AllCodePointsCompoundFallback()
    {
        var encoder = new TestEncoder();

        encoder.Fallback = new CompoundTestEncoderFallback();

        for (int value = 0x0000; value <= 0xFFFF; value++)
        {
            var chars = new char[] { (char)value };

            encoder.GetByteCount(chars, true);
            encoder.GetBytes(chars, EmptyBuffer, true);
        }
        for (int value = 0x010000; value <= 0x10FFFF; value++)
        {
            var chars = char.ConvertFromUtf32(value);

            encoder.GetByteCount(chars, true);
            encoder.GetBytes(chars, EmptyBuffer, true);
        }

        encoder.AssertConsistency();

        for (int index = 0x0000; index <= 0xD7FF; index++)
        {
            Assert.True(encoder.EncodeValues[index] == index);
        }
        for (int index = 0xD800; index <= 0xDFFF; index++)
        {
            var char1 = (char)(0xD800 | (index >> 8));
            var char2 = (char)(0xDC00 | (index & 0xFF));

            Assert.True(encoder.EncodeValues[index] == char.ConvertToUtf32(char1, char2));
        }
        for (int index = 0xE000; index <= 0x10FFFF; index++)
        {
            Assert.True(encoder.EncodeValues[index] == index);
        }

        encoder.Reset();
    }

    [Fact]
    public void HsBmpHsFlush()
    {
        var invalidString = " \uD800 \uD800";
        var composedList = new List<int> { 0x20, 0xFFFD, 0x20, 0xFFFD };
        var encoder = new TestEncoder();

        var byteCount = encoder.GetByteCount(invalidString, true);
        var buffer = new byte[byteCount];

        encoder.GetBytes(invalidString, buffer, true);

        encoder.AssertConsistency();
        Assert.False(encoder.Pending);
        Assert.True(encoder.EncodeValues.SequenceEqual(composedList));
    }

    [Fact]
    public void HsBmpHsNoFlush()
    {
        var invalidString = " \uD800 \uD800";
        var composedList = new List<int> { 0x20, 0xFFFD, 0x20 };
        var encoder = new TestEncoder();

        var byteCount = encoder.GetByteCount(invalidString, false);
        var buffer = new byte[byteCount];

        encoder.GetBytes(invalidString, buffer, false);

        encoder.AssertConsistency();
        Assert.True(encoder.Pending);
        Assert.True(encoder.EncodeValues.SequenceEqual(composedList));
    }

    [Fact]
    public void HsHsFlush()
    {
        var invalidString = " \uD800\uD800";
        var composedList = new List<int> { 0x20, 0xFFFD, 0xFFFD };
        var encoder = new TestEncoder();

        var byteCount = encoder.GetByteCount(invalidString, true);
        var buffer = new byte[byteCount];

        encoder.GetBytes(invalidString, buffer, true);

        encoder.AssertConsistency();
        Assert.False(encoder.Pending);
        Assert.True(encoder.EncodeValues.SequenceEqual(composedList));
    }

    [Fact]
    public void HsHsNoFlush()
    {
        var invalidString = " \uD800\uD800";
        var composedList = new List<int> { 0x20, 0xFFFD };
        var encoder = new TestEncoder();

        var byteCount = encoder.GetByteCount(invalidString, false);
        var buffer = new byte[byteCount];

        encoder.GetBytes(invalidString, buffer, false);

        encoder.AssertConsistency();
        Assert.True(encoder.Pending);
        Assert.True(encoder.EncodeValues.SequenceEqual(composedList));
    }

    [Fact]
    public void LsFlush()
    {
        var invalidString = " \uDC00";
        var composedList = new List<int> { 0x20, 0xFFFD };
        var encoder = new TestEncoder();

        var byteCount = encoder.GetByteCount(invalidString, true);
        var buffer = new byte[byteCount];

        encoder.GetBytes(invalidString, buffer, true);

        encoder.AssertConsistency();
        Assert.False(encoder.Pending);
        Assert.True(encoder.EncodeValues.SequenceEqual(composedList));
    }

    [Fact]
    public void LsNoFlush()
    {
        var invalidString = " \uDC00";
        var composedList = new List<int> { 0x20, 0xFFFD };
        var encoder = new TestEncoder();

        var byteCount = encoder.GetByteCount(invalidString, false);
        var buffer = new byte[byteCount];

        encoder.GetBytes(invalidString, buffer, false);

        encoder.AssertConsistency();
        Assert.False(encoder.Pending);
        Assert.True(encoder.EncodeValues.SequenceEqual(composedList));
    }
}
