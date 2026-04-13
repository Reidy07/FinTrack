namespace FinTrack.Core.Constants
{
    public static class InsightMessages
    {
        public const string Default = "Sigue registrando tus movimientos diarios para que la IA pueda detectar tus patrones de consumo.";

        public const string NewUser = "🤖 ¡Hola! Comienza a registrar tus gastos e ingresos. Con más datos, mi motor de Machine Learning podrá predecir tu futuro financiero.";

        public const string CriticalDebt = "🚨 Tu balance histórico está en números rojos. Evita los 'gastos hormiga' temporales y considera aplicar el método Avalancha (pagar primero las deudas con mayor tasa de interés).";

        // Usamos {0} y {1} para inyectar el nombre de la categoría y el porcentaje dinámicamente
        public const string SpendingAnomaly = "📊 ¡Ojo con '{0}'! Estás destinando el {1}% de tus gastos del mes solo a eso. Establece un presupuesto estricto para esta categoría.";

        // Usamos {0} para inyectar el monto del fondo de emergencia
        public const string EmergencyFundGoal = "💡 Regla de Oro: Tu fondo de emergencia ideal debería ser de al menos {0} (3 meses de gastos promedio). ¡Prioriza ahorrar esa cantidad antes de realizar gastos fuertes!";

        public const string HighLiquidity = "🚀 ¡Excelente disciplina! Has gastado menos de la mitad de tus ingresos del mes. Es el momento ideal para destinar el excedente a fondos indexados o inversiones a plazo fijo.";

        public const string LowMargin = "⚖️ Tu margen de ahorro es muy estrecho. Intenta aplicar la regla 50/30/20: 50% en necesidades básicas, 30% en estilo de vida y garantizar un 20% intocable para el ahorro.";
    }
}