using System.Buffers.Text;
using System.Runtime.CompilerServices;
using RecruitmentTasks.Common;

namespace RecruitmentTasks.Converters;

public static class Base64UrlToGuidConverter
{

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Guid Convert(ReadOnlySpan<char> base64Url)
    {
        if (base64Url.Length != Constants.Encoding.Base64UrlGuidLength)
        {
            throw new ArgumentException(
                $"Base64 URL string must be exactly {Constants.Encoding.Base64UrlGuidLength} characters for a GUID. Provided: {base64Url.Length}",
                nameof(base64Url));
        }

        Span<char> base64 = stackalloc char[Constants.Encoding.Base64PaddedLength];
        Span<byte> guidBytes = stackalloc byte[Constants.Encoding.GuidByteLength];

        for (int i = 0; i < Constants.Encoding.Base64UrlGuidLength; i++)
        {
            char c = base64Url[i];
            base64[i] = c switch
            {
                '-' => '+',
                '_' => '/',
                _ => c
            };
        }

        base64[22] = '=';
        base64[23] = '=';

        Span<byte> base64Bytes = stackalloc byte[Constants.Encoding.Base64PaddedLength];
        for (int i = 0; i < Constants.Encoding.Base64PaddedLength; i++)
        {
            base64Bytes[i] = (byte)base64[i];
        }

        var status = Base64.DecodeFromUtf8(base64Bytes, guidBytes, out _, out int bytesWritten);
        
        if (status != System.Buffers.OperationStatus.Done)
        {
            throw new FormatException("Invalid Base64 URL string format");
        }
        
        if (bytesWritten != Constants.Encoding.GuidByteLength)
        {
            throw new FormatException($"Decoded bytes length mismatch. Expected: {Constants.Encoding.GuidByteLength}, Got: {bytesWritten}");
        }

        return new Guid(guidBytes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int TryConvertToBase64Url(Guid guid, Span<char> destination)
    {
        if (destination.Length < Constants.Encoding.Base64UrlGuidLength)
        {
            throw new ArgumentException(
                $"Destination span must have at least {Constants.Encoding.Base64UrlGuidLength} characters. Provided: {destination.Length}",
                nameof(destination));
        }

        Span<byte> guidBytes = stackalloc byte[Constants.Encoding.GuidByteLength];
        Span<byte> base64Bytes = stackalloc byte[Constants.Encoding.Base64PaddedLength];

        if (!guid.TryWriteBytes(guidBytes))
        {
            throw new InvalidOperationException("Failed to write GUID bytes");
        }
        
        var status = Base64.EncodeToUtf8(guidBytes, base64Bytes, out _, out int bytesWritten);
        
        if (status != System.Buffers.OperationStatus.Done || bytesWritten != Constants.Encoding.Base64PaddedLength)
        {
            throw new InvalidOperationException("Failed to encode GUID to Base64");
        }

        for (int i = 0; i < Constants.Encoding.Base64UrlGuidLength; i++)
        {
            char c = (char)base64Bytes[i];
            destination[i] = c switch
            {
                '+' => '-',
                '/' => '_',
                _ => c
            };
        }

        return Constants.Encoding.Base64UrlGuidLength;
    }
}
