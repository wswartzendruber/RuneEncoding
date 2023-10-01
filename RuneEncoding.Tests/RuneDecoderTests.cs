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

using System.Text;

namespace RuneEncoding.Tests;

public class RuneDecoderTests
{
#if TEST_NETSTANDARD2_1
    [Fact]
    public void AllThreeBytesValuesSpanNoFlush()
    {
        var index = 0;
        var decoder = new TestDecoder();
        var random = new Random(1_024);
        var bytesList = new List<byte>();
        var stringBuilder = new StringBuilder();
        var runeValues = new List<int>();

        for (uint value = 0; value < 16_777_216; value++)
        {
            bytesList.Add((byte)(value >> 16));
            bytesList.Add((byte)(value >> 8));
            bytesList.Add((byte)value);
        }

        var bytesArray = bytesList.ToArray();

        while (index < bytesArray.Length)
        {
            var count = Math.Min(random.Next(1_024), bytesArray.Length - index);
            var span = new ReadOnlySpan<byte>(bytesArray, index, count);
            var expectedLength = decoder.GetCharCount(span, false);
            var chars = new char[expectedLength];
            var actualLength = decoder.GetChars(span, chars, false);

            Assert.True(expectedLength == actualLength);

            stringBuilder.Append(chars);

            index += count;
        }

        decoder.AssertConsistency();

        foreach (var rune in stringBuilder.ToString().EnumerateRunes())
            runeValues.Add(rune.Value);

        for (int i = 0; i < 0xD800; i++)
            Assert.True(runeValues[i] == i);

        for (int i = 0xD800; i < 0xE000; i++)
            Assert.True(runeValues[i] == 0xFFFD);

        for (int i = 0xE000; i < 0x110000; i++)
            Assert.True(runeValues[i] == i);

        for (int i = 0x110000; i < 0x1000000; i++)
            Assert.True(runeValues[i] == 0x10FFFF);
    }

    [Fact]
    public void AllThreeBytesValuesSpanFlush()
    {
        var index = 0;
        var decoder = new TestDecoder();
        var random = new Random(1_024);
        var bytesList = new List<byte>();
        var stringBuilder = new StringBuilder();
        var runeValues = new List<int>();

        for (uint value = 0; value < 16_777_216; value++)
        {
            bytesList.Add((byte)(value >> 16));
            bytesList.Add((byte)(value >> 8));
            bytesList.Add((byte)value);
        }

        var bytesArray = bytesList.ToArray();

        while (index < bytesArray.Length)
        {
            var count = Math.Min(random.Next(1_024), bytesArray.Length - index);
            var span = new ReadOnlySpan<byte>(bytesArray, index, count);
            var expectedLength = decoder.GetCharCount(span, true);
            var chars = new char[expectedLength];
            var actualLength = decoder.GetChars(span, chars, true);

            Assert.True(expectedLength == actualLength);

            stringBuilder.Append(chars);

            index += count;
        }

        decoder.AssertConsistency();
    }

    [Fact]
    public void NoCharsNoTrailingBytesSpanNoFlush()
    {
        var decoder = new TestDecoder
        {
            Fallback = new PassthruTestDecoderFallback(),
        };
        var bytes = new byte[] { };
        var expectedChars = new char[] { };
        var expectedLength = decoder.GetCharCount(bytes, false);
        var chars = new char[expectedLength];
        var actualLength = decoder.GetChars(bytes, chars, false);

        decoder.AssertConsistency();

        Assert.True(actualLength == expectedChars.Length);
        Assert.True(chars.SequenceEqual(expectedChars));
    }

    [Fact]
    public void NoCharsNoTrailingBytesSpanFlush()
    {
        var decoder = new TestDecoder
        {
            Fallback = new PassthruTestDecoderFallback(),
        };
        var bytes = new byte[] { };
        var expectedChars = new char[] { };
        var expectedLength = decoder.GetCharCount(bytes, true);
        var chars = new char[expectedLength];
        var actualLength = decoder.GetChars(bytes, chars, true);

        decoder.AssertConsistency();

        Assert.True(actualLength == expectedChars.Length);
        Assert.True(chars.SequenceEqual(expectedChars));
    }

    [Fact]
    public void NoCharsOneTrailingByteSpanNoFlush()
    {
        var decoder = new TestDecoder
        {
            Fallback = new PassthruTestDecoderFallback(),
        };
        var bytes = new byte[] { 0x56 };
        var expectedChars = new char[] { };
        var expectedLength = decoder.GetCharCount(bytes, false);
        var chars = new char[expectedLength];
        var actualLength = decoder.GetChars(bytes, chars, false);

        decoder.AssertConsistency();

        Assert.True(actualLength == expectedChars.Length);
        Assert.True(chars.SequenceEqual(expectedChars));
    }

    [Fact]
    public void NoCharsOneTrailingByteSpanFlush()
    {
        var decoder = new TestDecoder
        {
            Fallback = new PassthruTestDecoderFallback(),
        };
        var bytes = new byte[] { 0x56 };
        var expectedChars = new char[] { '\u0056' };
        var expectedLength = decoder.GetCharCount(bytes, true);
        var chars = new char[expectedLength];
        var actualLength = decoder.GetChars(bytes, chars, true);

        decoder.AssertConsistency();

        Assert.True(actualLength == expectedChars.Length);
        Assert.True(chars.SequenceEqual(expectedChars));
    }

    [Fact]
    public void NoCharsTwoTrailingBytesSpanNoFlush()
    {
        var decoder = new TestDecoder
        {
            Fallback = new PassthruTestDecoderFallback(),
        };
        var bytes = new byte[] { 0x56, 0x78 };
        var expectedChars = new char[] { };
        var expectedLength = decoder.GetCharCount(bytes, false);
        var chars = new char[expectedLength];
        var actualLength = decoder.GetChars(bytes, chars, false);

        decoder.AssertConsistency();

        Assert.True(actualLength == expectedChars.Length);
        Assert.True(chars.SequenceEqual(expectedChars));
    }

    [Fact]
    public void NoCharsTwoTrailingBytesSpanFlush()
    {
        var decoder = new TestDecoder
        {
            Fallback = new PassthruTestDecoderFallback(),
        };
        var bytes = new byte[] { 0x56, 0x78 };
        var expectedChars = new char[] { '\u0056', '\u0078' };
        var expectedLength = decoder.GetCharCount(bytes, true);
        var chars = new char[expectedLength];
        var actualLength = decoder.GetChars(bytes, chars, true);

        decoder.AssertConsistency();

        Assert.True(actualLength == expectedChars.Length);
        Assert.True(chars.SequenceEqual(expectedChars));
    }

    [Fact]
    public void OneCharNoTrailingBytesSpanNoFlush()
    {
        var decoder = new TestDecoder
        {
            Fallback = new PassthruTestDecoderFallback(),
        };
        var bytes = new byte[] { 0x00, 0x12, 0x34 };
        var expectedChars = new char[] { '\u1234' };
        var expectedLength = decoder.GetCharCount(bytes, false);
        var chars = new char[expectedLength];
        var actualLength = decoder.GetChars(bytes, chars, false);

        decoder.AssertConsistency();

        Assert.True(actualLength == expectedChars.Length);
        Assert.True(chars.SequenceEqual(expectedChars));
    }

    [Fact]
    public void OneCharNoTrailingBytesSpanFlush()
    {
        var decoder = new TestDecoder
        {
            Fallback = new PassthruTestDecoderFallback(),
        };
        var bytes = new byte[] { 0x00, 0x12, 0x34 };
        var expectedChars = new char[] { '\u1234' };
        var expectedLength = decoder.GetCharCount(bytes, true);
        var chars = new char[expectedLength];
        var actualLength = decoder.GetChars(bytes, chars, true);

        decoder.AssertConsistency();

        Assert.True(actualLength == expectedChars.Length);
        Assert.True(chars.SequenceEqual(expectedChars));
    }

    [Fact]
    public void OneCharOneTrailingByteSpanNoFlush()
    {
        var decoder = new TestDecoder
        {
            Fallback = new PassthruTestDecoderFallback(),
        };
        var bytes = new byte[] { 0x00, 0x12, 0x34, 0x56 };
        var expectedChars = new char[] { '\u1234' };
        var expectedLength = decoder.GetCharCount(bytes, false);
        var chars = new char[expectedLength];
        var actualLength = decoder.GetChars(bytes, chars, false);

        decoder.AssertConsistency();

        Assert.True(actualLength == expectedChars.Length);
        Assert.True(chars.SequenceEqual(expectedChars));
    }

    [Fact]
    public void OneCharOneTrailingByteSpanFlush()
    {
        var decoder = new TestDecoder
        {
            Fallback = new PassthruTestDecoderFallback(),
        };
        var bytes = new byte[] { 0x00, 0x12, 0x34, 0x56 };
        var expectedChars = new char[] { '\u1234', '\u0056' };
        var expectedLength = decoder.GetCharCount(bytes, true);
        var chars = new char[expectedLength];
        var actualLength = decoder.GetChars(bytes, chars, true);

        decoder.AssertConsistency();

        Assert.True(actualLength == expectedChars.Length);
        Assert.True(chars.SequenceEqual(expectedChars));
    }

    [Fact]
    public void OneCharTwoTrailingBytesSpanNoFlush()
    {
        var decoder = new TestDecoder
        {
            Fallback = new PassthruTestDecoderFallback(),
        };
        var bytes = new byte[] { 0x00, 0x12, 0x34, 0x56, 0x78 };
        var expectedChars = new char[] { '\u1234' };
        var expectedLength = decoder.GetCharCount(bytes, false);
        var chars = new char[expectedLength];
        var actualLength = decoder.GetChars(bytes, chars, false);

        decoder.AssertConsistency();

        Assert.True(actualLength == expectedChars.Length);
        Assert.True(chars.SequenceEqual(expectedChars));
    }

    [Fact]
    public void OneCharTwoTrailingBytesSpanFlush()
    {
        var decoder = new TestDecoder
        {
            Fallback = new PassthruTestDecoderFallback(),
        };
        var bytes = new byte[] { 0x00, 0x12, 0x34, 0x56, 0x78 };
        var expectedChars = new char[] { '\u1234', '\u0056', '\u0078' };
        var expectedLength = decoder.GetCharCount(bytes, true);
        var chars = new char[expectedLength];
        var actualLength = decoder.GetChars(bytes, chars, true);

        decoder.AssertConsistency();

        Assert.True(actualLength == expectedChars.Length);
        Assert.True(chars.SequenceEqual(expectedChars));
    }
#endif

    [Fact]
    public void AllThreeBytesValuesArray()
    {
        var index = 0;
        var decoder = new TestDecoder();
        var random = new Random(1_024);
        var bytesList = new List<byte>();
        var stringBuilder = new StringBuilder();
        var runeValues = new List<int>();

        for (uint value = 0; value < 16_777_216; value++)
        {
            bytesList.Add((byte)(value >> 16));
            bytesList.Add((byte)(value >> 8));
            bytesList.Add((byte)value);
        }

        var bytesArray = bytesList.ToArray();

        while (index < bytesArray.Length)
        {
            var count = Math.Min(random.Next(1_024), bytesArray.Length - index);
            var expectedLength = decoder.GetCharCount(bytesArray, index, count);
            var chars = new char[expectedLength];
            var actualLength = decoder.GetChars(bytesArray, index, count, chars, 0);

            Assert.True(expectedLength == actualLength);

            stringBuilder.Append(chars);

            index += count;
        }

        decoder.AssertConsistency();

        foreach (var rune in stringBuilder.ToString().EnumerateRunes())
            runeValues.Add(rune.Value);

        for (int i = 0; i < 0xD800; i++)
            Assert.True(runeValues[i] == i);

        for (int i = 0xD800; i < 0xE000; i++)
            Assert.True(runeValues[i] == 0xFFFD);

        for (int i = 0xE000; i < 0x110000; i++)
            Assert.True(runeValues[i] == i);

        for (int i = 0x110000; i < 0x1000000; i++)
            Assert.True(runeValues[i] == 0x10FFFF);
    }

    [Fact]
    public void AllThreeBytesValuesArrayNoFlush()
    {
        var index = 0;
        var decoder = new TestDecoder();
        var random = new Random(1_024);
        var bytesList = new List<byte>();
        var stringBuilder = new StringBuilder();
        var runeValues = new List<int>();

        for (uint value = 0; value < 16_777_216; value++)
        {
            bytesList.Add((byte)(value >> 16));
            bytesList.Add((byte)(value >> 8));
            bytesList.Add((byte)value);
        }

        var bytesArray = bytesList.ToArray();

        while (index < bytesArray.Length)
        {
            var count = Math.Min(random.Next(1_024), bytesArray.Length - index);
            var expectedLength = decoder.GetCharCount(bytesArray, index, count, false);
            var chars = new char[expectedLength];
            var actualLength = decoder.GetChars(bytesArray, index, count, chars, 0, false);

            Assert.True(expectedLength == actualLength);

            stringBuilder.Append(chars);

            index += count;
        }

        decoder.AssertConsistency();

        foreach (var rune in stringBuilder.ToString().EnumerateRunes())
            runeValues.Add(rune.Value);

        for (int i = 0; i < 0xD800; i++)
            Assert.True(runeValues[i] == i);

        for (int i = 0xD800; i < 0xE000; i++)
            Assert.True(runeValues[i] == 0xFFFD);

        for (int i = 0xE000; i < 0x110000; i++)
            Assert.True(runeValues[i] == i);

        for (int i = 0x110000; i < 0x1000000; i++)
            Assert.True(runeValues[i] == 0x10FFFF);
    }

    [Fact]
    public void AllThreeBytesValuesArrayFlush()
    {
        var index = 0;
        var decoder = new TestDecoder();
        var random = new Random(1_024);
        var bytesList = new List<byte>();
        var stringBuilder = new StringBuilder();
        var runeValues = new List<int>();

        for (uint value = 0; value < 16_777_216; value++)
        {
            bytesList.Add((byte)(value >> 16));
            bytesList.Add((byte)(value >> 8));
            bytesList.Add((byte)value);
        }

        var bytesArray = bytesList.ToArray();

        while (index < bytesArray.Length)
        {
            var count = Math.Min(random.Next(1_024), bytesArray.Length - index);
            var expectedLength = decoder.GetCharCount(bytesArray, index, count, true);
            var chars = new char[expectedLength];
            var actualLength = decoder.GetChars(bytesArray, index, count, chars, 0, true);

            Assert.True(expectedLength == actualLength);

            stringBuilder.Append(chars);

            index += count;
        }

        decoder.AssertConsistency();
    }

    [Fact]
    public void NoCharsNoTrailingBytesArrayNoFlush()
    {
        var decoder = new TestDecoder
        {
            Fallback = new PassthruTestDecoderFallback(),
        };
        var bytes = new byte[] { };
        var expectedChars = new char[] { };
        var expectedLength = decoder.GetCharCount(bytes, 0, bytes.Length, false);
        var chars = new char[expectedLength];
        var actualLength = decoder.GetChars(bytes, 0, bytes.Length, chars, 0, false);

        decoder.AssertConsistency();

        Assert.True(actualLength == expectedChars.Length);
        Assert.True(chars.SequenceEqual(expectedChars));
    }

    [Fact]
    public void NoCharsNoTrailingBytesArrayFlush()
    {
        var decoder = new TestDecoder
        {
            Fallback = new PassthruTestDecoderFallback(),
        };
        var bytes = new byte[] { };
        var expectedChars = new char[] { };
        var expectedLength = decoder.GetCharCount(bytes,0, bytes.Length,  true);
        var chars = new char[expectedLength];
        var actualLength = decoder.GetChars(bytes, 0, bytes.Length, chars, 0, true);

        decoder.AssertConsistency();

        Assert.True(actualLength == expectedChars.Length);
        Assert.True(chars.SequenceEqual(expectedChars));
    }

    [Fact]
    public void NoCharsOneTrailingByteArrayNoFlush()
    {
        var decoder = new TestDecoder
        {
            Fallback = new PassthruTestDecoderFallback(),
        };
        var bytes = new byte[] { 0x56 };
        var expectedChars = new char[] { };
        var expectedLength = decoder.GetCharCount(bytes, 0, bytes.Length, false);
        var chars = new char[expectedLength];
        var actualLength = decoder.GetChars(bytes, 0, bytes.Length, chars, 0, false);

        decoder.AssertConsistency();

        Assert.True(actualLength == expectedChars.Length);
        Assert.True(chars.SequenceEqual(expectedChars));
    }

    [Fact]
    public void NoCharsOneTrailingByteArrayFlush()
    {
        var decoder = new TestDecoder
        {
            Fallback = new PassthruTestDecoderFallback(),
        };
        var bytes = new byte[] { 0x56 };
        var expectedChars = new char[] { '\u0056' };
        var expectedLength = decoder.GetCharCount(bytes,0, bytes.Length,  true);
        var chars = new char[expectedLength];
        var actualLength = decoder.GetChars(bytes, 0, bytes.Length, chars, 0, true);

        decoder.AssertConsistency();

        Assert.True(actualLength == expectedChars.Length);
        Assert.True(chars.SequenceEqual(expectedChars));
    }

    [Fact]
    public void NoCharsTwoTrailingBytesArrayNoFlush()
    {
        var decoder = new TestDecoder
        {
            Fallback = new PassthruTestDecoderFallback(),
        };
        var bytes = new byte[] { 0x56, 0x78 };
        var expectedChars = new char[] { };
        var expectedLength = decoder.GetCharCount(bytes, 0, bytes.Length, false);
        var chars = new char[expectedLength];
        var actualLength = decoder.GetChars(bytes, 0, bytes.Length, chars, 0, false);

        decoder.AssertConsistency();

        Assert.True(actualLength == expectedChars.Length);
        Assert.True(chars.SequenceEqual(expectedChars));
    }

    [Fact]
    public void NoCharsTwoTrailingBytesArrayFlush()
    {
        var decoder = new TestDecoder
        {
            Fallback = new PassthruTestDecoderFallback(),
        };
        var bytes = new byte[] { 0x56, 0x78 };
        var expectedChars = new char[] { '\u0056', '\u0078' };
        var expectedLength = decoder.GetCharCount(bytes,0, bytes.Length,  true);
        var chars = new char[expectedLength];
        var actualLength = decoder.GetChars(bytes, 0, bytes.Length, chars, 0, true);

        decoder.AssertConsistency();

        Assert.True(actualLength == expectedChars.Length);
        Assert.True(chars.SequenceEqual(expectedChars));
    }

    [Fact]
    public void OneCharNoTrailingBytesArrayNoFlush()
    {
        var decoder = new TestDecoder
        {
            Fallback = new PassthruTestDecoderFallback(),
        };
        var bytes = new byte[] { 0x00, 0x12, 0x34 };
        var expectedChars = new char[] { '\u1234' };
        var expectedLength = decoder.GetCharCount(bytes, 0, bytes.Length, false);
        var chars = new char[expectedLength];
        var actualLength = decoder.GetChars(bytes, 0, bytes.Length, chars, 0, false);

        decoder.AssertConsistency();

        Assert.True(actualLength == expectedChars.Length);
        Assert.True(chars.SequenceEqual(expectedChars));
    }

    [Fact]
    public void OneCharNoTrailingBytesArrayFlush()
    {
        var decoder = new TestDecoder
        {
            Fallback = new PassthruTestDecoderFallback(),
        };
        var bytes = new byte[] { 0x00, 0x12, 0x34 };
        var expectedChars = new char[] { '\u1234' };
        var expectedLength = decoder.GetCharCount(bytes,0, bytes.Length,  true);
        var chars = new char[expectedLength];
        var actualLength = decoder.GetChars(bytes, 0, bytes.Length, chars, 0, true);

        decoder.AssertConsistency();

        Assert.True(actualLength == expectedChars.Length);
        Assert.True(chars.SequenceEqual(expectedChars));
    }

    [Fact]
    public void OneCharOneTrailingByteArrayNoFlush()
    {
        var decoder = new TestDecoder
        {
            Fallback = new PassthruTestDecoderFallback(),
        };
        var bytes = new byte[] { 0x00, 0x12, 0x34, 0x56 };
        var expectedChars = new char[] { '\u1234' };
        var expectedLength = decoder.GetCharCount(bytes, 0, bytes.Length, false);
        var chars = new char[expectedLength];
        var actualLength = decoder.GetChars(bytes, 0, bytes.Length, chars, 0, false);

        decoder.AssertConsistency();

        Assert.True(actualLength == expectedChars.Length);
        Assert.True(chars.SequenceEqual(expectedChars));
    }

    [Fact]
    public void OneCharOneTrailingByteArrayFlush()
    {
        var decoder = new TestDecoder
        {
            Fallback = new PassthruTestDecoderFallback(),
        };
        var bytes = new byte[] { 0x00, 0x12, 0x34, 0x56 };
        var expectedChars = new char[] { '\u1234', '\u0056' };
        var expectedLength = decoder.GetCharCount(bytes,0, bytes.Length,  true);
        var chars = new char[expectedLength];
        var actualLength = decoder.GetChars(bytes, 0, bytes.Length, chars, 0, true);

        decoder.AssertConsistency();

        Assert.True(actualLength == expectedChars.Length);
        Assert.True(chars.SequenceEqual(expectedChars));
    }

    [Fact]
    public void OneCharTwoTrailingBytesArrayNoFlush()
    {
        var decoder = new TestDecoder
        {
            Fallback = new PassthruTestDecoderFallback(),
        };
        var bytes = new byte[] { 0x00, 0x12, 0x34, 0x56, 0x78 };
        var expectedChars = new char[] { '\u1234' };
        var expectedLength = decoder.GetCharCount(bytes, 0, bytes.Length, false);
        var chars = new char[expectedLength];
        var actualLength = decoder.GetChars(bytes, 0, bytes.Length, chars, 0, false);

        decoder.AssertConsistency();

        Assert.True(actualLength == expectedChars.Length);
        Assert.True(chars.SequenceEqual(expectedChars));
    }

    [Fact]
    public void OneCharTwoTrailingBytesArrayFlush()
    {
        var decoder = new TestDecoder
        {
            Fallback = new PassthruTestDecoderFallback(),
        };
        var bytes = new byte[] { 0x00, 0x12, 0x34, 0x56, 0x78 };
        var expectedChars = new char[] { '\u1234', '\u0056', '\u0078' };
        var expectedLength = decoder.GetCharCount(bytes,0, bytes.Length,  true);
        var chars = new char[expectedLength];
        var actualLength = decoder.GetChars(bytes, 0, bytes.Length, chars, 0, true);

        decoder.AssertConsistency();

        Assert.True(actualLength == expectedChars.Length);
        Assert.True(chars.SequenceEqual(expectedChars));
    }
}
