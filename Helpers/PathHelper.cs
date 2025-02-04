using System.IO;

namespace ColumnExplorer.Helpers
{
    public static class PathHelper
    {
        private const string DRIVE = "Drive";

        /// <summary>
        /// Gets the label based on the specified path.
        /// </summary>
        /// <param name="path">The path to get the label for.</param>
        /// <returns>The label based on the path.</returns>
        public static string GetLabel(string path)
        {
            return (path == Path.GetPathRoot(path) || path == DRIVE)
                ? path
                : Path.GetFileName(path);
        }
    }
}
