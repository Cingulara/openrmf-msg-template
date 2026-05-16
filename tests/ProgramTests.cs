using System;
using System.Reflection;
using openrmf_msg_template.Models;
using Xunit;

namespace tests;

public class ProgramTests
{
    private static Type ProgramType => typeof(Template).Assembly.GetType("openrmf_msg_template.Program")!;

    [Fact]
    public void SanitizeString_ReplacesKnownTokens()
    {
        var method = ProgramType.GetMethod("SanitizeString", BindingFlags.NonPublic | BindingFlags.Static);

        var result = (string)method!.Invoke(null, new object[] { "MS Windows STIG SCAP Benchmark" })!;

        Assert.Equal("Windows Security Technical Implementation Guide", result);
        Assert.DoesNotContain("SCAP", result);
    }

    [Fact]
    public void SanitizeFilename_RemovesVersionSuffixWhenPresent()
    {
        var method = ProgramType.GetMethod("SanitizeFilename", BindingFlags.NonPublic | BindingFlags.Static);

        var result = (string)method!.Invoke(null, new object[] { "U_MS_Windows_11_STIG_V1R1_Manual-xccdf.xml" })!;

        Assert.Equal("U_MS_Windows_11_STIG", result);
        Assert.DoesNotContain("_V1", result);
    }

    [Fact]
    public void SanitizeFilename_RemovesManualSuffixWhenVersionPatternIsAbsent()
    {
        var method = ProgramType.GetMethod("SanitizeFilename", BindingFlags.NonPublic | BindingFlags.Static);

        var result = (string)method!.Invoke(null, new object[] { "sample_Manual-xccdf.xml" })!;

        Assert.Equal("sample", result);
        Assert.NotEqual("sample_Manual-xccdf.xml", result);
    }
}
