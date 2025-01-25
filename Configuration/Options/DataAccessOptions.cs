namespace Configuration.Options
{
    public class DataAccessOptions : IOptions
    {
        public static string SectionName = "dataAccess";

        public string ConnectionString { get; set; }
    }
}
