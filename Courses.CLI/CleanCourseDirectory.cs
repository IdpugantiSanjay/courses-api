using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace Courses.CLI;

public class CleanCourseDirectory
{
    private static readonly Regex Regex = new(@"^\[[^\]]*\]");
    private readonly ILogger<CleanCourseDirectory> _logger;

    public CleanCourseDirectory(ILogger<CleanCourseDirectory> logger)
    {
        _logger = logger;
    }

    public DirectoryInfo Clean(DirectoryInfo coursePath)
    {
        // if course files have index re index them

        // sort files in every section

        // remove square bracket content

        if (coursePath.Parent is null)
            return coursePath;

        if (BrandingExists(coursePath.Name))
        {
            var newPath = $@"{coursePath.Parent.FullName}/{RemoveBranding(coursePath.Name)}";
            coursePath.MoveTo(newPath);
        }


        // remove nested single folders
        var fsEntry = coursePath.EnumerateFileSystemInfos("*", SearchOption.TopDirectoryOnly).ToList();
        var videoFiles = fsEntry
            .OfType<FileInfo>()
            .Where(x =>
                CourseDirectoryWalker.VideoExtensions.Any(e =>
                    x.Extension.EndsWith(e, StringComparison.InvariantCultureIgnoreCase)))
            .ToList();

        var directories = fsEntry.OfType<DirectoryInfo>().ToList();


        if (videoFiles.Count == 0 && directories.Count == 1)
        {
            var folder = directories.First();
            var entries = Directory.GetFileSystemEntries(folder.FullName, "*", SearchOption.TopDirectoryOnly);

            foreach (var entryPath in entries)
                if (Directory.Exists(entryPath))
                {
                    Directory.Move(entryPath, entryPath.Replace($"/{new DirectoryInfo(folder.FullName).Name}", ""));
                    _logger.LogInformation("Moved {OriginalName} from {NewName}", entryPath,
                        entryPath.Replace($"/{folder.FullName}", ""));
                }
                else if (File.Exists(entryPath))
                {
                    File.Move(entryPath, entryPath.Replace($"/{new FileInfo(folder.FullName).Name}", ""));
                    _logger.LogInformation("Moved {OriginalName} from {NewName}", entryPath,
                        entryPath.Replace($"/{folder.FullName}", ""));
                }
        }

        _logger.LogInformation("Course Path after cleaning: {CoursePath}", coursePath.FullName);
        return coursePath;
    }


    private static string RemoveBranding(string name)
    {
        return Regex.Replace(name, "").Replace("git.ir", "");
    }

    private static bool BrandingExists(string name)
    {
        return Regex.IsMatch(name) || name.Contains("git.ir");
    }
}