using Newtonsoft.Json.Linq;
using Spectre.Console;
using static Entropy.Common;

namespace Entropy
{
    internal class UpdateManager
    {
        public static async Task CheckForUpdates(string currentVersion, Settings settings, bool IsOnStartup = false)
        {
            if (IsOnStartup == true && settings.CheckForUpdates == false)
            {
                return;
            }
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);

            AnsiConsole.MarkupLine($"[purple]You are on version [fuchsia]{EntropyVersion}[/] of Entropy[/]");
            AnsiConsole.MarkupLine($"[purple]Checking for updates...[/]");

            try
            {
                var response = await httpClient.GetStringAsync(RepoUrl);
                var releases = JArray.Parse(response);

                var parsedReleases = releases
                    .Select(release => new
                    {
                        Name = release["name"].ToString(),
                        Version = ParseVersionString(release["name"].ToString())
                    })
                    .ToList();

                var currentParsedVersion = ParseVersionString(currentVersion);

                var latestNormalVersion = parsedReleases
                    .Where(release => !release.Version.IsLTS && !release.Version.IsEXP)
                    .OrderByDescending(release => release.Version.BaseVersion)
                    .FirstOrDefault();

                var latestLTSVersion = parsedReleases
                    .Where(release => release.Version.IsLTS)
                    .OrderByDescending(release => release.Version.BaseVersion)
                    .ThenByDescending(release => release.Version.LTSBuild)
                    .FirstOrDefault();

                var newerLTSBuilds = parsedReleases
                    .Where(release => release.Version.IsLTS &&
                                      release.Version.BaseVersion == currentParsedVersion.BaseVersion &&
                                      release.Version.LTSBuild > currentParsedVersion.LTSBuild)
                    .OrderBy(release => release.Version.LTSBuild)
                    .ToList();

                var latestEXPVersion = parsedReleases
                    .Where(release => release.Version.IsEXP)
                    .OrderByDescending(release => release.Version.BaseVersion)
                    .ThenByDescending(release => release.Version.EXPBuild)
                    .FirstOrDefault();

                var newerEXPBuilds = parsedReleases
                    .Where(release => release.Version.IsEXP &&
                                      release.Version.BaseVersion == currentParsedVersion.BaseVersion &&
                                      release.Version.EXPBuild > currentParsedVersion.EXPBuild)
                    .OrderBy(release => release.Version.EXPBuild)
                    .ToList();

                if (!currentParsedVersion.IsLTS && !currentParsedVersion.IsEXP)
                {
                    if (latestNormalVersion != null && CompareVersions(currentParsedVersion, latestNormalVersion.Version))
                    {
                        AnsiConsole.MarkupLine($"[purple]New version available: [fuchsia]{latestNormalVersion.Name}[/][/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"[purple]You are on the latest version: [fuchsia]{Common.EntropyVersion}[/][/]");
                    }

                    if (latestLTSVersion != null)
                    {
                        AnsiConsole.MarkupLine($"[purple]Latest LTS version available: [fuchsia]{latestLTSVersion.Name}[/][/]");
                    }

                    if (latestEXPVersion != null)
                    {
                        AnsiConsole.MarkupLine($"[purple]Latest EXP version available: [fuchsia]{latestEXPVersion.Name}[/][/]");
                    }
                }
                else if (currentParsedVersion.IsLTS)
                {
                    if (latestLTSVersion != null && CompareVersions(currentParsedVersion, latestLTSVersion.Version))
                    {
                        AnsiConsole.MarkupLine($"[purple]New LTS build available: [fuchsia]{latestLTSVersion.Name}[/][/]");
                    }
                    else if (newerLTSBuilds.Any())
                    {
                        AnsiConsole.MarkupLine($"[purple]New LTS builds available: [fuchsia]{string.Join(", ", newerLTSBuilds.Select(r => r.Name))}[/][/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"[purple]You are on the latest LTS version.[/]");
                    }

                    if (latestEXPVersion != null)
                    {
                        AnsiConsole.MarkupLine($"[purple]Latest EXP version available: [fuchsia]{latestEXPVersion.Name}[/][/]");
                    }
                }
                else if (currentParsedVersion.IsEXP)
                {
                    if (latestEXPVersion != null && CompareVersions(currentParsedVersion, latestEXPVersion.Version))
                    {
                        AnsiConsole.MarkupLine($"[purple]New EXP build available: [fuchsia]{latestEXPVersion.Name}[/][/]");
                    }
                    else if (newerEXPBuilds.Any())
                    {
                        AnsiConsole.MarkupLine($"[purple]New EXP builds available: [fuchsia]{string.Join(", ", newerEXPBuilds.Select(r => r.Name))}[/][/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"[purple]You are on the latest EXP version.[/]");
                    }

                    if (latestNormalVersion != null)
                    {
                        AnsiConsole.MarkupLine($"[purple]Latest normal version available: [fuchsia]{latestNormalVersion.Name}[/][/]");
                    }
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Failed to check for updates: [fuchsia]{ex.Message}[/][/]");
            }
        }

        private static ParsedVersion ParseVersionString(string versionString)
        {
            var parsedVersion = new ParsedVersion();
            if (string.IsNullOrEmpty(versionString))
                return parsedVersion;

            var parts = versionString.TrimStart('v').Split('-');
            if (parts.Length > 0)
            {
                parsedVersion.BaseVersion = parts[0];
            }
            if (parts.Length > 1 && parts[1] == "LTS")
            {
                parsedVersion.IsLTS = true;
            }
            else if (parts.Length > 1 && parts[1] == "EXP")
            {
                parsedVersion.IsEXP = true;
            }
            if (parts.Length > 2 && parts[1] == "LTS")
            {
                parsedVersion.LTSBuild = int.Parse(parts[2]);
            }
            else if (parts.Length > 2 && parts[1] == "EXP")
            {
                parsedVersion.EXPBuild = int.Parse(parts[2]);
            }

            return parsedVersion;
        }

        private static bool CompareVersions(ParsedVersion current, ParsedVersion latest)
        {
            var baseVersionComparison = string.Compare(current.BaseVersion, latest.BaseVersion, StringComparison.OrdinalIgnoreCase);

            if (baseVersionComparison < 0)
                return true;

            if (baseVersionComparison > 0)
                return false;
            if (current.IsLTS != latest.IsLTS)
                return latest.IsLTS;
            if (current.IsEXP != latest.IsEXP)
                return latest.IsEXP;

            if (current.IsLTS)
                return current.LTSBuild < latest.LTSBuild;
            if (current.IsEXP)
                return current.EXPBuild < latest.EXPBuild;

            return false;
        }

        public class ParsedVersion
        {
            public string BaseVersion { get; set; }
            public bool IsLTS { get; set; }
            public int LTSBuild { get; set; }
            public bool IsEXP { get; set; }
            public int EXPBuild { get; set; }
        }
    }
}