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

using Autolithium.core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Autolithium.compiler
{
    public static class ASMInclude
    {
        public static Assembly IncludeDLL(ModuleBuilder ASM, string Filen, string DestDir = null)
        {
            DestDir = DestDir ?? Directory.GetCurrentDirectory();
            var fstream = File.OpenRead(Filen);
            ASM.DefineManifestResource(Path.GetFileName(Filen), fstream, ResourceAttributes.Public);
            
            //r.Generate();
            return Assembly.LoadFrom(Filen);
        }
        public static ConstructorBuilder ResxLoader(TypeBuilder T, MethodInfo Next = null)
        {
            var Method = T.DefineMethod("&=.EventH", MethodAttributes.Static | MethodAttributes.Public, typeof(Assembly), new Type[] {typeof(object), typeof(ResolveEventArgs)});
            ILGenerator EH = Method.GetILGenerator();

            EH.DeclareLocal(typeof(string));
            EH.DeclareLocal(typeof(Stream));
            EH.DeclareLocal(typeof(byte[]));
            EH.DeclareLocal(typeof(Assembly));

            EH.Emit(OpCodes.Nop);
            EH.Emit(OpCodes.Ldstr, "");
            EH.Emit(OpCodes.Ldarg_1); 
            EH.Emit(OpCodes.Callvirt, typeof(ResolveEventArgs).GetMethod("get_Name"));
            EH.Emit(OpCodes.Newobj, typeof(AssemblyName).GetConstructor(new Type[] {typeof(string)}));
            EH.Emit(OpCodes.Call, typeof(AssemblyName).GetMethod("get_Name"));
            EH.Emit(OpCodes.Ldstr, ".dll");
            EH.Emit(OpCodes.Call, typeof(string).GetTypeInfo().GetMethod("Concat", new Type[] {typeof(string), typeof(string), typeof(string)}));
            EH.Emit(OpCodes.Stloc_0);
            EH.Emit(OpCodes.Ldtoken, T);
            EH.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
            EH.Emit(OpCodes.Callvirt, typeof(Type).GetProperty("Assembly").GetGetMethod());
            EH.Emit(OpCodes.Ldloc_0); 
            EH.Emit(OpCodes.Callvirt, typeof(Assembly).GetMethod("GetManifestResourceStream", new Type[] {typeof(string)}));
            EH.Emit(OpCodes.Stloc_1);
            EH.Emit(OpCodes.Ldloc_1);
            EH.Emit(OpCodes.Callvirt, typeof(Stream).GetProperty("Length").GetGetMethod());
            EH.Emit(OpCodes.Conv_Ovf_I);
            EH.Emit(OpCodes.Newarr, typeof(byte));
            EH.Emit(OpCodes.Stloc_2);
            EH.Emit(OpCodes.Ldloc_1); 
            EH.Emit(OpCodes.Ldloc_2); 
            EH.Emit(OpCodes.Ldc_I4_0); 
            EH.Emit(OpCodes.Ldloc_2); 
            EH.Emit(OpCodes.Ldlen); 
            EH.Emit(OpCodes.Conv_I4);
            EH.Emit(OpCodes.Callvirt, typeof(Stream).GetMethod("Read", new Type[] {typeof(byte[]), typeof(int), typeof(int)}));
            EH.Emit(OpCodes.Pop);
            EH.Emit(OpCodes.Ldloc_2);
            EH.Emit(OpCodes.Call, typeof(Assembly).GetMethod("Load", new Type[] {typeof(byte[])}));
            EH.Emit(OpCodes.Stloc_3);
            var s = EH.DefineLabel();
            EH.Emit(OpCodes.Br_S, s);
            EH.MarkLabel(s);
            EH.Emit(OpCodes.Ldloc_3);
            
            EH.Emit(OpCodes.Ret);

            

            var Ret = T.DefineConstructor(MethodAttributes.Static | MethodAttributes.Private, System.Reflection.CallingConventions.Standard, new Type[]{});
            ILGenerator IL = Ret.GetILGenerator();
            IL.Emit(OpCodes.Nop);
            IL.EmitCall(OpCodes.Call, typeof(AppDomain).GetTypeInfo().GetDeclaredProperty("CurrentDomain").GetGetMethod(), null);
            IL.Emit(OpCodes.Ldnull);
            IL.Emit(OpCodes.Ldftn, Method);
            IL.Emit(OpCodes.Newobj,
                typeof(ResolveEventHandler).GetTypeInfo().GetConstructors().First());
            IL.EmitCall(OpCodes.Callvirt, typeof(AppDomain).GetTypeInfo().GetMethod("add_AssemblyResolve"), null);
            IL.Emit(OpCodes.Nop);
            if (Next != null) IL.EmitCall(OpCodes.Call, Next, null);
            IL.Emit(OpCodes.Ret);
            return Ret;

        }
        public static void GenerateDelegateBinder(MethodBuilder Dest, IEnumerable<FunctionMeta> D)
        {
            var IL = Dest.GetILGenerator();
            var FDefine = typeof(Autcorlib).GetTypeInfo().DeclaredMethods.First(x => x.Name == "FUNCTIONDEFINE");
            var FReg = typeof(Autcorlib).GetTypeInfo().DeclaredMethods.First(x => x.Name == "FUNCTIONREGISTER");
            foreach (var f in D)
            {
                IL.Emit(OpCodes.Ldstr, f.Name);
                IL.EmitCall(OpCodes.Call, FDefine, null);
                IL.Emit(OpCodes.Ldstr, f.Name);
                IL.Emit(OpCodes.Ldnull);
                IL.Emit(OpCodes.Ldftn, ((AssemblyFunction)f.Information).Method);
                IL.Emit(OpCodes.Newobj,
                    ((AssemblyFunction)f.Information).DelegateType.GetTypeInfo().GetConstructors().First());
                IL.Emit(OpCodes.Stsfld, ((AssemblyFunction)f.Information).DelegateField);
                IL.Emit(OpCodes.Ldsfld, ((AssemblyFunction)f.Information).DelegateField);
                IL.EmitCall(OpCodes.Call, FReg, null);
            }
            IL.Emit(OpCodes.Ret);
        }
        public static Assembly RequireDLL(string Filen, string DestDir = null)
        {
            DestDir = DestDir ?? Directory.GetCurrentDirectory();
            File.Copy(Filen, Path.Combine(DestDir, Path.GetFileName(Filen)), true);
            return Assembly.LoadFrom(Filen);
        }
        public static Assembly RequireNetFXASM(string ASMName)
        {
            return Assembly.Load(new AssemblyName(ASMName));
        }
    }
}
