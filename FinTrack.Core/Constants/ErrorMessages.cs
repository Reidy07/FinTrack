
namespace FinTrack.Core.Constants
{
    public static class ErrorMessages
    {
        // Entidades (Validaciones)
        public const string RequiredName = "El nombre es obligatorio.";
        public const string RequiredDescription = "La descripción es obligatoria.";
        public const string AmountGreaterThanZero = "El monto debe ser mayor a cero.";
        public const string MaxAmountExceeded = "El monto no puede exceder $1,000,000,000";
        public const string MaxLength100 = "El nombre no puede exceder 100 caracteres.";
        public const string MaxLength200 = "La descripción no puede exceder 200 caracteres.";
        public const string InvalidColorFormat = "El color debe estar en formato hexadecimal (#RRGGBB).";
        public const string FutureDateNotAllowed = "La fecha no puede ser futura.";
        public const string InvalidEndDate = "La fecha de fin debe ser posterior a la de inicio.";

        // API y Controladores
        public const string RequiredField = "Faltan datos obligatorios. Por favor, revisa el formulario.";
        public const string ApiSaveError = "Ocurrió un error al comunicarse con el servidor.";
        public const string NotFound = "El registro que buscas no existe o no tienes permisos.";
        public const string UserIdRequired = "El ID de usuario es requerido.";
        public const string IdMismatch = "El ID de la petición no coincide con el del objeto.";
        public const string IncompleteData = "Datos incompletos.";

        // Errores de Login y Registro
        public const string InvalidLogin = "Correo electrónico o contraseña incorrectos.";
        public const string InvalidEmailFormat = "El formato del correo no es válido.";
        public const string PasswordMismatch = "Las contraseñas no coinciden.";

    }
}