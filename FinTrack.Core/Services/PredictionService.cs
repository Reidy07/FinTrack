using FinTrack.Core.DTOs;
using FinTrack.Core.Interfaces;
using FinTrack.Core.Interfaces.Services;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Data;

namespace FinTrack.Infrastructure.Services
{
    public class PredictionService : IPredictionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly MLContext _mlContext;

        public PredictionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mlContext = new MLContext();
        }

        // Clase interna para los datos de entrada del modelo
        public class ModelInput
        {
            public float Month { get; set; }
            public float Amount { get; set; }
        }

        // Clase interna para la predicción
        public class ModelOutput
        {
            [ColumnName("Score")]
            public float PredictedAmount { get; set; }
        }

        public async Task<decimal> PredictNextMonthExpensesAsync(string userId)
        {
            var expenses = await _unitOfWork.Expenses.FindAsync(e => e.UserId == userId);

            // Si hay pocos datos, devolvemos 0 para no romper el programa
            if (expenses.Count() < 2) return 0;

            // Preparamos los datos
            var data = expenses
                .GroupBy(e => e.Date.Month)
                .Select(g => new ModelInput
                {
                    Month = g.Key,
                    Amount = (float)g.Sum(e => e.Amount)
                });

            IDataView trainingData = _mlContext.Data.LoadFromEnumerable(data);

            // Entrenamos un modelo de regresión simple
            var pipeline = _mlContext.Transforms.Concatenate("Features", "Month")
                .Append(_mlContext.Regression.Trainers.Sdca(labelColumnName: "Amount", maximumNumberOfIterations: 100));

            var model = pipeline.Fit(trainingData);

            // Predecimos el mes siguiente
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(model);
            var nextMonth = DateTime.Now.AddMonths(1).Month;

            var prediction = predictionEngine.Predict(new ModelInput { Month = nextMonth });

            return (decimal)Math.Max(0, prediction.PredictedAmount);
        }

        // Métodos vacíos por ahora para cumplir la interfaz
        public Task<Dictionary<string, decimal>> PredictCategoryTrendsAsync(string userId) => Task.FromResult(new Dictionary<string, decimal>());
        public Task<List<AlertDto>> GenerateAlertsAsync(string userId) => Task.FromResult(new List<AlertDto>());
    }
}