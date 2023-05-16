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

    public override void Reset()
    {
        Buffer[0] = 0;
        Buffer[1] = 0;
        BufferIndex = 0;
        CountBytes.Clear();
        DecodeBytes.Clear();
        CountIncrements.Clear();
        DecodeIncrements.Clear();
    }

    protected override int IsScalarValueBasic(byte[] bytes, int index, int limit, bool first
        , out bool? isBasic)
    {
        isBasic = null;

        var buffer = new byte[2];
        var bufferIndex = first ? BufferIndex : 0;
        var end = index + limit;
        var bytesRead = 0;

        if (first)
        {
            buffer[0] = Buffer[0];
            buffer[1] = Buffer[1];
        }

        for (int offset = index; offset < end; offset++)
        {
            switch (bufferIndex)
            {
                case 0:
                    bytesRead++;
                    isBasic = null;
                    buffer[0] = bytes[offset];
                    CountBytes.Add(bytes[offset]);
                    bufferIndex = 1;
                    break;
                case 1:
                    bytesRead++;
                    isBasic = null;
                    buffer[1] = bytes[offset];
                    CountBytes.Add(bytes[offset]);
                    bufferIndex = 2;
                    break;
                case 2:
                    bytesRead++;
                    var scalarValue = (buffer[0] << 16) | (buffer[1] << 8) | bytes[offset];
                    CountBytes.Add(bytes[offset]);
                    bufferIndex = 0;
                    if ((0x00 <= scalarValue && scalarValue <= 0xFFFF)
                        || (0x110000 <= scalarValue))
                    {
                        isBasic = true;
                    }
                    else
                    {
                        isBasic = false;
                    }
                    return bytesRead;
                default:
                    throw new InvalidOperationException("Internal state is irrational.");
            }
        }

        return bytesRead;
    }

    protected override int ReadScalarValue(byte[] bytes, int index, int limit
        , out int? scalarValue)
    {
        scalarValue = null;

        var end = index + limit;
        var bytesRead = 0;

        for (int offset = index; offset < end; offset++)
        {
            switch (BufferIndex)
            {
                case 0:
                    bytesRead++;
                    scalarValue = null;
                    Buffer[0] = bytes[offset];
                    DecodeBytes.Add(bytes[offset]);
                    BufferIndex = 1;
                    break;
                case 1:
                    bytesRead++;
                    scalarValue = null;
                    Buffer[1] = bytes[offset];
                    DecodeBytes.Add(bytes[offset]);
                    BufferIndex = 2;
                    break;
                case 2:
                    bytesRead++;
                    scalarValue = (Buffer[0] << 16) | (Buffer[1] << 8) | bytes[offset];
                    DecodeBytes.Add(bytes[offset]);
                    BufferIndex = 0;
                    return bytesRead;
                default:
                    throw new InvalidOperationException("Internal state is irrational.");
            }
        }

        return bytesRead;
    }
}
