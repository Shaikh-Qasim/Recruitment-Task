namespace RecruitmentTasks.Common;

public static class Constants
{
    public static class Database
    {
        public const string StoredProcedureName = "GetCategoryTree";
        public const int CommandTimeoutSeconds = 30;
        public const int ProcedureNameMaxLength = 256;
        public const int CacheClearingTimeoutSeconds = 60;
        public const string CacheClearingCommand = "DBCC FREEPROCCACHE; DBCC DROPCLEANBUFFERS;";
    }

    public static class Benchmark
    {
        public const int Iterations = 5;
    }

    public static class Display
    {
        public const int SeparatorWidth = 80;
        public const char SeparatorChar = '=';
        public const char SubSeparatorChar = '-';
        public const int IndentSpacesPerLevel = 2;
    }

    public static class Encoding
    {
        public const int Base64UrlGuidLength = 22;
        public const int Base64PaddedLength = 24;
        public const int GuidByteLength = 16;
    }
}
