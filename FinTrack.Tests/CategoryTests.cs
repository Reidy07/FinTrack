using FinTrack.Core.Entities;
using Xunit;

namespace FinTrack.Tests
{
    public class CategoryTests
    {
        [Fact]
        public void Categoría_debe_asignar_nombre()
        {
            var category = new Category
            {
                Name = "Transporte"
            };

            Assert.Equal("Transporte", category.Name);
        }
    }
}
