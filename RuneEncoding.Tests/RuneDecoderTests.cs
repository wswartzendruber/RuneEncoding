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
}
