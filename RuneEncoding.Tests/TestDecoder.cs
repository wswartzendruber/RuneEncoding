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

public class TestDecoder : RuneDecoder
{
    public List<byte> CountBytes = new();
    public List<byte> DecodeBytes = new();
    public List<int> CountIncrements = new();
    public List<int> DecodeIncrements = new();

    private byte[] Buffer = new byte[2];
    private int BufferIndex = 0;

    public void AssertConsistency()
    {
        if (!CountBytes.SequenceEqual(DecodeBytes))
            throw new InvalidOperationException("CountBytes != DecodeBytes");
        if (!CountIncrements.SequenceEqual(DecodeIncrements))
            throw new InvalidOperationException("CountIncrements != DecodeIncrements");
    }

    protected override void ResetState()
    {
        Buffer[0] = 0;
        Buffer[1] = 0;
        BufferIndex = 0;
        CountBytes.Clear();
        DecodeBytes.Clear();
        CountIncrements.Clear();
        DecodeIncrements.Clear();
    }

    protected override unsafe int AssessScalarValue(byte* bytes, int count, bool first
        , out bool? isBMP)
    {
        isBMP = null;

        var buffer = new byte[2];
        var bufferIndex = first ? BufferIndex : 0;
        var bytesRead = 0;

        if (first)
        {
            buffer[0] = Buffer[0];
            buffer[1] = Buffer[1];
        }

        for (int byteIndex = 0; byteIndex < count; byteIndex++)
        {
            switch (bufferIndex)
            {
                case 0:
                    bytesRead++;
                    isBMP = null;
                    buffer[0] = bytes[byteIndex];
                    CountBytes.Add(bytes[byteIndex]);
                    bufferIndex = 1;
                    break;
                case 1:
                    bytesRead++;
                    isBMP = null;
                    buffer[1] = bytes[byteIndex];
                    CountBytes.Add(bytes[byteIndex]);
                    bufferIndex = 2;
                    break;
                case 2:
                    bytesRead++;
                    var scalarValue = (buffer[0] << 16) | (buffer[1] << 8) | bytes[byteIndex];
                    CountBytes.Add(bytes[byteIndex]);
                    bufferIndex = 0;
                    if ((0x00 <= scalarValue && scalarValue <= 0xFFFF)
                        || (0x110000 <= scalarValue))
                    {
                        isBMP = true;
                    }
                    else
                    {
                        isBMP = false;
                    }
                    return bytesRead;
                default:
                    throw new InvalidOperationException("Internal state is irrational.");
            }
        }

        return bytesRead;
    }

    protected override unsafe int DecodeScalarValue(byte* bytes, int count
        , out int? scalarValue)
    {
        scalarValue = null;

        var bytesRead = 0;

        for (int byteIndex = 0; byteIndex < count; byteIndex++)
        {
            switch (BufferIndex)
            {
                case 0:
                    bytesRead++;
                    scalarValue = null;
                    Buffer[0] = bytes[byteIndex];
                    DecodeBytes.Add(bytes[byteIndex]);
                    BufferIndex = 1;
                    break;
                case 1:
                    bytesRead++;
                    scalarValue = null;
                    Buffer[1] = bytes[byteIndex];
                    DecodeBytes.Add(bytes[byteIndex]);
                    BufferIndex = 2;
                    break;
                case 2:
                    bytesRead++;
                    scalarValue = (Buffer[0] << 16) | (Buffer[1] << 8) | bytes[byteIndex];
                    DecodeBytes.Add(bytes[byteIndex]);
                    BufferIndex = 0;
                    return bytesRead;
                default:
                    throw new InvalidOperationException("Internal state is irrational.");
            }
        }

        return bytesRead;
    }
}
