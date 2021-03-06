﻿/*Copyright or © or Copr. THOUVENIN Alexandre (2013)

nem-e5i5software@live.fr

This software is a computer program whose purpose is to [describe
functionalities and technical features of your software].

This software is governed by the CeCILL-C license under French law and
abiding by the rules of distribution of free software.  You can  use, 
modify and/ or redistribute the software under the terms of the CeCILL-C
license as circulated by CEA, CNRS and INRIA at the following URL
"http://www.cecill.info". 

As a counterpart to the access to the source code and  rights to copy,
modify and redistribute granted by the license, users are provided only
with a limited warranty  and the software's author,  the holder of the
economic rights,  and the successive licensors  have only  limited
liability. 

In this respect, the user's attention is drawn to the risks associated
with loading,  using,  modifying and/or developing or reproducing the
software by the user in light of its specific status of free software,
that may mean  that it is complicated to manipulate,  and  that  also
therefore means  that it is reserved for developers  and  experienced
professionals having in-depth computer knowledge. Users are therefore
encouraged to load and test the software's suitability as regards their
requirements in conditions enabling the security of their systems and/or 
data to be ensured and,  more generally, to use and operate it in the 
same conditions as regards security. 

The fact that you are presently reading this means that you have had
knowledge of the CeCILL-C license and that you accept its terms.*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;
using Autolithium.core;
using System.Net;
using System.Linq.Expressions;

namespace Autolithium.compiler
{
    partial class Program
    {
        //public static List<FunctionDefinition> DefinedMethodInfo = new List<FunctionDefinition>();
        public static List<FieldBuilder> GlobalVars = new List<FieldBuilder>();

        public static MethodBuilder CompileMain(ModuleBuilder M, string Script, params Assembly[] References)
        {
            var T = M.DefineType("Autolithium-" + Script.GetHashCode());
            MethodBuilder DelegateBinder = T.DefineMethod("&=.DelegateBinder", MethodAttributes.Static);
            ASMInclude.ResxLoader(T, DelegateBinder);

            var ASMScope = new AssemblyScope(M, T);
            var lambda = LiParser.Parse(
                Script,
                ASMScope,
                References);

            ASMInclude.GenerateDelegateBinder(DelegateBinder, ASMScope.ScopeFunctions);
            MethodBuilder Method = null;
            if (lambda != null)
            {
                Method = T.DefineMethod("Autolithium-Main", MethodAttributes.Public | MethodAttributes.Static, typeof(int), new Type[] { typeof(string[]) });
                lambda.CompileToMethod(Method);
            }
            T.CreateType();
            return Method;
        }
    }
}
