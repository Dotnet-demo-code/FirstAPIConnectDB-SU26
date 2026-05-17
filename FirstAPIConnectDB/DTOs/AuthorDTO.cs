namespace FirstAPIConnectDB.DTOs
{
    public class AuthorDTO
    {
        public int AuthorId { get; set; }

        public string Name { get; set; } = null!;

        public int? BirthYear { get; set; }
    }
}
