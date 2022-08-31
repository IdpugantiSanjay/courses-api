using JetBrains.Annotations;

namespace Courses.CLI;

internal class CourseFileSystemEntriesComparer : IComparer<FileSystemInfo>
{
    public int Compare(FileSystemInfo? x, FileSystemInfo? y)
    {
        if (x is FileInfo xFileInfo && y is FileInfo yFileInfo)
            return SequenceNumber(xFileInfo.Name) - SequenceNumber(yFileInfo.Name);

        if (x is DirectoryInfo xDirectoryInfo && y is DirectoryInfo yDirectoryInfo)
            return SequenceNumber(xDirectoryInfo.Name) - SequenceNumber(yDirectoryInfo.Name);

        throw new NotImplementedException();
    }

    [Pure]
    private static int SequenceNumber(string fileOrDirectoryName)
    {
        if (int.TryParse(string.Concat(fileOrDirectoryName.TakeWhile(char.IsNumber)), out var number)) return number;
        return 0;
    }
}