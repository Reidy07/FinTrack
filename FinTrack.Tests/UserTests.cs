using Xunit;
using FinTrack.Core.Entities;
using System;
using System.Collections.Generic;

namespace FinTrack.Tests
{
    public class UserTests
    {
        [Fact]
        public void Debe_Crear_Usuario_Con_Datos_Validos()
        {
            var user = new User
            {
                Id = "1",
                Email = "reidybrown@gmail.com",
                UserName = "reidy07",
                FirstName = "Reidy",
                LastName = "Diaz"
            };

            Assert.Equal("1", user.Id);
            Assert.Equal("reidybrown@gmail.com", user.Email);
            Assert.Equal("reidy07", user.UserName);
            Assert.Equal("Reidy", user.FirstName);
            Assert.Equal("Diaz", user.LastName);
        }

        [Fact]
        public void Debe_Asignar_Fecha_De_Creacion_Automaticamente()
        {
            var user = new User();

            Assert.True(user.CreatedAt <= DateTime.UtcNow);
        }

        [Fact]
        public void Debe_Permitir_Asignar_Colecciones()
        {
            var user = new User
            {
                Incomes = new List<Income>(),
                Expenses = new List<Expense>()
            };

            Assert.NotNull(user.Incomes);
            Assert.NotNull(user.Expenses);
        }
    }
}
