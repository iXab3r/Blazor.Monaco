using System;
using System.ComponentModel;
using System.Globalization;
using Microsoft.CodeAnalysis;

namespace BlazorMonacoEditor.Roslyn.Scaffolding;

public sealed class DocumentIdTypeConverter : TypeConverter
{
    private static readonly Lazy<DocumentIdTypeConverter> InstanceSupplier = new();

    public static DocumentIdTypeConverter Instance => InstanceSupplier.Value;
    
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        if (destinationType == typeof(string))
        {
            return true;
        }

        return false;
    }

    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        if (sourceType == typeof(string))
        {
            return true;
        }

        return false;
    }

    public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (!CanConvertTo(context, destinationType))
        {
            throw new NotSupportedException($"Conversion from {typeof(DocumentId)} to {destinationType} is not supported");
        }

        if (value is not DocumentId documentId)
        {
            throw new NotSupportedException($"Provided value is expected to be of type {typeof(DocumentId)}, got {value} ({value?.GetType()})");
        }

        return $"{documentId.ProjectId.Id}/{documentId.Id}";
    }

    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (!CanConvertFrom(context, value.GetType()))
        {
            throw new NotSupportedException($"Conversion from {typeof(DocumentId)} to {value.GetType()} is not supported");
        }
        
        if (value is not string documentIdString)
        {
            throw new NotSupportedException($"Provided value is expected to be of type {typeof(string)}, got {value} ({value.GetType()})");
        }

        if (!TryConvert(documentIdString, out var documentId) || documentId == null)
        {
            throw new NotSupportedException($"Failed to supported '{documentIdString}' to {typeof(DocumentId)}");
        }

        return documentId;
    }

    public static bool TryConvert(string documentIdString, out DocumentId? documentId)
    {
        if (string.IsNullOrEmpty(documentIdString))
        {
            documentId = default;
            return false;
        }
        
        var path = Uri.TryCreate(documentIdString, UriKind.Absolute, out var documentUri) ? documentUri.AbsolutePath : documentIdString;
        var segments = path.Trim('/','\\').Split('/', '\\');
        if (segments.Length != 2)
        {
            documentId = default;
            return false;
        }
        
        if (!Guid.TryParse(segments[0].Trim('/', '\\'), out var projectGuid))
        {
            documentId = default;
            return false;
        }

        if (!Guid.TryParse(segments[1].Trim('/', '\\'), out var documentGuid))
        {
            documentId = default;
            return false;
        }

        var projectId = ProjectId.CreateFromSerialized(projectGuid);
        documentId = DocumentId.CreateFromSerialized(projectId, documentGuid);
        return true;
    }
}