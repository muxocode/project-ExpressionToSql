using ExpresionToSql.util.test.clases;
using ExpressionToSQL.util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ExpressionTExpressionUtil.common.test
{
    [TestClass]
    public class ExpressionUtilTests
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
            this.mockRepository.VerifyAll();
        }

        [TestMethod]
        public void Igual()
        {
            
            var sResult = ExpressionUtil.ToSql<Paciente>(x => x.Id == 3);

            Assert.IsTrue(sResult == "Id = 3");
        }

        [TestMethod]
        public void Mayor()
        {
            var sResult = ExpressionUtil.ToSql<Paciente>(x => x.Id > 3);

            Assert.IsTrue(sResult == "Id > 3");
        }

        [TestMethod]
        public void Menor()
        {

            var sResult = ExpressionUtil.ToSql<Paciente>(x => x.Id < 3);

            Assert.IsTrue(sResult == "Id < 3");
        }

        [TestMethod]
        public void MenorIgual()
        {
            var sResult = ExpressionUtil.ToSql<Paciente>(x => x.Id <= 3);

            Assert.IsTrue(sResult == "Id <= 3");
        }

        [TestMethod]
        public void MayorIgual()
        {
            var sResult = ExpressionUtil.ToSql<Paciente>(x => x.Id >= 3);

            Assert.IsTrue(sResult == "Id >= 3");
        }

        [TestMethod]
        public void Distinto()
        {
            var sResult = ExpressionUtil.ToSql<Paciente>(x => x.Id != 3);

            Assert.IsTrue(sResult == "Id <> 3");
        }

        [TestMethod]
        public void And()
        {
            var sResult = ExpressionUtil.ToSql<Paciente>(x => x.Id != 3 && x.Id == 3);

            Assert.IsTrue(sResult == "(Id <> 3) AND (Id = 3)");
        }

        [TestMethod]
        public void OR()
        {
            var sResult = ExpressionUtil.ToSql<Paciente>(x => x.Id != 3 || x.Id == 3);

            Assert.IsTrue(sResult == "(Id <> 3) OR (Id = 3)");
        }

        [TestMethod]
        public void OR_AND()
        {
            var sResult = ExpressionUtil.ToSql<Paciente>(x => (x.Id != 3) || (x.Id == 3 && x.Id < 4));

            Assert.IsTrue(sResult == "(Id <> 3) OR ((Id = 3) AND (Id < 4))");
        }

        [TestMethod]
        public void ANDOR()
        {
            var sResult = ExpressionUtil.ToSql<Paciente>(x => (x.Id != 3) || (x.Id == 3 && x.Id < 4));

            Assert.IsTrue(sResult == "(Id <> 3) OR ((Id = 3) AND (Id < 4))");
        }

        [TestMethod]
        public void String()
        {
            var sResult = ExpressionUtil.ToSql<Paciente>(x => x.Nombre == "Ejemplo");

            Assert.IsTrue(sResult == "Nombre = N'Ejemplo'");
        }

        [TestMethod]
        public void StarsWith()
        {
            var sResult = ExpressionUtil.ToSql<Paciente>(x => x.Nombre.StartsWith("e"));

            Assert.IsTrue(sResult == "Nombre LIKE N'e%'");
        }


        [TestMethod]
        public void NotEndsWith()
        {
            var sResult = ExpressionUtil.ToSql<Paciente>(x => !x.Nombre.StartsWith("e"));

            Assert.IsTrue(sResult == "Nombre NOT LIKE N'e%'");
        }

        [TestMethod]
        public void NotStarsWith()
        {
            var sResult = ExpressionUtil.ToSql<Paciente>(x => !x.Nombre.StartsWith("e"));

            Assert.IsTrue(sResult == "Nombre NOT LIKE N'e%'");
        }


        [TestMethod]
        public void EndsWith()
        {
            var sResult = ExpressionUtil.ToSql<Paciente>(x => x.Nombre.StartsWith("e"));

            Assert.IsTrue(sResult == "Nombre LIKE N'e%'");
        }

        [TestMethod]
        public void StarsWith_AND()
        {
            var sResult = ExpressionUtil.ToSql<Paciente>(x => x.Id == 3 && x.Nombre.StartsWith("e"));

            Assert.IsTrue(sResult == "(Id = 3) AND (Nombre LIKE N'e%')");
        }


        [TestMethod]
        public void EndsWith_OR()
        {
            var sResult = ExpressionUtil.ToSql<Paciente>(x => x.Nombre.StartsWith("e") || x.Id == 3);

            Assert.IsTrue(sResult == "(Nombre LIKE N'e%') OR (Id = 3)");
        }

        [TestMethod]
        public void Bool()
        {
            var sResult = ExpressionUtil.ToSql<Paciente>(x => x.MayorEdad);

            Assert.IsTrue(sResult == "MayorEdad = 1");

            sResult = ExpressionUtil.ToSql<Paciente>(x => x.MayorEdad == true);

            Assert.IsTrue(sResult == "MayorEdad = 1");

            sResult = ExpressionUtil.ToSql<Paciente>(x => x.MayorEdad != true);

            Assert.IsTrue(sResult == "MayorEdad <> 1");
        }

        [TestMethod]
        public void BoolNegado()
        {
            var sResult = ExpressionUtil.ToSql<Paciente>(x => !x.MayorEdad);

            Assert.IsTrue(sResult == "NOT (MayorEdad = 1)");

            sResult = ExpressionUtil.ToSql<Paciente>(x => x.MayorEdad == false);

            Assert.IsTrue(sResult == "MayorEdad = 0");

            sResult = ExpressionUtil.ToSql<Paciente>(x => x.MayorEdad != false);

            Assert.IsTrue(sResult == "MayorEdad <> 0");
        }

        [TestMethod]
        public void Compuesto()
        {
            var sResult = ExpressionUtil.ToSql<Paciente>(x => x.MayorEdad && x.Id == 3 || x.Nombre.EndsWith("a"));

            Assert.IsTrue(sResult == "((MayorEdad = 1) AND (Id = 3)) OR (Nombre LIKE N'%a')");
        }

        [TestMethod]
        public void CompuestoNegado()
        {
            var sResult = ExpressionUtil.ToSql<Paciente>(x => !(x.MayorEdad && x.Id == 3 || x.Nombre.EndsWith("a")));

            Assert.IsTrue(sResult == "NOT (((MayorEdad = 1) AND (Id = 3)) OR (Nombre LIKE N'%a'))");
        }

        [TestMethod]
        public void CompuestoNegadoParcial()
        {
            var sResult = ExpressionUtil.ToSql<Paciente>(x => !(x.MayorEdad && x.Id == 3) || x.Nombre.EndsWith("a") && !(x.Id > 9));

            Assert.IsTrue(sResult == "(NOT ((MayorEdad = 1) AND (Id = 3))) OR ((Nombre LIKE N'%a') AND (NOT (Id > 9)))");
        }

        [TestMethod]
        public void Prueba()
        {

            System.Linq.Expressions.Expression<Func<Paciente, bool>> oExpr = (x) => (x.Id != 3) || (x.Id == 3 && x.Id < 4);
            string sTesto = oExpr.ToString();

            //var sR = Conversion(sTesto);

            var sResult = "((MayorEdad = 1) AND (Id = 3) OR (Nombre LIKE N'%a'))";

            string sRegex = "(\b[a-z])";
            int i = 0;
            string sQuery = sResult.Replace(sRegex, "({" + (i++) + "})");

            Assert.IsTrue(sResult == "((MayorEdad = 1) AND (Id = 3) OR (Nombre LIKE N'%a'))");
        }

        [TestMethod]
        public void ContainsString()
        {
            var sResult = ExpressionUtil.ToSql<Paciente>(x => x.Nombre.Contains("a"));

            Assert.IsTrue(sResult == "Nombre LIKE N'%a%'");
        }

        [TestMethod]
        public void ContainsId()
        {
            var aIds = new List<long>() { 1, 2, 3 };

            var sResult = ExpressionUtil.ToSql<Paciente>(x => aIds.Contains(x.Id));

            Assert.IsTrue(sResult == "Id IN (1,2,3)");
        }

        [TestMethod]
        public void ContainsId_ENUMERABLE()
        {
            var _aIds = new List<long>() { 1, 2, 3 };

            var aIds = _aIds.Select(x => x);

            var sResult = ExpressionUtil.ToSql<Paciente>(x => aIds.Contains(x.Id));

            Assert.IsTrue(sResult == "Id IN (1,2,3)");
        }

        [TestMethod]
        public void ContainsId_ENUMERABLE_From_Entities()
        {
            var aUsuarios = new Paciente[3];

            aUsuarios[0] = new Paciente() { Id = 1 };
            aUsuarios[1] = new Paciente() { Id = 2 };
            aUsuarios[2] = new Paciente() { Id = 3 };


            var aIds = aUsuarios.Select(x => x.Id);

            var sResult = ExpressionUtil.ToSql<Paciente>(x => aIds.Contains(x.Id));

            Assert.IsTrue(sResult == "Id IN (1,2,3)");

            sResult = ExpressionUtil.ToSql<Paciente>(x => aUsuarios.Select(y => y.Id).Contains(x.Id));

            Assert.IsTrue(sResult == "Id IN (1,2,3)");
        }

        [TestMethod]
        public void ContainsId_ENUMERABLE_From_Empty()
        {
            var aUsuarios = new Paciente[0];

            var aIds = aUsuarios.Select(x => x.Id);

            var sResult = ExpressionUtil.ToSql<Paciente>(x => aIds.Contains(x.Id));

            Assert.IsTrue(sResult == "1 = 0");

        }

        [TestMethod]
        public void ContainsId_ARRAY()
        {
            var _aIds = new List<long>() { 1, 2, 3 };

            var aIds = _aIds.Select(x => x).ToArray();

            var sResult = ExpressionUtil.ToSql<Paciente>(x => aIds.Contains(x.Id));

            Assert.IsTrue(sResult == "Id IN (1,2,3)");
        }

        [TestMethod]
        public void NotContainsId()
        {
            var aIds = new List<long>() { 1, 2, 3 };

            var sResult = ExpressionUtil.ToSql<Paciente>(x => !aIds.Contains(x.Id));

            Assert.IsTrue(sResult == "Id NOT IN (1,2,3)");
        }

        [TestMethod]
        public void ContainsListString()
        {
            var aIds = new List<string>() { "a", "b", "c", "d" };

            var sResult = ExpressionUtil.ToSql<Paciente>(x => aIds.Contains(x.Nombre));

            Assert.IsTrue(sResult == "Nombre IN (N'a',N'b',N'c',N'd')");
        }

        [TestMethod]
        public void NotContainsListString()
        {
            var aIds = new List<string>() { "a", "b", "c", "d" };

            var sResult = ExpressionUtil.ToSql<Paciente>(x => !aIds.Contains(x.Nombre));

            Assert.IsTrue(sResult == "Nombre NOT IN (N'a',N'b',N'c',N'd')");
        }

        [TestMethod]
        public void ContainsListString_OR()
        {
            var aIdStrings = new List<string>() { "a", "b", "c", "d" };
            var aIds = new List<long>() { 1, 2, 3 };

            var sResult = ExpressionUtil.ToSql<Paciente>(x => aIds.Contains(x.Id) || aIdStrings.Contains(x.Nombre));


            Assert.IsTrue(sResult == "(Id IN (1,2,3)) OR (Nombre IN (N'a',N'b',N'c',N'd'))");
        }

        [TestMethod]
        public void NULL()
        {
            var sResult = ExpressionUtil.ToSql<Paciente>(null);

            Assert.IsTrue(sResult == null);
        }

        [TestMethod]
        public void ISNULL()
        {
            var sResult = ExpressionUtil.ToSql<Paciente>(x => x.Nombre == null);

            Assert.IsTrue(sResult == "Nombre IS null");
        }

        [TestMethod]
        public void ISNOTNULL()
        {
            var sResult = ExpressionUtil.ToSql<Paciente>(x => x.Nombre != null);

            Assert.IsTrue(sResult == "Nombre IS NOT null");
        }

        [TestMethod]
        public void Nullable()
        {
            var sResult = ExpressionUtil.ToSql<Persona>(x => x.Fecha == DateTime.Now);

            Assert.IsTrue(sResult == $"Fecha = '{DateTime.Now.ToString("yyyyMMdd HH:mm")}'");

            sResult = ExpressionUtil.ToSql<Persona>(x => x.Saldo == 30);

            Assert.IsTrue(sResult == "Saldo = 30");
        }

        [TestMethod]
        public void ExpressionNames()
        {
            var sResult = ExpressionUtil.ToSqlSeleccion((Paciente x) => x.Id);
            Assert.IsTrue(sResult == "Id");

            sResult = ExpressionUtil.ToSqlSeleccion((Paciente x) => x.MayorEdad);
            Assert.IsTrue(sResult == "MayorEdad");

            sResult = ExpressionUtil.ToSqlSeleccion((Paciente x) => x.Nombre);
            Assert.IsTrue(sResult == "Nombre");

            sResult = ExpressionUtil.ToSqlSeleccion((Persona x) => x.Apellido1);
            Assert.IsTrue(sResult == "Apellido1");

            sResult = ExpressionUtil.ToSqlSeleccion((Persona x) => x.apellido2);
            Assert.IsTrue(sResult == "apellido2");

            sResult = ExpressionUtil.ToSqlSeleccion((Persona x) => x.edad);
            Assert.IsTrue(sResult == "edad");

            sResult = ExpressionUtil.ToSqlSeleccion((Persona x) => x.Fecha);
            Assert.IsTrue(sResult == "Fecha");

            sResult = ExpressionUtil.ToSqlSeleccion((Persona x) => x.Id);
            Assert.IsTrue(sResult == "Id");

            sResult = ExpressionUtil.ToSqlSeleccion((Persona x) => x.Nombre);
            Assert.IsTrue(sResult == "Nombre");

            sResult = ExpressionUtil.ToSqlSeleccion((Persona x) => x.Saldo);
            Assert.IsTrue(sResult == "Saldo");
        }
    }
}
