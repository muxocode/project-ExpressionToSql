using Dapper;
using ExpresionToSql.util.test.clases;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ExpressionToSql.sql.test
{
    [TestClass]
    public class Mssql
    {
        string createTable(string name) => $@"CREATE TABLE {name}(
                                 Id decimal PRIMARY KEY,
                                 Nombre VARCHAR (50) NOT NULL,
                                 MayorEdad bit NOT NULL
                                );";

        string removeAll() => $@"DECLARE @sql NVARCHAR(max)=''

                                SELECT @sql += ' Drop table ' + QUOTENAME(TABLE_SCHEMA) + '.'+ QUOTENAME(TABLE_NAME) + '; '
                                FROM INFORMATION_SCHEMA.TABLES
                                WHERE TABLE_TYPE = 'BASE TABLE' and TABLE_NAME like '%{Token.Value}'

                                Exec Sp_executesql @sql";

        string connection = "Server=localhost;Database=Master;User Id = SA;Password=deVops.Docker!";

        Lazy<string> Token = new Lazy<string>(() =>
                {
                    return DateTime.UtcNow.ToBinary().ToString();
 
                });
            
        

        Paciente[] getEntities()
        {
            var aLista = new List<Paciente>();

            for (int i = 1; i < 100; i++)
            {
                aLista.Add(new Paciente
                {
                    Id = i,
                    MayorEdad = i % 2 == 0,
                    Nombre = $"Paciente{i}"
                });
            }

            return aLista.ToArray();
        }

        void Create(IDbConnection dbConnection, string table)
        {
                dbConnection.Execute(createTable(table));
        }

        internal IDbConnection Connection
        {
            get
            {
                return new SqlConnection(connection);
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestCleanup]
        public void TestCleanup()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                //Borramos la tabla
                dbConnection.Execute(removeAll());
            }
        }

        [TestMethod]
        public void Flujo_Command()
        {

            string table = $"Flujo_Command{Token.Value}";
            var oSqlParser = new ExpressionToSQL.SqlParse();
            var oCommand = oSqlParser.Command<Paciente>().Configure(includeId: true, tableName: table);
            var oQuery = oSqlParser.Query<Paciente>().Configure(tableName: table);



            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                //Creamos la tabla
                Create(dbConnection, table);
                createTable(table);

                //Comprobamos la creación
                var sQuery = oQuery.Select();
                IEnumerable<Paciente> aLista = dbConnection.Query<Paciente>(sQuery);
                Assert.IsTrue(aLista.Count() == 0);


                //INSERCION
                var aEntities = getEntities();
                sQuery = oCommand.Insert(getEntities());
                dbConnection.Execute(sQuery);

                //Listado
                sQuery = oQuery.Select();
                aLista = dbConnection.Query<Paciente>(sQuery);
                Assert.IsTrue(aEntities.Count() == aLista.Count());

                //Update
                var oObj = aLista.First();
                oObj.Nombre = "MuxoCode";

                sQuery = oCommand.Where(x => x.Id == oObj.Id).Update(oObj);
                dbConnection.Execute(sQuery);

                //Ontenemos elemento
                sQuery = oQuery.Where(x => x.Id == oObj.Id).Select();
                var newObj = dbConnection.Query<Paciente>(sQuery).Single();
                Assert.IsTrue(newObj.Id == oObj.Id && newObj.Nombre == "MuxoCode");

                //Nuevo
                var oNewElement = new Paciente()
                {
                    Id = 5000,
                    Nombre = "New Person",
                    MayorEdad = true
                };
                sQuery = oCommand.Insert(oNewElement);
                dbConnection.Execute(sQuery);
                sQuery = oQuery.Where(x => x.Id == 5000).Where(x => x.Id > 4999).Select();
          
                var oSearch = dbConnection.Query<Paciente>(sQuery).Single();
                Assert.IsTrue(oSearch.Id == 5000);

                //Borrado
                sQuery = oCommand.Where(x => x.Id == 5000).Delete();
                dbConnection.Execute(sQuery);

                sQuery = oQuery.Where(x => x.Id == 5000).Select();

                oSearch = dbConnection.Query<Paciente>(sQuery).SingleOrDefault();
                Assert.IsTrue(oSearch == null);
            }
        }

        [TestMethod]
        public void Flujo_Query()
        {

            string table = $"Flujo_Query_{Token.Value}";
            var oSqlParser = new ExpressionToSQL.SqlParse();
            var oQuery = oSqlParser.Query<Paciente>().Configure(tableName: table);
            var oCommand = oSqlParser.Command<Paciente>().Configure(includeId: true, tableName: table);



            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                //Creamos la tabla
                Create(dbConnection, table);
                createTable(table);

                //Comprobamos la creación
                var sQuery = oQuery.Select();
                IEnumerable<Paciente> aLista = dbConnection.Query<Paciente>(sQuery);
                Assert.IsTrue(aLista.Count() == 0);

                //INSERCION
                var aEntities = getEntities();
                sQuery = oCommand.Insert(getEntities());
                dbConnection.Execute(sQuery);

                //MAX
                sQuery = oQuery.Max(x => x.Id);
                var oMax = dbConnection.ExecuteScalar<long>(sQuery);
                Assert.IsTrue(oMax == 99);

                sQuery = oQuery.Where(x => x.Id < 10).Max(x => x.Id);
                oMax = dbConnection.ExecuteScalar<long>(sQuery);
                Assert.IsTrue(oMax == 9);

                sQuery = oQuery.GroupBy(x => x.MayorEdad).Max(x => x.Id);
                oMax = dbConnection.ExecuteScalar<long>(sQuery);
                Assert.IsTrue(oMax == 99);

                //COUNT
                sQuery = oQuery.Count();
                var oCount = dbConnection.ExecuteScalar<long>(sQuery);
                Assert.IsTrue(oCount == 99);

                sQuery = oQuery.Where(x => x.Id < 10).Max(x => x.Id);
                oCount = dbConnection.ExecuteScalar<long>(sQuery);
                Assert.IsTrue(oCount == 9);

                sQuery = oQuery.GroupBy(x => x.MayorEdad).Count();
                oCount = dbConnection.ExecuteScalar<long>(sQuery);
                Assert.IsTrue(oCount == 50);

                //MIN
                sQuery = oQuery.Min(x => x.Id);
                var oMin = dbConnection.ExecuteScalar<long>(sQuery);
                Assert.IsTrue(oMin == 1);

                sQuery = oQuery.Where(x => x.Id < 10).Min(x => x.Id);
                oMin = dbConnection.ExecuteScalar<long>(sQuery);
                Assert.IsTrue(oMin == 1);

                sQuery = oQuery.Where(x => x.Id < 10).GroupBy(x => x.MayorEdad).Min(x => x.Id);
                oMin = dbConnection.ExecuteScalar<long>(sQuery);
                Assert.IsTrue(oMin == 1);

                //SUM
                sQuery = oQuery.Sum(x => x.Id);
                var oSum = dbConnection.ExecuteScalar<long>(sQuery);
                Assert.IsTrue(oSum == 4950);

                sQuery = oQuery.Where(x => x.Id < 10).Sum(x => x.Id);
                oSum = dbConnection.ExecuteScalar<long>(sQuery);
                Assert.IsTrue(oSum == 45);


                sQuery = oQuery.Where(x => x.Id < 10).GroupBy(x => x.MayorEdad).Sum(x => x.Id);
                oSum = dbConnection.ExecuteScalar<long>(sQuery);
                Assert.IsTrue(oSum == 25);


                //FIRST
                sQuery = oQuery.Fisrt();
                var oPaciente = dbConnection.Query<Paciente>(sQuery).SingleOrDefault();
                Assert.IsTrue(oPaciente.Id == 1);

                sQuery = oQuery.Where(x => x.Id < 10).Fisrt();
                oPaciente = dbConnection.Query<Paciente>(sQuery).SingleOrDefault();
                Assert.IsTrue(oPaciente.Id == 1);

                sQuery = oQuery.Where(x => x.Id > 10).OrderBy(x => x.Nombre).Fisrt(); 
                oPaciente = dbConnection.Query<Paciente>(sQuery).SingleOrDefault();
                Assert.IsTrue(oPaciente.Id == 11);

                //LIST
                var aListaNum = new List<long>() { 1, 2, 3, 4, 5 };
                sQuery = oQuery.Where(x => aListaNum.Contains(x.Id)).Select();
                var aListaPac = dbConnection.Query<Paciente>(sQuery);
                Assert.IsTrue(aListaPac.Count() == 5);


                //BORRAMOS LA TABLA
                sQuery = oCommand.Delete();
                dbConnection.Execute(sQuery);

                //MAX
                sQuery = oQuery.Max(x => x.Id);
                oMax = dbConnection.ExecuteScalar<long>(sQuery);
                Assert.IsTrue(oMax == 0);

                sQuery = oQuery.Where(x => x.Id < 10).Max(x => x.Id);
                oMax = dbConnection.ExecuteScalar<long>(sQuery);
                Assert.IsTrue(oMax == 0);

                sQuery = oQuery.GroupBy(x => x.MayorEdad).Max(x => x.Id);
                oMax = dbConnection.ExecuteScalar<long>(sQuery);
                Assert.IsTrue(oMax == 0);

                //COUNT
                sQuery = oQuery.Count();
                oCount = dbConnection.ExecuteScalar<long>(sQuery);
                Assert.IsTrue(oCount == 0);

                sQuery = oQuery.Where(x => x.Id < 10).Max(x => x.Id);
                oCount = dbConnection.ExecuteScalar<long>(sQuery);
                Assert.IsTrue(oCount == 0);

                sQuery = oQuery.GroupBy(x => x.MayorEdad).Count();
                oCount = dbConnection.ExecuteScalar<long>(sQuery);
                Assert.IsTrue(oCount == 0);

                //MIN
                sQuery = oQuery.Min(x => x.Id);
                oMin = dbConnection.ExecuteScalar<long>(sQuery);
                Assert.IsTrue(oMin == 0);

                sQuery = oQuery.Where(x => x.Id < 10).Min(x => x.Id);
                oMin = dbConnection.ExecuteScalar<long>(sQuery);
                Assert.IsTrue(oMin == 0);

                sQuery = oQuery.Where(x => x.Id < 10).GroupBy(x => x.MayorEdad).Min(x => x.Id);
                oMin = dbConnection.ExecuteScalar<long>(sQuery);
                Assert.IsTrue(oMin == 0);

                //SUM
                sQuery = oQuery.Sum(x => x.Id);
                oSum = dbConnection.ExecuteScalar<long>(sQuery);
                Assert.IsTrue(oSum == 0);

                sQuery = oQuery.Where(x => x.Id < 10).Sum(x => x.Id);
                oSum = dbConnection.ExecuteScalar<long>(sQuery);
                Assert.IsTrue(oSum == 0);


                sQuery = oQuery.Where(x => x.Id < 10).GroupBy(x => x.MayorEdad).Sum(x => x.Id);
                oSum = dbConnection.ExecuteScalar<long>(sQuery);
                Assert.IsTrue(oSum == 0);


                //FIRST
                sQuery = oQuery.Fisrt();
                oPaciente = dbConnection.Query<Paciente>(sQuery).SingleOrDefault();
                Assert.IsTrue(oPaciente == null);

                sQuery = oQuery.Where(x => x.Id < 10).Fisrt();
                oPaciente = dbConnection.Query<Paciente>(sQuery).SingleOrDefault();
                Assert.IsTrue(oPaciente == null);

                sQuery = oQuery.Where(x => x.Id > 10).OrderBy(x => x.Nombre).Fisrt();
                oPaciente = dbConnection.Query<Paciente>(sQuery).SingleOrDefault();
                Assert.IsTrue(oPaciente == null);

                //LIST
                sQuery = oQuery.Where(x => aListaNum.Contains(x.Id)).Select();
                aListaPac = dbConnection.Query<Paciente>(sQuery);
                Assert.IsTrue(aListaPac.Count() == 0);

            }
        }
    }
}
