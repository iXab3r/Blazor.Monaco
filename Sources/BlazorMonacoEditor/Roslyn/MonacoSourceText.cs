using System;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis.Text;

namespace BlazorMonacoEditor.Roslyn;



internal sealed class MonacoSourceText : SourceText
{
    public MonacoSourceText(
        SourceText source,
        SourceTextContainer container,
        ImmutableArray<byte> checksum = default,
        SourceHashAlgorithm checksumAlgorithm = SourceHashAlgorithm.Sha1)
        : base(checksum: checksum, checksumAlgorithm, container)
    {
        Source = source;
    }

    public override Encoding? Encoding => Source.Encoding;

    /// <summary>
    /// Underlying string which is the source of this <see cref="StringText"/>instance
    /// </summary>
    public SourceText Source { get; }

    /// <summary>
    /// The length of the text represented by <see cref="StringText"/>.
    /// </summary>
    public override int Length => Source.Length;

    /// <summary>
    /// Returns a character at given position.
    /// </summary>
    /// <param name="position">The position to get the character from.</param>
    /// <returns>The character.</returns>
    /// <exception cref="ArgumentOutOfRangeException">When position is negative or 
    /// greater than <see cref="Length"/>.</exception>
    public override char this[int position] => Source[position];

    /// <summary>
    /// Provides a string representation of the StringText located within given span.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">When given span is outside of the text range.</exception>
    public override string ToString(TextSpan span)
    {
        return Source.ToString(span);
    }

    public override void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
    {
        Source.CopyTo(sourceIndex, destination, destinationIndex, count);
    }

    public override void Write(TextWriter textWriter, TextSpan span, CancellationToken cancellationToken = default(CancellationToken))
    {
        Source.Write(textWriter, span, cancellationToken);
    }
}