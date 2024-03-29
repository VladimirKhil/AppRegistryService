﻿namespace AppRegistryService.Helpers;

internal static class VersionHelper
{
    internal static int ToInt(this Version version) => version.Major * 1_000_000 + version.Minor * 1_000 + Math.Max(0, version.Build);

    internal static Version CreateVersion(int value)
    {
        var major = value / 1_000_000;
        var minor = (value % 1_000_000) / 1_000;
        var build = value % 1_000;

        return new Version(major, minor, build);
    }

    internal static int ToOSInt(this Version version) => version.Major * 100_000_000 + version.Minor * 100_000 + Math.Max(0, version.Build);

    internal static Version CreateOSVersion(int value)
    {
        var major = value / 100_000_000;
        var minor = (value % 100_000_000) / 100_000;
        var build = value % 100_000;

        return new Version(major, minor, build);
    }
}
