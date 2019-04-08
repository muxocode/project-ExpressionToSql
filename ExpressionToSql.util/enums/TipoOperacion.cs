using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionToSQL.util.enums
{
    internal enum TipoOperacion
    {
        //
        // Resumen:
        //     Nodo que representa la suma aritmética sin comprobación de desbordamiento.
        Add = 0,
        //
        // Resumen:
        //     Nodo que representa la suma aritmética con comprobación de desbordamiento.
        AddChecked = 1,
        //
        // Resumen:
        //     Nodo que representa una operación AND bit a bit.
        And = 2,
        //
        // Resumen:
        //     Nodo que representa una operación AND condicional de cortocircuito.
        AndAlso = 3,
        //
        // Resumen:
        //     Nodo que representa la obtención de la longitud de una matriz unidimensional.
        ArrayLength = 4,
        //
        // Resumen:
        //     Nodo que representa la indización en una matriz unidimensional.
        ArrayIndex = 5,
        //
        // Resumen:
        //     Nodo que representa la llamada a un método.
        Call = 6,
        //
        // Resumen:
        //     Nodo que representa una operación de uso combinado de null.
        Coalesce = 7,
        //
        // Resumen:
        //     Nodo que representa una operación condicional.
        Conditional = 8,
        //
        // Resumen:
        //     Nodo que representa una expresión que tiene un valor constante.
        Constant = 9,
        //
        // Resumen:
        //     Nodo que representa una operación de conversión. Si la operación es una conversión
        //     numérica, se produce automáticamente un desbordamiento si el valor convertido
        //     no se ajusta al tipo de destino.
        Convert = 10,
        //
        // Resumen:
        //     Nodo que representa una operación de conversión. Si la operación es una conversión
        //     numérica, se produce una excepción si el valor convertido no se ajusta al tipo
        //     de destino.
        ConvertChecked = 11,
        //
        // Resumen:
        //     Nodo que representa la división aritmética.
        Divide = 12,
        //
        // Resumen:
        //     Nodo que representa una comparación de igualdad.
        Equal = 13,
        //
        // Resumen:
        //     Nodo que representa una operación XOR bit a bit.
        ExclusiveOr = 14,
        //
        // Resumen:
        //     Nodo que representa una comparación numérica "mayor que".
        GreaterThan = 15,
        //
        // Resumen:
        //     Nodo que representa una comparación numérica "mayor o igual que".
        GreaterThanOrEqual = 16,
        //
        // Resumen:
        //     Nodo que representa la aplicación de un delegado o una expresión lambda a una
        //     lista de expresiones de argumento.
        Invoke = 17,
        //
        // Resumen:
        //     Nodo que representa una expresión lambda.
        Lambda = 18,
        //
        // Resumen:
        //     Nodo que representa una operación bit a bit de desplazamiento a la izquierda.
        LeftShift = 19,
        //
        // Resumen:
        //     Nodo que representa una comparación numérica "menor que".
        LessThan = 20,
        //
        // Resumen:
        //     Nodo que representa una comparación numérica "menor o igual que".
        LessThanOrEqual = 21,
        //
        // Resumen:
        //     Nodo que representa la creación de un nuevo objeto System.Collections.IEnumerable
        //     y su inicialización desde una lista de elementos.
        ListInit = 22,
        //
        // Resumen:
        //     Nodo que representa la lectura de un campo o una propiedad.
        MemberAccess = 23,
        //
        // Resumen:
        //     Nodo que representa la creación de un nuevo objeto y la inicialización de uno
        //     o varios de sus miembros.
        MemberInit = 24,
        //
        // Resumen:
        //     Nodo que representa una operación aritmética de resto.
        Modulo = 25,
        //
        // Resumen:
        //     Nodo que representa la multiplicación aritmética sin comprobación de desbordamiento.
        Multiply = 26,
        //
        // Resumen:
        //     Nodo que representa la multiplicación aritmética con comprobación de desbordamiento.
        MultiplyChecked = 27,
        //
        // Resumen:
        //     Nodo que representa una operación de negación aritmética.
        Negate = 28,
        //
        // Resumen:
        //     Nodo que representa una operación unaria +. El resultado de una operación unaria
        //     + predefinida es simplemente el valor del operando, pero las implementaciones
        //     definidas por el usuario pueden tener resultados no triviales.
        UnaryPlus = 29,
        //
        // Resumen:
        //     Nodo que representa una operación de negación aritmética con comprobación de
        //     desbordamiento.
        NegateChecked = 30,
        //
        // Resumen:
        //     Nodo que representa la llamada a un constructor para crear un nuevo objeto.
        New = 31,
        //
        // Resumen:
        //     Nodo que representa la creación de una nueva matriz unidimensional y su inicialización
        //     desde una lista de elementos.
        NewArrayInit = 32,
        //
        // Resumen:
        //     Nodo que representa la creación de una nueva matriz donde se especifican los
        //     límites de cada dimensión.
        NewArrayBounds = 33,
        //
        // Resumen:
        //     Nodo que representa una operación de complemento bit a bit.
        Not = 34,
        //
        // Resumen:
        //     Nodo que representa una comparación de desigualdad.
        NotEqual = 35,
        //
        // Resumen:
        //     Nodo que representa una operación OR bit a bit.
        Or = 36,
        //
        // Resumen:
        //     Nodo que representa una operación OR condicional de cortocircuito.
        OrElse = 37,
        //
        // Resumen:
        //     Nodo que representa una referencia a un parámetro definido en el contexto de
        //     la expresión.
        Parameter = 38,
        //
        // Resumen:
        //     Nodo que representa la elevación de un número a una potencia.
        Power = 39,
        //
        // Resumen:
        //     Nodo que representa una expresión que tiene un valor constante de tipo System.Linq.Expressions.Expression.
        //     Un nodo System.Linq.Expressions.ExpressionType.Quote puede contener referencias
        //     a los parámetros definidos en el contexto de la expresión que representa.
        Quote = 40,
        //
        // Resumen:
        //     Nodo que representa una operación bit a bit de desplazamiento a la derecha.
        RightShift = 41,
        //
        // Resumen:
        //     Nodo que representa la resta aritmética sin comprobación de desbordamiento.
        Subtract = 42,
        //
        // Resumen:
        //     Nodo que representa la resta aritmética con comprobación de desbordamiento.
        SubtractChecked = 43,
        //
        // Resumen:
        //     Nodo que representa una referencia explícita o conversión boxing donde se proporciona
        //     el valor null si se produce un error en la conversión.
        TypeAs = 44,
        //
        // Resumen:
        //     Nodo que representa una comprobación de tipo.
        TypeIs = 45,
        /// 
        /// Valor por defecto
        /// 
        Default = -1
    }
}
