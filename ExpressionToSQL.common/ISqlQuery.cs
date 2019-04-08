using ExpressionToSQL.common.configuration;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionToSQL.common
{
    /// <summary>
    /// Interfaz de conversor de expresiones a SQL
    /// </summary>
    public interface ISqlQuery<T>
    {
        IQueryConfiguration Configuration { get; }

        string Select();
        /// <summary>
        /// Genera el código de SELECT
        /// </summary>
        /// <typeparam name="T">Tipo de la entidad</typeparam>
        /// <param name="Expression">Filtro</param>
        /// <param name="Pagina">Número de página</param>
        /// <param name="NumeroRegistros">Número de registros por página</param>
        /// <param name="NombreOrden">Nombre por que se ordena para filtrar</param>
        /// <returns></returns>
        string Select(int Pagina, int NumeroRegistros);
        /// <summary>
        /// Genera el código de COUNT
        /// </summary>
        /// <typeparam name="T">Tipo de la entidad</typeparam>
        /// <param name="Expression">Filtro</param>
        /// <returns></returns>
        string Count();
        /// <summary>
        /// Genera el código de FIRST
        /// </summary>
        /// <typeparam name="T">Tipo de la entidad</typeparam>
        /// <param name="Expression">Filtro</param>
        /// <returns></returns>
        string Fisrt();
        /// <summary>
        /// Calcula el máximo
        /// </summary>
        /// <typeparam name="T">Clase mapeada</typeparam>
        /// <typeparam name="TPropiedad">Tipo del campo a calcular el MAX</typeparam>
        /// <param name="Expression">WHERE para la consulta, si es NULL devuelve toda la colección</param>
        /// <param name="Propiedad">Propiedad sobre la que calcular el max</param>
        /// <returns>Instancias de objetos o null si no existe</returns>
        string Max<TPropiedad>(Expression<Func<T, TPropiedad>> Propiedad);

        /// <summary>
        /// Calcula el mínimo
        /// </summary>
        /// <typeparam name="T">Clase mapeada</typeparam>
        /// <typeparam name="TPropiedad">Tipo del campo a calcular el MAX</typeparam>
        /// <param name="Expression">WHERE para la consulta, si es NULL devuelve toda la colección</param>
        /// <param name="Propiedad">Propiedad sobre la que calcular el max</param>
        /// <returns>Instancias de objetos o null si no existe</returns>
        string Min<TPropiedad>(Expression<Func<T, TPropiedad>> Propiedad);
    }
}
