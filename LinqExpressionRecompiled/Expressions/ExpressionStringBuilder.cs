﻿namespace System.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Dynamic;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;

    internal sealed class ExpressionStringBuilder : ExpressionVisitor
    {
        private Dictionary<object, int> _ids;
        private StringBuilder _out = new StringBuilder();

        private ExpressionStringBuilder()
        {
        }

        private void AddLabel(LabelTarget label)
        {
            if (this._ids == null)
            {
                this._ids = new Dictionary<object, int>();
                this._ids.Add(label, 0);
            }
            else if (!this._ids.ContainsKey(label))
            {
                this._ids.Add(label, this._ids.Count);
            }
        }

        private void AddParam(ParameterExpression p)
        {
            if (this._ids == null)
            {
                this._ids = new Dictionary<object, int>();
                this._ids.Add(this._ids, 0);
            }
            else if (!this._ids.ContainsKey(p))
            {
                this._ids.Add(p, this._ids.Count);
            }
        }

        internal static string CatchBlockToString(CatchBlock node)
        {
            ExpressionStringBuilder builder = new ExpressionStringBuilder();
            builder.VisitCatchBlock(node);
            return builder.ToString();
        }

        private void DumpLabel(LabelTarget target)
        {
            if (!string.IsNullOrEmpty(target.Name))
            {
                this.Out(target.Name);
            }
            else
            {
                this.Out("UnamedLabel_" + this.GetLabelId(target));
            }
        }

        internal static string ElementInitBindingToString(ElementInit node)
        {
            ExpressionStringBuilder builder = new ExpressionStringBuilder();
            builder.VisitElementInit(node);
            return builder.ToString();
        }

        internal static string ExpressionToString(Expression node)
        {
            ExpressionStringBuilder builder = new ExpressionStringBuilder();
            builder.Visit(node);
            return builder.ToString();
        }

        private static string FormatBinder(CallSiteBinder binder)
        {
            ConvertBinder binder2 = binder as ConvertBinder;
            if (binder2 != null)
            {
                return ("Convert " + binder2.Type);
            }
            GetMemberBinder binder3 = binder as GetMemberBinder;
            if (binder3 != null)
            {
                return ("GetMember " + binder3.Name);
            }
            SetMemberBinder binder4 = binder as SetMemberBinder;
            if (binder4 != null)
            {
                return ("SetMember " + binder4.Name);
            }
            DeleteMemberBinder binder5 = binder as DeleteMemberBinder;
            if (binder5 != null)
            {
                return ("DeleteMember " + binder5.Name);
            }
            if (binder is GetIndexBinder)
            {
                return "GetIndex";
            }
            if (binder is SetIndexBinder)
            {
                return "SetIndex";
            }
            if (binder is DeleteIndexBinder)
            {
                return "DeleteIndex";
            }
            InvokeMemberBinder binder6 = binder as InvokeMemberBinder;
            if (binder6 != null)
            {
                return ("Call " + binder6.Name);
            }
            if (binder is InvokeBinder)
            {
                return "Invoke";
            }
            if (binder is CreateInstanceBinder)
            {
                return "Create";
            }
            UnaryOperationBinder binder7 = binder as UnaryOperationBinder;
            if (binder7 != null)
            {
                return binder7.Operation.ToString();
            }
            BinaryOperationBinder binder8 = binder as BinaryOperationBinder;
            if (binder8 != null)
            {
                return binder8.Operation.ToString();
            }
            return "CallSiteBinder";
        }

        private int GetLabelId(LabelTarget label)
        {
            int count;
            if (this._ids == null)
            {
                this._ids = new Dictionary<object, int>();
                this.AddLabel(label);
                return 0;
            }
            if (!this._ids.TryGetValue(label, out count))
            {
                count = this._ids.Count;
                this.AddLabel(label);
            }
            return count;
        }

        private int GetParamId(ParameterExpression p)
        {
            int count;
            if (this._ids == null)
            {
                this._ids = new Dictionary<object, int>();
                this.AddParam(p);
                return 0;
            }
            if (!this._ids.TryGetValue(p, out count))
            {
                count = this._ids.Count;
                this.AddParam(p);
            }
            return count;
        }

        internal static string MemberBindingToString(MemberBinding node)
        {
            ExpressionStringBuilder builder = new ExpressionStringBuilder();
            builder.VisitMemberBinding(node);
            return builder.ToString();
        }

        private void Out(char c)
        {
            this._out.Append(c);
        }

        private void Out(string s)
        {
            this._out.Append(s);
        }

        private void OutMember(Expression instance, MemberInfo member)
        {
            if (instance != null)
            {
                this.Visit(instance);
                this.Out("." + member.Name);
            }
            else
            {
                this.Out(member.DeclaringType.Name + "." + member.Name);
            }
        }

        internal static string SwitchCaseToString(SwitchCase node)
        {
            ExpressionStringBuilder builder = new ExpressionStringBuilder();
            builder.VisitSwitchCase(node);
            return builder.ToString();
        }

        public override string ToString()
        {
            return this._out.ToString();
        }

        protected internal override Expression VisitBinary(BinaryExpression node)
        {
            string str;
            switch (node.NodeType)
            {
                case ExpressionType.Add:
                    str = "+";
                    break;

                case ExpressionType.AddChecked:
                    str = "+";
                    break;

                case ExpressionType.And:
                    if (!(node.Type == typeof(bool)) && !(node.Type == typeof(bool?)))
                    {
                        str = "&";
                    }
                    else
                    {
                        str = "And";
                    }
                    break;

                case ExpressionType.AndAlso:
                    str = "AndAlso";
                    break;

                case ExpressionType.Coalesce:
                    str = "??";
                    break;

                case ExpressionType.Divide:
                    str = "/";
                    break;

                case ExpressionType.Equal:
                    str = "==";
                    break;

                case ExpressionType.ExclusiveOr:
                    str = "^";
                    break;

                case ExpressionType.GreaterThan:
                    str = ">";
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    str = ">=";
                    break;

                case ExpressionType.LeftShift:
                    str = "<<";
                    break;

                case ExpressionType.LessThan:
                    str = "<";
                    break;

                case ExpressionType.LessThanOrEqual:
                    str = "<=";
                    break;

                case ExpressionType.Modulo:
                    str = "%";
                    break;

                case ExpressionType.Multiply:
                    str = "*";
                    break;

                case ExpressionType.MultiplyChecked:
                    str = "*";
                    break;

                case ExpressionType.NotEqual:
                    str = "!=";
                    break;

                case ExpressionType.Or:
                    if (!(node.Type == typeof(bool)) && !(node.Type == typeof(bool?)))
                    {
                        str = "|";
                    }
                    else
                    {
                        str = "Or";
                    }
                    break;

                case ExpressionType.OrElse:
                    str = "OrElse";
                    break;

                case ExpressionType.Power:
                    str = "^";
                    break;

                case ExpressionType.RightShift:
                    str = ">>";
                    break;

                case ExpressionType.Subtract:
                    str = "-";
                    break;

                case ExpressionType.SubtractChecked:
                    str = "-";
                    break;

                case ExpressionType.Assign:
                    str = "=";
                    break;

                case ExpressionType.AddAssign:
                    str = "+=";
                    break;

                case ExpressionType.AndAssign:
                    if (!(node.Type == typeof(bool)) && !(node.Type == typeof(bool?)))
                    {
                        str = "&=";
                    }
                    else
                    {
                        str = "&&=";
                    }
                    break;

                case ExpressionType.DivideAssign:
                    str = "/=";
                    break;

                case ExpressionType.ExclusiveOrAssign:
                    str = "^=";
                    break;

                case ExpressionType.LeftShiftAssign:
                    str = "<<=";
                    break;

                case ExpressionType.ModuloAssign:
                    str = "%=";
                    break;

                case ExpressionType.MultiplyAssign:
                    str = "*=";
                    break;

                case ExpressionType.OrAssign:
                    if (!(node.Type == typeof(bool)) && !(node.Type == typeof(bool?)))
                    {
                        str = "|=";
                    }
                    else
                    {
                        str = "||=";
                    }
                    break;

                case ExpressionType.PowerAssign:
                    str = "**=";
                    break;

                case ExpressionType.RightShiftAssign:
                    str = ">>=";
                    break;

                case ExpressionType.SubtractAssign:
                    str = "-=";
                    break;

                case ExpressionType.AddAssignChecked:
                    str = "+=";
                    break;

                case ExpressionType.MultiplyAssignChecked:
                    str = "*=";
                    break;

                case ExpressionType.SubtractAssignChecked:
                    str = "-=";
                    break;

                case ExpressionType.ArrayIndex:
                    this.Visit(node.Left);
                    this.Out("[");
                    this.Visit(node.Right);
                    this.Out("]");
                    return node;

                default:
                    throw new InvalidOperationException();
            }
            this.Out("(");
            this.Visit(node.Left);
            this.Out(' ');
            this.Out(str);
            this.Out(' ');
            this.Visit(node.Right);
            this.Out(")");
            return node;
        }

        protected internal override Expression VisitBlock(BlockExpression node)
        {
            this.Out("{");
            foreach (ParameterExpression expression in node.Variables)
            {
                this.Out("var ");
                this.Visit(expression);
                this.Out(";");
            }
            this.Out(" ... }");
            return node;
        }

        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            this.Out("catch (" + node.Test.Name);
            if (node.Variable != null)
            {
                this.Out(node.Variable.Name ?? "");
            }
            this.Out(") { ... }");
            return node;
        }

        protected internal override Expression VisitConditional(ConditionalExpression node)
        {
            this.Out("IIF(");
            this.Visit(node.Test);
            this.Out(", ");
            this.Visit(node.IfTrue);
            this.Out(", ");
            this.Visit(node.IfFalse);
            this.Out(")");
            return node;
        }

        protected internal override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value != null)
            {
                string s = node.Value.ToString();
                if (node.Value is string)
                {
                    this.Out("\"");
                    this.Out(s);
                    this.Out("\"");
                    return node;
                }
                if (s == node.Value.GetType().ToString())
                {
                    this.Out("value(");
                    this.Out(s);
                    this.Out(")");
                    return node;
                }
                this.Out(s);
                return node;
            }
            this.Out("null");
            return node;
        }

        protected internal override Expression VisitDebugInfo(DebugInfoExpression node)
        {
            string s = string.Format(CultureInfo.CurrentCulture, "<DebugInfo({0}: {1}, {2}, {3}, {4})>", new object[] { node.Document.FileName, node.StartLine, node.StartColumn, node.EndLine, node.EndColumn });
            this.Out(s);
            return node;
        }

        protected internal override Expression VisitDefault(DefaultExpression node)
        {
            this.Out("default(");
            this.Out(node.Type.Name);
            this.Out(")");
            return node;
        }

        protected internal override Expression VisitDynamic(DynamicExpression node)
        {
            this.Out(FormatBinder(node.Binder));
            this.VisitExpressions<Expression>('(', node.Arguments, ')');
            return node;
        }

        protected override ElementInit VisitElementInit(ElementInit initializer)
        {
            this.Out(initializer.AddMethod.ToString());
            string seperator = ", ";
            this.VisitExpressions<Expression>('(', initializer.Arguments, ')', seperator);
            return initializer;
        }

        private void VisitExpressions<T>(char open, IList<T> expressions, char close) where T: Expression
        {
            this.VisitExpressions<T>(open, expressions, close, ", ");
        }

        private void VisitExpressions<T>(char open, IList<T> expressions, char close, string seperator) where T: Expression
        {
            this.Out(open);
            if (expressions != null)
            {
                bool flag = true;
                foreach (T local in expressions)
                {
                    if (flag)
                    {
                        flag = false;
                    }
                    else
                    {
                        this.Out(seperator);
                    }
                    this.Visit(local);
                }
            }
            this.Out(close);
        }

        protected internal override Expression VisitExtension(Expression node)
        {
            BindingFlags bindingAttr = BindingFlags.ExactBinding | BindingFlags.Public | BindingFlags.Instance;
            if (node.GetType().GetMethod("ToString", bindingAttr, null, Type.EmptyTypes, null).DeclaringType != typeof(Expression))
            {
                this.Out(node.ToString());
                return node;
            }
            this.Out("[");
            if (node.NodeType == ExpressionType.Extension)
            {
                this.Out(node.GetType().FullName);
            }
            else
            {
                this.Out(node.NodeType.ToString());
            }
            this.Out("]");
            return node;
        }

        protected internal override Expression VisitGoto(GotoExpression node)
        {
            this.Out(node.Kind.ToString().ToLower(CultureInfo.CurrentCulture));
            this.DumpLabel(node.Target);
            if (node.Value != null)
            {
                this.Out(" (");
                this.Visit(node.Value);
                this.Out(") ");
            }
            return node;
        }

        protected internal override Expression VisitIndex(IndexExpression node)
        {
            if (node.Object != null)
            {
                this.Visit(node.Object);
            }
            else
            {
                this.Out(node.Indexer.DeclaringType.Name);
            }
            if (node.Indexer != null)
            {
                this.Out(".");
                this.Out(node.Indexer.Name);
            }
            this.VisitExpressions<Expression>('[', node.Arguments, ']');
            return node;
        }

        protected internal override Expression VisitInvocation(InvocationExpression node)
        {
            this.Out("Invoke(");
            this.Visit(node.Expression);
            string s = ", ";
            int num = 0;
            int count = node.Arguments.Count;
            while (num < count)
            {
                this.Out(s);
                this.Visit(node.Arguments[num]);
                num++;
            }
            this.Out(")");
            return node;
        }

        protected internal override Expression VisitLabel(LabelExpression node)
        {
            this.Out("{ ... } ");
            this.DumpLabel(node.Target);
            this.Out(":");
            return node;
        }

        protected internal override Expression VisitLambda<T>(Expression<T> node)
        {
            if (node.Parameters.Count == 1)
            {
                this.Visit(node.Parameters[0]);
            }
            else
            {
                this.VisitExpressions<ParameterExpression>('(', node.Parameters, ')');
            }
            this.Out(" => ");
            this.Visit(node.Body);
            return node;
        }

        protected internal override Expression VisitListInit(ListInitExpression node)
        {
            this.Visit(node.NewExpression);
            this.Out(" {");
            int num = 0;
            int count = node.Initializers.Count;
            while (num < count)
            {
                if (num > 0)
                {
                    this.Out(", ");
                }
                this.Out(node.Initializers[num].ToString());
                num++;
            }
            this.Out("}");
            return node;
        }

        protected internal override Expression VisitLoop(LoopExpression node)
        {
            this.Out("loop { ... }");
            return node;
        }

        protected internal override Expression VisitMember(MemberExpression node)
        {
            this.OutMember(node.Expression, node.Member);
            return node;
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
        {
            this.Out(assignment.Member.Name);
            this.Out(" = ");
            this.Visit(assignment.Expression);
            return assignment;
        }

        protected internal override Expression VisitMemberInit(MemberInitExpression node)
        {
            if ((node.NewExpression.Arguments.Count == 0) && node.NewExpression.Type.Name.Contains("<"))
            {
                this.Out("new");
            }
            else
            {
                this.Visit(node.NewExpression);
            }
            this.Out(" {");
            int num = 0;
            int count = node.Bindings.Count;
            while (num < count)
            {
                MemberBinding binding = node.Bindings[num];
                if (num > 0)
                {
                    this.Out(", ");
                }
                this.VisitMemberBinding(binding);
                num++;
            }
            this.Out("}");
            return node;
        }

        protected override MemberListBinding VisitMemberListBinding(MemberListBinding binding)
        {
            this.Out(binding.Member.Name);
            this.Out(" = {");
            int num = 0;
            int count = binding.Initializers.Count;
            while (num < count)
            {
                if (num > 0)
                {
                    this.Out(", ");
                }
                this.VisitElementInit(binding.Initializers[num]);
                num++;
            }
            this.Out("}");
            return binding;
        }

        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
        {
            this.Out(binding.Member.Name);
            this.Out(" = {");
            int num = 0;
            int count = binding.Bindings.Count;
            while (num < count)
            {
                if (num > 0)
                {
                    this.Out(", ");
                }
                this.VisitMemberBinding(binding.Bindings[num]);
                num++;
            }
            this.Out("}");
            return binding;
        }

        protected internal override Expression VisitMethodCall(MethodCallExpression node)
        {
            int num = 0;
            Expression expression = node.Object;
            if (Attribute.GetCustomAttribute(node.Method, typeof(ExtensionAttribute)) != null)
            {
                num = 1;
                expression = node.Arguments[0];
            }
            if (expression != null)
            {
                this.Visit(expression);
                this.Out(".");
            }
            this.Out(node.Method.Name);
            this.Out("(");
            int num2 = num;
            int count = node.Arguments.Count;
            while (num2 < count)
            {
                if (num2 > num)
                {
                    this.Out(", ");
                }
                this.Visit(node.Arguments[num2]);
                num2++;
            }
            this.Out(")");
            return node;
        }

        protected internal override Expression VisitNew(NewExpression node)
        {
            this.Out("new " + node.Type.Name);
            this.Out("(");
            ReadOnlyCollection<MemberInfo> members = node.Members;
            for (int i = 0; i < node.Arguments.Count; i++)
            {
                if (i > 0)
                {
                    this.Out(", ");
                }
                if (members != null)
                {
                    string name = members[i].Name;
                    this.Out(name);
                    this.Out(" = ");
                }
                this.Visit(node.Arguments[i]);
            }
            this.Out(")");
            return node;
        }

        protected internal override Expression VisitNewArray(NewArrayExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.NewArrayInit:
                    this.Out("new [] ");
                    this.VisitExpressions<Expression>('{', node.Expressions, '}');
                    return node;

                case ExpressionType.NewArrayBounds:
                    this.Out("new " + node.Type.ToString());
                    this.VisitExpressions<Expression>('(', node.Expressions, ')');
                    return node;
            }
            return node;
        }

        protected internal override Expression VisitParameter(ParameterExpression node)
        {
            if (node.IsByRef)
            {
                this.Out("ref ");
            }
            string name = node.Name;
            if (string.IsNullOrEmpty(name))
            {
                this.Out("Param_" + this.GetParamId(node));
                return node;
            }
            this.Out(name);
            return node;
        }

        protected internal override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            this.VisitExpressions<ParameterExpression>('(', node.Variables, ')');
            return node;
        }

        protected internal override Expression VisitSwitch(SwitchExpression node)
        {
            this.Out("switch ");
            this.Out("(");
            this.Visit(node.SwitchValue);
            this.Out(") { ... }");
            return node;
        }

        protected override SwitchCase VisitSwitchCase(SwitchCase node)
        {
            this.Out("case ");
            this.VisitExpressions<Expression>('(', node.TestValues, ')');
            this.Out(": ...");
            return node;
        }

        protected internal override Expression VisitTry(TryExpression node)
        {
            this.Out("try { ... }");
            return node;
        }

        protected internal override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            this.Out("(");
            this.Visit(node.Expression);
            switch (node.NodeType)
            {
                case ExpressionType.TypeIs:
                    this.Out(" Is ");
                    break;

                case ExpressionType.TypeEqual:
                    this.Out(" TypeEqual ");
                    break;
            }
            this.Out(node.TypeOperand.Name);
            this.Out(")");
            return node;
        }

        protected internal override Expression VisitUnary(UnaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.TypeAs:
                    this.Out("(");
                    break;

                case ExpressionType.Decrement:
                    this.Out("Decrement(");
                    break;

                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                    this.Out("-");
                    break;

                case ExpressionType.UnaryPlus:
                    this.Out("+");
                    break;

                case ExpressionType.Not:
                    this.Out("Not(");
                    break;

                case ExpressionType.Quote:
                    break;

                case ExpressionType.Increment:
                    this.Out("Increment(");
                    break;

                case ExpressionType.Throw:
                    this.Out("throw(");
                    break;

                case ExpressionType.PreIncrementAssign:
                    this.Out("++");
                    break;

                case ExpressionType.PreDecrementAssign:
                    this.Out("--");
                    break;

                case ExpressionType.OnesComplement:
                    this.Out("~(");
                    break;

                default:
                    this.Out(node.NodeType.ToString());
                    this.Out("(");
                    break;
            }
            this.Visit(node.Operand);
            switch (node.NodeType)
            {
                case ExpressionType.PreIncrementAssign:
                case ExpressionType.PreDecrementAssign:
                    return node;

                case ExpressionType.PostIncrementAssign:
                    this.Out("++");
                    return node;

                case ExpressionType.PostDecrementAssign:
                    this.Out("--");
                    return node;

                case ExpressionType.TypeAs:
                    this.Out(" As ");
                    this.Out(node.Type.Name);
                    this.Out(")");
                    return node;

                case ExpressionType.Negate:
                case ExpressionType.UnaryPlus:
                case ExpressionType.NegateChecked:
                    return node;

                case ExpressionType.Quote:
                    return node;
            }
            this.Out(")");
            return node;
        }
    }
}

