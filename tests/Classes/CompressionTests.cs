using System;
using openrmf_msg_template.Classes;
using Xunit;

namespace tests.Classes;

public class CompressionTests
{
    [Fact]
    public void CompressThenDecompress_RoundTripsOriginalText()
    {
        const string input = "Line 1\nLine 2\tTabbed";

        var compressed = Compression.CompressString(input);
        var decompressed = Compression.DecompressString(compressed);

        Assert.False(string.IsNullOrWhiteSpace(compressed));
        Assert.Equal(input, decompressed);
        Assert.NotEqual("wrong", decompressed);
    }

    [Fact]
    public void DecompressString_InvalidBase64_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => Compression.DecompressString("not-base64"));
    }

    [Fact]
    public void DecompressString_InvalidGzipPayload_Throws()
    {
        var invalidCompressed = Convert.ToBase64String(new byte[] { 1, 0, 0, 0, 120, 121, 122 });

        Assert.ThrowsAny<Exception>(() => Compression.DecompressString(invalidCompressed));
    }
}
