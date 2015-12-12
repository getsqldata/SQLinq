﻿//Copyright (c) Chris Pietschmann 2013 (http://pietschsoft.com)
//Licensed under the GNU Library General Public License (LGPL)
//License can be found here: http://sqlinq.codeplex.com/license

using System.Linq.Expressions;
using SQLinq.Compiler;
using System;

namespace SQLinq.Dynamic
{
    public class DynamicSQLinqLambdaExpression<T> : IDynamicSQLinqExpression
    {
        public DynamicSQLinqLambdaExpression(string fieldName, LambdaExpression expression) // Expression<Func<T, bool>> expression)
        {
            this.FieldName = fieldName;
            this.Expression = expression;
        }

        public string FieldName { get; set; }
        public LambdaExpression Expression { get; set; } // Expression<Func<T, bool>> Expression { get; set; }

        public SqlExpressionCompilerResult Compile(int existingParameterCount = 0, string parameterNamePrefix = SqlExpressionCompiler.DefaultParameterNamePrefix)
        {
            if (string.IsNullOrEmpty((parameterNamePrefix ?? string.Empty).Trim()))
            {
                throw new ArgumentException("parameterNamePrefix must be specified.", "parameterNamePrefix");
            }

            var ps = this.Expression.Parameters;

            var compiler = new SqlExpressionCompiler(existingParameterCount) { ParameterNamePrefix = parameterNamePrefix };
            var result = compiler.Compile(this.Expression);

            var fieldName = DialectProvider.Dialect.ParseColumnName(this.FieldName);

            result.SQL = result.SQL.Replace("{FieldName}", fieldName);

            return result;
        }
    }
}