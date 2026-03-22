using FinTrack.Core.Interfaces.Services;
using System.Text.Json;
using System.Text;

namespace FinTrack.Core.Services
{
    public class AIChatbotService : IAIChatbotService
    {
        private readonly IFinancialService _financialService;

        private readonly string _geminiApiKey = "AIzaSyCsGssNLtyMmKmmzwIgnZdnZ5NaTFndEno";

        public AIChatbotService(IFinancialService financialService)
        {
            _financialService = financialService;
        }

        public async Task<string> AskFinancialAdvisorAsync(string userId, string userMessage)
        {
            try
            {
                // 1. Obtenemos el contexto financiero del usuario
                var summary = await _financialService.GetDashboardSummaryAsync(userId, DateTime.Now);

                string systemContext = $@"
                    Eres 'FinBot', el asistente financiero IA experto de la aplicación 'FinTrack'.
                    Datos actuales del usuario:
                    - Balance actual: {summary.Balance:C}
                    - Ingresos este mes: {summary.TotalIncome:C}
                    - Gastos este mes: {summary.TotalExpenses:C}
                    
                    Reglas:
                    1. Responde SIEMPRE en español, de forma amigable, empática y profesional.
                    2. Sé conciso, usa párrafos cortos (máximo 2 o 3).
                    3. Basa tus consejos en los números reales del usuario que te acabo de dar.
                    
                    El usuario te dice/pregunta: {userMessage}";

                // 2. Formato estricto para la API de Gemini
                using var client = new HttpClient();
                var requestBody = new
                {
                    contents = new[]
                    {
                        new {
                            parts = new[] {
                                new { text = systemContext }
                            }
                        }
                    }
                };

                // Asegurarnos de que el JSON se serialice correctamente en minúsculas (CamelCase)
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var jsonContent = JsonSerializer.Serialize(requestBody, options);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // 3. Petición a Google
                var response = await client.PostAsync($"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_geminiApiKey}", content);

                // 4. Leer la respuesta (sea buena o mala)
                var responseData = await response.Content.ReadAsStringAsync();

                // SI FALLA, AHORA VEREMOS EL POR QUÉ:
                if (!response.IsSuccessStatusCode)
                {
                    return $"⚠️ **Error de Google ({(int)response.StatusCode})**: {responseData}";
                }

                using var jsonDoc = JsonDocument.Parse(responseData);

                return jsonDoc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text").GetString() ?? "No pude generar una respuesta.";
            }
            catch (Exception ex)
            {
                return $"ERROR INTERNO DE LA APLICACIÓN: {ex.Message}";
            }
        }
    }
}