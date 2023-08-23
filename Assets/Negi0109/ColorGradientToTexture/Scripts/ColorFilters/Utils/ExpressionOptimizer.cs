using System.Linq.Expressions;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework.Constraints;

namespace Negi0109.ColorGradientToTexture.Filters.Formulas
{
    public static class ExpressionOptimizer
    {
        public static class Utils
        {
            public static bool EqualConstraint(Expression expression, object value)
            {
                return expression is ConstantExpression ce && ce.Value == value;
            }
        }

        public static Expression PostProcessingExpression(Expression expression)
        {
            if (expression is BinaryExpression be)
            {
                if (TryConvertSimpleBinaryExpression(ref be)) return be;

                return Expression.MakeBinary(
                    be.NodeType,
                    PostProcessingExpression(be.Left),
                    PostProcessingExpression(be.Right)
                );
            }
            return expression;
        }

        private static bool TryConvertSimpleBinaryExpression(ref BinaryExpression expression)
        {
            if (expression.NodeType == ExpressionType.Add)
            {
                if (expression.Right is BinaryExpression bre && bre.NodeType == ExpressionType.Multiply)
                {
                    if (Utils.EqualConstraint(bre.Left, -1f))
                    {
                        expression = Expression.MakeBinary(
                            ExpressionType.Subtract,
                            PostProcessingExpression(expression.Left),
                            PostProcessingExpression(bre.Right)
                        );
                        return true;
                    }
                    else if (Utils.EqualConstraint(bre.Right, -1f))
                    {
                        expression = Expression.MakeBinary(
                            ExpressionType.Subtract,
                            PostProcessingExpression(expression.Left),
                            PostProcessingExpression(bre.Left)
                        );
                        return true;
                    }
                }
            }
            if (expression.NodeType == ExpressionType.Multiply)
            {
                if (expression.Right is BinaryExpression bre && bre.NodeType == ExpressionType.Divide)
                {
                    if (Utils.EqualConstraint(bre.Left, 1f))
                    {
                        expression = Expression.MakeBinary(
                            ExpressionType.Divide,
                            PostProcessingExpression(expression.Left),
                            PostProcessingExpression(bre.Right)
                        );
                        return true;
                    }
                }
            }

            return false;
        }

        public static Expression ReduceExpression(Expression expression)
        {
            if (expression is BinaryExpression be)
            {
                if (be.Left is ConstantExpression lc && be.Right is ConstantExpression rc)
                {
                    return ReduceConstantExpression(be.NodeType, lc, rc);
                }
                else if (TryConvertCommutativeExpression(ref be))
                {
                    return ReduceExpression(be);
                }
                else if (TryReduceCommutativeExpression(ref be))
                {
                    return be;
                }
            }

            return expression;
        }

        private static Expression ReduceConstantExpression(ExpressionType type, ConstantExpression left, ConstantExpression right)
        {
            float l = (float)left.Value;
            float r = (float)right.Value;

            return type switch
            {
                ExpressionType.Add => Expression.Constant(l + r),
                ExpressionType.Subtract => Expression.Constant(l - r),
                ExpressionType.Multiply => Expression.Constant(l * r),
                ExpressionType.Divide => Expression.Constant(l / r),
                _ => throw new System.NotImplementedException()
            };
        }

        private static bool TryConvertCommutativeExpression(ref BinaryExpression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Subtract:
                    expression = Expression.Add(
                        expression.Left,
                        ReduceExpression(Expression.Multiply(expression.Right, Expression.Constant(-1f)))
                    );
                    break;
                case ExpressionType.Divide:
                    {
                        var right = Expression.Divide(Expression.Constant(1f), expression.Right);
                        Expression tmp = right;

                        if (right.Left is ConstantExpression lc && right.Right is ConstantExpression rc)
                            tmp = ReduceConstantExpression(right.NodeType, lc, rc);

                        expression = Expression.Multiply(
                            expression.Left,
                            tmp
                        );
                    }
                    break;
                default: return false;
            }
            return true;
        }

        private static bool TryReduceCommutativeExpression(ref BinaryExpression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.Multiply:
                    break;
                default: return false;
            }
            expression = ReduceCommutativeExpression(expression);

            return true;
        }

        private static BinaryExpression ReduceCommutativeExpression(BinaryExpression expression)
        {
            var nodeType = expression.NodeType;
            var le = expression.Left as BinaryExpression;
            var re = expression.Right as BinaryExpression;

            if (le != null && re != null && le.NodeType == nodeType && re.NodeType == nodeType)
                return ReduceCommutativeExpression(expression.NodeType, le, re);
            if (le != null && le.NodeType == nodeType) return ReduceCommutativeExpression(expression.NodeType, le, expression.Right);
            if (re != null && re.NodeType == nodeType) return ReduceCommutativeExpression(expression.NodeType, re, expression.Left, false);

            return expression;
        }

        private static BinaryExpression ReduceCommutativeExpression(ExpressionType type, BinaryExpression left, BinaryExpression right)
        {
            if (left.Left is ConstantExpression llc)
            {
                if (right.Left is ConstantExpression rlc)
                    return Expression.MakeBinary(
                        type,
                        ReduceConstantExpression(type, llc, rlc),
                        ReduceExpression(Expression.MakeBinary(type, left.Right, right.Right))
                    );
                else if (right.Right is ConstantExpression rrc)
                    return Expression.MakeBinary(
                        type,
                        ReduceConstantExpression(type, llc, rrc),
                        ReduceExpression(Expression.MakeBinary(type, left.Right, right.Left))
                    );
            }
            else if (left.Right is ConstantExpression lrc)
            {
                if (right.Left is ConstantExpression rlc)
                    return Expression.MakeBinary(
                        type,
                        ReduceExpression(Expression.MakeBinary(type, left.Left, right.Right)),
                        ReduceConstantExpression(type, lrc, rlc)
                    );

                else if (right.Right is ConstantExpression rrc)
                    return Expression.MakeBinary(
                        type,
                        ReduceExpression(Expression.MakeBinary(type, left.Left, right.Left)),
                        ReduceConstantExpression(type, lrc, rrc)
                    );
            }
            else if (right.Left is ConstantExpression rlc)
            {
                return Expression.MakeBinary(
                    type,
                    rlc,
                    Expression.MakeBinary(type, left, right.Right)
                );
            }
            else if (right.Right is ConstantExpression rrc)
            {
                return Expression.MakeBinary(
                    type,
                    Expression.MakeBinary(type, left, right.Left),
                    rrc
                );
            }

            return Expression.MakeBinary(type, left, right);
        }

        private static BinaryExpression ReduceCommutativeExpression(ExpressionType type, BinaryExpression left, Expression right, bool ltor = true)
        {

            if (left.Left is ConstantExpression llc)
                if (right is ConstantExpression rc) return Expression.MakeBinary(type, ReduceConstantExpression(type, llc, rc), left.Right);
                else return Expression.MakeBinary(type, left.Left, Expression.MakeBinary(type, left.Right, right));
            if (left.Right is ConstantExpression lrc)
                if (right is ConstantExpression rc) return Expression.MakeBinary(type, left.Left, ReduceConstantExpression(type, lrc, rc));
                else return Expression.MakeBinary(type, Expression.MakeBinary(type, left.Left, right), left.Right);


            return ltor ?
                Expression.MakeBinary(type, left, right)
                : Expression.MakeBinary(type, right, left);
        }
    }
}
