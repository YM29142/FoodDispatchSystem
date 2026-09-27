using Resend;

namespace FoodDispatchSystem.Web.Services
{
    public class EmailService
    {
        private readonly IResend _resend;

        public EmailService(IResend resend)
        {
            _resend = resend;
        }

       

        public async Task SendPasswordResetEmailAsync(
    string recipientEmail,
    string resetUrl)
        {
            var safeResetUrl = System.Net.WebUtility.HtmlEncode(resetUrl);

            var message = new EmailMessage
            {
                From = "FoodDispatch <onboarding@resend.dev>",
                Subject = "Recuperación de contraseña - FoodDispatch",
                HtmlBody = $"""
            <div style="font-family: Arial, sans-serif; max-width: 600px; margin: auto;">
                <h2>FoodDispatch</h2>

                <p>Recibimos una solicitud para restablecer tu contraseña.</p>

                <p>
                    <a href="{safeResetUrl}"
                       style="
                           display: inline-block;
                           padding: 12px 18px;
                           background-color: #0d6efd;
                           color: white;
                           text-decoration: none;
                           border-radius: 6px;">
                        Restablecer contraseña
                    </a>
                </p>

                <p>
                    Si no solicitaste este cambio, puedes ignorar este correo.
                </p>

                <p style="color: #666; font-size: 13px;">
                    Por seguridad, no compartas este enlace con otras personas.
                </p>
            </div>
            """
            };

            message.To.Add(recipientEmail);

            await _resend.EmailSendAsync(message);
        }
    }
}