using ExpressionTExpressionUtil.common.test.clases;
using ExpressionToSQL.common.configuration;
using ExpressionToSQL.util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace ExpresionToSql.util.test
{
    [TestClass]
    public class SqlEntityUtilTests
    {
        private MockRepository mockRepository;



        [TestInitialize]
        public void TestInitialize()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);


        }

        [TestCleanup]
        public void TestCleanup()
        {
            //this.mockRepository.VerifyAll();
        }

        public Persona GetPersona()
        {
            return new Persona()
            {
                Id = 5,
                Fecha = new System.DateTime(1982, 3, 26),
                Nombre = "Miguel Angel",
                Apellido1 = "del Campo",
                apellido2 = "Morales",
                edad = 37
            };
        }

        [TestMethod]
        public void GetFieds()
        {
            // Arrange
            ICommandConfiguration Configuration = this.mockRepository
                .Of<ICommandConfiguration>()
                .Where(x => x.FieldsInclude == true)
                .Where(x => x.PropsInclude == false)
                .Where(x => x.TableName == "Persona")
                .Where(x => x.KeyTableDefault == "Id")
                .Where(x => x.IncludeId == true)
                .First();

           

            Persona Entidad = GetPersona();

            // Act
            var result = SqlEntityUtil.GetKeysValues(
                Configuration,
                Entidad);

            // Assert
            Assert.IsTrue(result.Count()==1 && result["edad"] == "37");
        }

        [TestMethod]
        public void GetProps()
        {
            // Arrange
            ICommandConfiguration Configuration = this.mockRepository
                .Of<ICommandConfiguration>()
                .Where(x => x.FieldsInclude == false)
                .Where(x => x.PropsInclude == true)
                .Where(x => x.TableName == "Persona")
                .Where(x => x.KeyTableDefault == "Id")
                .Where(x => x.IncludeId == true)
                .First();



            Persona Entidad = GetPersona();

            // Act
            var result = SqlEntityUtil.GetKeysValues(
                Configuration,
                Entidad);

            // Assert
            Assert.IsTrue(
                result.Count() == 4 
                && result["Fecha"]=="'19820326 00:00'"
                && result["Nombre"] == "N'Miguel Angel'"
                && result["Id"] == "5"
                && result["Saldo"] == "null"    
                );
        }

        [TestMethod]
        public void GetProps_NoId()
        {
            // Arrange
            ICommandConfiguration Configuration = this.mockRepository
                .Of<ICommandConfiguration>()
                .Where(x => x.FieldsInclude == true)
                .Where(x => x.PropsInclude == true)
                .Where(x => x.TableName == "Persona")
                .Where(x => x.KeyTableDefault == "Id")
                .Where(x => x.IncludeId == true)
                .First();



            Persona Entidad = GetPersona();

            // Act
            var result = SqlEntityUtil.GetKeysValues(
                Configuration,
                Entidad);

            // Assert
            Assert.IsTrue(
                result.Count() == 5
                && result["Fecha"] == "'19820326 00:00'"
                && result["Nombre"] == "N'Miguel Angel'"
                && result["Id"] == "5"
                && result["Saldo"] == "null"
                && result["edad"] == "37"
                );
        }


        [TestMethod]
        public void GetProps_and_Fields()
        {
            // Arrange
            ICommandConfiguration Configuration = this.mockRepository
                .Of<ICommandConfiguration>()
                .Where(x => x.FieldsInclude == true)
                .Where(x => x.PropsInclude == true)
                .Where(x => x.TableName == "Persona")
                .Where(x => x.KeyTableDefault == "Id")
                .Where(x => x.IncludeId == false)
                .First();



            Persona Entidad = GetPersona();

            // Act
            var result = SqlEntityUtil.GetKeysValues(
                Configuration,
                Entidad);

            // Assert
            Assert.IsTrue(
                result.Count() == 4
                && result["Fecha"] == "'19820326 00:00'"
                && result["Nombre"] == "N'Miguel Angel'"
                && result["Saldo"] == "null"
                && result["edad"] == "37"
                );
        }
    }
}
