namespace ColumnExplorer.Helpers
{
    /// <summary>
    /// �G���[���b�Z�[�W���`����񋓌^�B
    /// </summary>
    public enum ErrorMessages
    {
        /// <summary>
        /// �A�N�Z�X�����ۂ��ꂽ���Ƃ������G���[���b�Z�[�W�B
        /// </summary>
        Unauthorized,

        /// <summary>
        /// �t�@�C����������Ȃ����Ƃ������G���[���b�Z�[�W�B
        /// </summary>
        FileNotFound,

        /// <summary>
        /// �f�B���N�g����������Ȃ����Ƃ������G���[���b�Z�[�W�B
        /// </summary>
        DirectoryNotFound,

        /// <summary>
        /// �p�X���������邱�Ƃ������G���[���b�Z�[�W�B
        /// </summary>
        PathTooLong,

        /// <summary>
        /// ��ʓI�ȓ��o�̓G���[�������G���[���b�Z�[�W�B
        /// </summary>
        IOError,

        /// <summary>
        /// �����ȃp�X�������G���[���b�Z�[�W�B
        /// </summary>
        InvalidPath,

        /// <summary>
        /// �f�B�X�N�������ς��ł��邱�Ƃ������G���[���b�Z�[�W�B
        /// </summary>
        DiskFull,

        /// <summary>
        /// �t�@�C�������̃v���Z�X�ɂ���Ďg�p����Ă��邱�Ƃ������G���[���b�Z�[�W�B
        /// </summary>
        SharingViolation
    }

    /// <summary>
    /// ErrorMessages �񋓌^�̊g�����\�b�h��񋟂���ÓI�N���X�B
    /// </summary>
    public static class ErrorMessagesExtensions
    {
        /// <summary>
        /// ErrorMessages �񋓌^�̒l�ɑΉ�����G���[���b�Z�[�W���擾���܂��B
        /// </summary>
        /// <param name="errorMessage">ErrorMessages �񋓌^�̒l�B</param>
        /// <returns>�G���[���b�Z�[�W�B</returns>
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
