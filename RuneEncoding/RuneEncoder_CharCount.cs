﻿/*
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

using System;
using System.Text;

namespace RuneEncoding;

public abstract partial class RuneEncoder : Encoder
{
#if NETSTANDARD2_1_OR_GREATER
    public sealed override unsafe int GetByteCount(ReadOnlySpan<char> chars, bool flush)
    {
        return 0;
    }
#endif

    public sealed override unsafe int GetByteCount(char[] chars, int index, int count
        , bool flush)
    {
        return 0;
    }

    public sealed override unsafe int GetByteCount(char* chars, int count, bool flush)
    {
        return 0;
    }
}
