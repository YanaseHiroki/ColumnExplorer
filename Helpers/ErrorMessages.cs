namespace ColumnExplorer.Helpers
{
    /// <summary>
    /// エラーメッセージを定義する列挙型。
    /// </summary>
    public enum ErrorMessages
    {
        /// <summary>
        /// アクセスが拒否されたことを示すエラーメッセージ。
        /// </summary>
        Unauthorized,

        /// <summary>
        /// ファイルが見つからないことを示すエラーメッセージ。
        /// </summary>
        FileNotFound,

        /// <summary>
        /// ディレクトリが見つからないことを示すエラーメッセージ。
        /// </summary>
        DirectoryNotFound,

        /// <summary>
        /// パスが長すぎることを示すエラーメッセージ。
        /// </summary>
        PathTooLong,

        /// <summary>
        /// 一般的な入出力エラーを示すエラーメッセージ。
        /// </summary>
        IOError,

        /// <summary>
        /// 無効なパスを示すエラーメッセージ。
        /// </summary>
        InvalidPath,

        /// <summary>
        /// ディスクがいっぱいであることを示すエラーメッセージ。
        /// </summary>
        DiskFull,

        /// <summary>
        /// ファイルが他のプロセスによって使用されていることを示すエラーメッセージ。
        /// </summary>
        SharingViolation
    }

    /// <summary>
    /// ErrorMessages 列挙型の拡張メソッドを提供する静的クラス。
    /// </summary>
    public static class ErrorMessagesExtensions
    {
        /// <summary>
        /// ErrorMessages 列挙型の値に対応するエラーメッセージを取得します。
        /// </summary>
        /// <param name="errorMessage">ErrorMessages 列挙型の値。</param>
        /// <returns>エラーメッセージ。</returns>
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
