namespace AdoRepository.Test.Defs
{
    public class Configs
    {
        public static string ConnectionString
        {
            get
            {
                return System
                    .Configuration
                    .ConfigurationManager
                    .ConnectionStrings["SchoolConnectionString"]
                    .ConnectionString;
            }
        }
    }
}
