using Newtonsoft.Json.Linq;
using Spectre.Console;

namespace Entropy
{
    internal class UpdateManager
    {
        private const string RepoUrl = "https://api.github.com/repos/MaxPopsuy/Entropy/releases";
        private const string UserAgent = "MaxPopsuy";

        public static async Task CheckForUpdates(string currentVersion)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);

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
                    .Where(release => !release.Version.IsLTS)
                    .OrderByDescending(release => release.Version.BaseVersion)
                    .ThenByDescending(release => release.Version.LTSBuild)
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

                if (!currentParsedVersion.IsLTS)
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
                }
                else
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
            if (parts.Length > 2)
            {
                parsedVersion.LTSBuild = int.Parse(parts[2]);
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

            return current.LTSBuild < latest.LTSBuild;
        }

        public class ParsedVersion
        {
            public string BaseVersion { get; set; }
            public bool IsLTS { get; set; }
            public int LTSBuild { get; set; }
        }
    }
}
