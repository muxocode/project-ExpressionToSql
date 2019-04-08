using ExpressionToSQL.common.configuration;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionToSQL.common
{
    /// <summary>
    /// Interfaz de conversor de expresiones a SQL
    /// </summary>
    public interface ISqlCommand<T>
    {
        IQueryConfiguration Configuration { get; }
        /// <summary>
        /// Genera el código de INSERT
        /// </summary>
        /// <typeparam name="T">Tipo de la entidad</typeparam>
        /// <param name="Entidad">Entidad</param>
        /// <param name="IdAutoGenerado">Indica si el ID es autogenerado</param>
        /// <param name="NombreId">Nombre del Id de la tabla, si no existe lo toma de Configuration</param>
        /// <returns></returns>
        string Insert(params T[] Entidad);
        /// <summary>
        /// Genera el código de DELETE
        /// </summary>
        /// <typeparam name="T">Tipo de la entidad</typeparam>
        /// <param name="Id">Valor de Id</param>
        /// <param name="NombreId">Nombre dle Id de la tabla</param>
        /// <returns></returns>
        /// <typeparam name="TPrimaryKey">Tipo de la clave primaria</typeparam>
        string Delete();
        /// <summary>
        /// Genera el código de UPADTE
        /// </summary>
        /// <typeparam name="T">Tipo de la entidad</typeparam>
        /// <param name="Id">Valor de Id</param>
        /// <param name="Entidad">Entidad</param>
        /// <typeparam name="TPrimaryKey">Tipo de la clave primara</typeparam>
        /// <param name="NombreId">Nombre dle Id de la tabla</param>
        /// <returns></returns>
        string Update(T Entidad);
        /// <summary>
        /// Update en BBDD
        /// </summary>
        /// <typeparam name="T">Clase mapeada</typeparam>
        /// <param name="CamposValores">Datos a actualizar</param>
        /// <param name="Predicado">Where de la consulta</param>
        /// <returns>Devulve verdadero cuando todo ha ido bien</returns>
        string Update(IDictionary<string, object> CamposValores);
    }
}
