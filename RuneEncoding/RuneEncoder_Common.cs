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

namespace RuneEncoding;

/// <summary>
///     Automatically handles the common aspects of character encoding, such as surrogate
///     composition, thereby allowing implementers to worry about specific concerns only.
/// </summary>
public abstract partial class RuneEncoder : Encoder
{
    private char? HighSurrogate = null;

    /// <summary>
    ///     Gets whether or not a pending high surrogate is buffered in the encoder's state.
    /// </summary>
    /// <returns>
    ///     Whether or not a pending high surrogate is buffered in the encoder's state.
    /// </returns>
    public bool Pending
    {
        get
        {
            return HighSurrogate != null;
        }
    }

    /// <summary>
    ///     Resets the surrogate state of the encoder; any pending high surrogate is cleared.
    ///     Also resets any implementation-specific state.
    /// </summary>
    public override void Reset()
    {
        HighSurrogate = null;
        ResetState();
    }

    /// <summary>
    ///     Resets the implementation-specific state of the encoder.
    /// </summary>
    protected virtual void ResetState() { }
}
