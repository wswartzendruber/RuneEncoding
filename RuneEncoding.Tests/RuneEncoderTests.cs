/*
 * Copyright 2023 William Swartzendruber
 *
 * To the extent possible under law, the person who associated CC0 with this file has waived
 * all copyright and related or neighboring rights to this file.
 *
 * You should have received a copy of the CC0 legalcode along with this work. If not, see
 * <http://creativecommons.org/publicdomain/zero/1.0/>.
 *
 * SPDX-License-Identifier: CC0-1.0
 */

using System.Text;
using RuneEncoding;

namespace RuneEncoding.Tests;

public class RuneEncoderTests
{
    [Fact]
    public void AllScalarValuesFlush()
    {
        var stringBuilder = new StringBuilder();
        var scalarValues = new Queue<char>(Common.All);
        var encoder = new TestEncoder();

        for (int i = 0; i < Common.All.Length && scalarValues.Count > 0; i++)
        {
            var count = i % 16;

            for (int c = 0; c < count && scalarValues.Count > 0; c++)
                stringBuilder.Append(scalarValues.Dequeue());

            var values = stringBuilder.ToString();
            var byteCount = encoder.GetByteCount(values, true);
            var buffer = new byte[byteCount];

            encoder.GetBytes(values, buffer, true);
            stringBuilder.Clear();
        }

        encoder.AssertConsistency();
    }

    [Fact]
    public void AllScalarValuesNoFlush()
    {
        var stringBuilder = new StringBuilder();
        var scalarValues = new Queue<char>(Common.All);
        var encoder = new TestEncoder();

        for (int i = 0; i < Common.All.Length && scalarValues.Count > 0; i++)
        {
            var count = i % 16;

            for (int c = 0; c < count && scalarValues.Count > 0; c++)
                stringBuilder.Append(scalarValues.Dequeue());

            var values = stringBuilder.ToString();
            var byteCount = encoder.GetByteCount(values, false);
            var buffer = new byte[byteCount];

            encoder.GetBytes(values, buffer, false);
            stringBuilder.Clear();
        }

        encoder.AssertConsistency();
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
        Assert.True(encoder.EncodeValues.SequenceEqual(composedList));
    }
}
