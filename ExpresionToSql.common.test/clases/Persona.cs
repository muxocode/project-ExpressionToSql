using ExpressionToSQL.common.attributes;
using System;

namespace ExpressionTExpressionUtil.common.test.clases
{
    public class Persona
    {
        public long Id { get; set; }
        public decimal? Saldo { get; set; }
        public DateTime? Fecha { get; set; }
        public string Nombre { get; set; }
        public int edad;
        [NonQuerable]
        public string Apellido1 { get; set; }
        [NonQuerable]
        public string apellido2;

    }
}