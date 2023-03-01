using System.Collections;

// using Elastic.Apm;
// using Elastic.Apm.Api;

namespace Courses.CLI;

internal class CourseDirectoryWalker : IEnumerable<FileSystemInfo>
{
    internal static readonly string[] VideoExtensions = { "mp4", "ts", "mkv", "webm", "m4v" };
    private readonly DirectoryInfo _path;

    public CourseDirectoryWalker(DirectoryInfo path)
    {
        _path = path;
    }

    public IEnumerator<FileSystemInfo> GetEnumerator()
    {
        var hasFilesInDirectory = false;

        var files = _path.EnumerateFiles().ToList();
        files.Sort(new CourseFileSystemEntriesComparer());
        foreach (var file in files)
            if (IsVideoFile(file))
            {
                if (!hasFilesInDirectory) hasFilesInDirectory = true;
                yield return file;
            }

        if (hasFilesInDirectory) yield break;

        var directories = _path.EnumerateDirectories().ToList();
        directories.Sort(new CourseFileSystemEntriesComparer());

        foreach (var fileOrDirectory in directories.SelectMany(directory => new CourseDirectoryWalker(directory)))
            yield return fileOrDirectory;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private static bool IsVideoFile(FileInfo file)
    {
        return VideoExtensions.Any(e => file.Name.EndsWith(e, StringComparison.InvariantCultureIgnoreCase));
    }
}