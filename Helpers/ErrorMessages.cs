namespace ColumnExplorer.Helpers
{
    /// <summary>
    /// Enum that defines error messages.
    /// </summary>
    public enum ErrorMessages
    {
        /// <summary>
        /// Error message indicating access is denied.
        /// </summary>
        Unauthorized,

        /// <summary>
        /// Error message indicating file not found.
        /// </summary>
        FileNotFound,

        /// <summary>
        /// Error message indicating directory not found.
        /// </summary>
        DirectoryNotFound,

        /// <summary>
        /// Error message indicating path is too long.
        /// </summary>
        PathTooLong,

        /// <summary>
        /// Error message indicating a general I/O error.
        /// </summary>
        IOError,

        /// <summary>
        /// Error message indicating an invalid path.
        /// </summary>
        InvalidPath,

        /// <summary>
        /// Error message indicating the disk is full.
        /// </summary>
        DiskFull,

        /// <summary>
        /// Error message indicating the file is being used by another process.
        /// </summary>
        SharingViolation
    }

    /// <summary>
    /// Static class that provides extension methods for the ErrorMessages enum.
    /// </summary>
    public static class ErrorMessagesExtensions
    {
        /// <summary>
        /// Gets the error message corresponding to the value of the ErrorMessages enum.
        /// </summary>
        /// <param name="errorMessage">The value of the ErrorMessages enum.</param>
        /// <returns>The error message.</returns>
        public static string GetMessage(this ErrorMessages errorMessage)
        {
            return errorMessage switch
            {
                ErrorMessages.Unauthorized => "Unauthorized",
                ErrorMessages.FileNotFound => "File not found",
                ErrorMessages.DirectoryNotFound => "Directory not found",
                ErrorMessages.PathTooLong => "Path too long",
                ErrorMessages.IOError => "I/O error occurred",
                ErrorMessages.InvalidPath => "Invalid path",
                ErrorMessages.DiskFull => "Disk is full",
                ErrorMessages.SharingViolation => "File is being used by another process",
                _ => "Unknown error"
            };
        }
    }
}
