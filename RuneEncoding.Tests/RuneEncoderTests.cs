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
    public void ByteLengthsAllScalarValuesFlush()
    {
        var totalString = new StringBuilder();
        var encoder = new TestEncoder();

        foreach (int value in Common.AllScalarValues)
            totalString.Append(char.ConvertFromUtf32(value));

        var byteCount = encoder.GetByteCount(totalString.ToString(), true);

        Assert.True(byteCount == Common.AllScalarValues.Count());
        Assert.True(encoder.ByteCounts.SequenceEqual(Common.AllScalarValues));
    }

    [Fact]
    public void ByteLengthsAllScalarValuesNoFlush()
    {
        var totalString = new StringBuilder();
        var encoder = new TestEncoder();

        foreach (int value in Common.AllScalarValues)
            totalString.Append(char.ConvertFromUtf32(value));

        var byteCount = encoder.GetByteCount(totalString.ToString(), false);

        Assert.True(byteCount == Common.AllScalarValues.Count());
        Assert.True(encoder.ByteCounts.SequenceEqual(Common.AllScalarValues));
    }

    [Fact]
    public void ByteLengthsInitialLSFlush()
    {
        var invalidString = " \uDE00";
        var composedList = new List<int> { 0x20, 0xFFFD };
        var encoder = new TestEncoder();

        var byteCount = encoder.GetByteCount(invalidString, true);

        Assert.True(byteCount == 2);
        Assert.True(encoder.ByteCounts.SequenceEqual(composedList));
    }

    [Fact]
    public void ByteLengthsInitialLSNoFlush()
    {
        var invalidString = " \uDE00";
        var composedList = new List<int> { 0x20, 0xFFFD };
        var encoder = new TestEncoder();

        var byteCount = encoder.GetByteCount(invalidString, false);

        Assert.True(byteCount == 2);
        Assert.True(encoder.ByteCounts.SequenceEqual(composedList));
    }

    [Fact]
    public void ByteLengthsUnfollowedHSThenBMPFlush()
    {
        var invalidString = " \uDA00 ";
        var composedList = new List<int> { 0x20, 0xFFFD, 0x20 };
        var encoder = new TestEncoder();

        var byteCount = encoder.GetByteCount(invalidString, true);

        Assert.True(byteCount == 3);
        Assert.True(encoder.ByteCounts.SequenceEqual(composedList));
    }

    [Fact]
    public void ByteLengthsUnfollowedHSThenBMPNoFlush()
    {
        var invalidString = " \uDA00 ";
        var composedList = new List<int> { 0x20, 0xFFFD, 0x20 };
        var encoder = new TestEncoder();

        var byteCount = encoder.GetByteCount(invalidString, false);

        Assert.True(byteCount == 3);
        Assert.True(encoder.ByteCounts.SequenceEqual(composedList));
    }

    [Fact]
    public void ByteLengthsUnfollowedHSThenSMPFlush()
    {
        var invalidString = " \uDA00\uD83D\uDE00";
        var composedList = new List<int> { 0x20, 0xFFFD, 0x1F600 };
        var encoder = new TestEncoder();

        var byteCount = encoder.GetByteCount(invalidString, true);

        Assert.True(byteCount == 3);
        Assert.True(encoder.ByteCounts.SequenceEqual(composedList));
    }

    [Fact]
    public void ByteLengthsUnfollowedHSThenSMPNoFlush()
    {
        var invalidString = " \uDA00\uD83D\uDE00";
        var composedList = new List<int> { 0x20, 0xFFFD, 0x1F600 };
        var encoder = new TestEncoder();

        var byteCount = encoder.GetByteCount(invalidString, false);

        Assert.True(byteCount == 3);
        Assert.True(encoder.ByteCounts.SequenceEqual(composedList));
    }

    [Fact]
    public void ByteLengthsTrailingHSFlush()
    {
        var invalidString = " \uDA00";
        var composedList = new List<int> { 0x20, 0xFFFD };
        var encoder = new TestEncoder();

        var byteCount = encoder.GetByteCount(invalidString, true);

        Assert.True(byteCount == 2);
        Assert.True(encoder.ByteCounts.SequenceEqual(composedList));
    }

    [Fact]
    public void ByteLengthsTrailingHSNoFlush()
    {
        var invalidString = " \uDA00";
        var composedList = new List<int> { 0x20 };
        var encoder = new TestEncoder();

        var byteCount = encoder.GetByteCount(invalidString, false);

        Assert.True(byteCount == 1);
        Assert.True(encoder.ByteCounts.SequenceEqual(composedList));
    }
}
