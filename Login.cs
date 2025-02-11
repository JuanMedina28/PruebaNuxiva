using System.ComponentModel.DataAnnotations;

namespace EvaluacionNuxiva
{
    public class Login
    {
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        public int Extension { get; set; }
        public int TipoMov { get; set; }
        [Required]
        public DateTime Fecha { get; set; }
        public User User { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Nombres { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public Area Area { get; set; }
    }

    public class Area
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

}
