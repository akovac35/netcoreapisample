using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class SampleConfig
    {
        [Required]
        public required string DbConnectionString { get; init; }
        public bool? UseInMemoryDb { get; init; }

        // TODO LOW add custom validation attribute to validate object
        [Required]
        public required JwtConfig JwtConfig { get; init; }

        [Required]
        public required DbType DbType { get; init; }
    }

    public class JwtConfig
    {
        public required int TokenValidityInMinutes { get; init; }
        public required string JwtSecretKey { get; init; }
    }

    public enum DbType
    {
        None = 0,
        SqlServer = 1,
        Oracle = 2
    }
}
