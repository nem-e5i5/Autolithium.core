﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Autolithium.core
{
    public partial class LiParser
    {
        private Expression ParsePrimary(Type Desired = null)
        {
            ConsumeWS();
            char ch = Read()[0];
            Expression Ret;
            string szTemp;
            switch (ch)
            {
                case ']':
                    throw new AutoitException(AutoitExceptionType.UNBALANCEDSUBSCRIPT, LineNumber, Cursor);
                case '(':
                    Ret = ParseBoolean();
                    if (Peek() != ")") throw new AutoitException(AutoitExceptionType.UNBALANCEDPAREN, LineNumber, Cursor);
                    Consume();
                    return Ret;
                case ')':
                    throw new AutoitException(AutoitExceptionType.UNBALANCEDPAREN, LineNumber, Cursor);
                case ';':
                    return null;

                case '$':
                    szTemp = Getstr(Reg_AlphaNum);

                    if (szTemp == "")
                        throw new AutoitException(AutoitExceptionType.LEXER_BADFORMAT, LineNumber, Cursor);
                    if (TryParseSubscript(out Ret))
                    {
                        szTemp += "[]";
                        if (!VarCompilerEngine.Get.ContainsKey(szTemp))
                        {
                            var a = VarCompilerEngine.Createvar(szTemp);
                            VarCompilerEngine.Get[szTemp].ArrayIndex.Push(Ret);
                            a.ActualType.Add(typeof(object[]));
                            a.PolymorphList.Add(typeof(object[]), ParameterExpression.Parameter(typeof(object[]), szTemp));
                            return Expression.Assign(VarCompilerEngine.Get[szTemp].ActualValue,
                                Expression.NewArrayBounds(typeof(object), Ret));
                        }
                        VarCompilerEngine.Get[szTemp].ArrayIndex.Push(Ret);
                        Type t;
                        if (TryParseCast(out t))
                            VarCompilerEngine.Get[szTemp].MyType.Push(t);
                        else VarCompilerEngine.Get[szTemp].MyType.Push(null);
                        return Expression.Parameter(typeof(object[]), szTemp);
                    }
                    if (!VarCompilerEngine.Get.ContainsKey(szTemp))
                    {
                        VarCompilerEngine.Createvar(szTemp);
                        return Expression.Parameter(typeof(object), szTemp);
                    }
                    else return VarCompilerEngine.Get[szTemp].ActualValue;

                case '@':
                    szTemp = Getstr(Reg_AlphaNum);

                    if (szTemp == "")
                    {
                        throw new AutoitException(AutoitExceptionType.LEXER_BADFORMAT, LineNumber, Cursor);
                    }
                    return Expression.Call(BasicMacro.GetMacroInfo, Expression.Constant(szTemp, typeof(string)));

                case '"':
                case '\'':
                    SeekRelative(-1);
                    return Expression.Constant(Lexer_CSString(), typeof(string));
                default:

                    SeekRelative(-1);
                    szTemp = GetNbr();
                    if (szTemp != "")
                    {
                        if (szTemp.Contains(".") || Math.Abs(double.Parse(szTemp, CultureInfo.InvariantCulture)) > long.MaxValue)
                            return Expression.Constant(double.Parse(szTemp, CultureInfo.InvariantCulture), typeof(double));
                        else if (Math.Abs(double.Parse(szTemp, CultureInfo.InvariantCulture)) > int.MaxValue)
                            return Expression.Constant(long.Parse(szTemp, CultureInfo.InvariantCulture), typeof(long));
                        else
                            return Expression.Constant(int.Parse(szTemp, CultureInfo.InvariantCulture), typeof(int));
                    }
                    szTemp = Getstr(Reg_AlphaNum);
                    if (szTemp != "") return ParseKeywordOrFunc(szTemp);
                    throw new AutoitException(AutoitExceptionType.LEXER_NOTRECOGNISED, LineNumber, Cursor, "" + ch);
            }
        }
    }
}
