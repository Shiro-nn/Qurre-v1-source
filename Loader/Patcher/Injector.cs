using System;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib.DotNet.Writer;
using FieldAttributes = dnlib.DotNet.FieldAttributes;
using MethodAttributes = dnlib.DotNet.MethodAttributes;
using MethodImplAttributes = dnlib.DotNet.MethodImplAttributes;
using TypeAttributes = dnlib.DotNet.TypeAttributes;
namespace Patcher
{
    public class Injector
    {
        ILogger Logger { get; set; }

        public Injector(ILogger logger) => Logger = logger;

        public void Init()
        {
            Inject(Logger.ReadLine().ToLower());
        }
        public void Inject(string com)
        {
            var module = ModuleDefMD.Load(com);
            if (module is null)
            {
                Console.WriteLine("Assembly file not found");
                return;
            }

            module.IsILOnly = true;
            module.VTableFixups = null;
            module.Assembly.PublicKey = null;
            module.Assembly.HasPublicKey = false;

            Console.WriteLine($"Loaded {module.Name}");
            Console.WriteLine("Assembly: Resolving Ref..");

            module.Context = ModuleDef.CreateModuleContext();
            ((AssemblyResolver)module.Context.AssemblyResolver).AddToCache(module);

            Console.WriteLine("Injection of Loader");

            var loader = ModuleDefMD.Load("Loader.dll");//а дальше лень переписывать, потом как-нибудь

            Console.WriteLine($"Injector: Loaded {loader.Name}");

            TypeDef modRefType = default;
            TypeDef initType = default;

            var _list = new System.Collections.Generic.List<TypeDef>();
            foreach (var type in loader.Types) _list.Add(type);
            foreach (var type in _list)
            {
                if (type.Name == "<Module>") continue;
                var _type = type;
                loader.Types.Remove(type);
                _type.DeclaringType = null;
                module.Types.Add(_type);
                Console.WriteLine($"Qurre-Inject: Hooked to: {_type.Namespace}.{_type.Name}");
                if (_type.Name == "Loader") modRefType = _type;
                else if (_type.Name == "MainInitializator") initType = _type;
            }

            for (int i = 3; i < 7; i++)
            {
                byte bb = (byte)i;
                var f = FindType(module.Assembly, "Respawning.SpawnableTeamType").Fields[1];
                string Name = "Null";
                if (bb == 3) Name = "ClassD";
                else if (bb == 4) Name = "Scientist";
                else if (bb == 5) Name = "SCP";
                else if (bb == 6) Name = "Tutorial";
                var field = new FieldDefUser(Name, f.FieldSig, f.Attributes)
                {
                    InitialValue = f.InitialValue,
                    Constant = new ConstantUser
                    {
                        Type = ElementType.U1,
                        Value = bb
                    }
                };
                FindType(module.Assembly, "Respawning.SpawnableTeamType").Fields.Add(field);
            }
            foreach (var n in module.Assembly.Modules.SelectMany(t => t.Types).Where(x => x.Name == "RoundSummary")
                .SelectMany(t => t.NestedTypes))
            {
                if (n.Name == "LeadingTeam")
                    n.Attributes = TypeAttributes.Public;
            }

            var call = FindMethod(modRefType, "LoadModSystem");

            if (call == null)
            {
                Console.WriteLine("Failed to get 'LoadModSystem'");
                return;
            }

            var callReal = FindMethod(initType, "Init");

            if (callReal == null)
            {
                Console.WriteLine("Failed to get 'Init'");
                return;
            }

            Console.WriteLine("Qurre-Inject: Injected!");
            Console.WriteLine("Qurre: Patching...");

            var def = FindType(module.Assembly, "ServerConsole");

            MethodDef bctor = FindMethod(def, "Start");

            if (bctor == null)
            {
                bctor = new MethodDefUser("Start", MethodSig.CreateInstance(module.CorLibTypes.Void),
                   MethodImplAttributes.IL | MethodImplAttributes.Managed,
                   MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
                def.Methods.Add(bctor);
            }
            bctor.Body.Instructions.Insert(0, OpCodes.Call.ToInstruction(call));
            bctor.Body.Instructions.Insert(bctor.Body.Instructions.Count() - 1, OpCodes.Call.ToInstruction(callReal));

            module.Write("Assembly-CSharp.dll");

            Console.WriteLine("Qurre: Patch Complete!");

            Console.WriteLine("Qurre-Public: Creating Publicized DLL");

            var expTypes = module.Assembly.Modules.SelectMany(t => t.ExportedTypes);
            var allTypes = module.Assembly.Modules.SelectMany(t => t.Types);
            var allMethods = allTypes.SelectMany(t => t.Methods);
            var allFields = allTypes.SelectMany(t => t.Fields);
            var allNt = allTypes.SelectMany(t => t.NestedTypes);
            var allNtMethods = allNt.SelectMany(t => t.Methods);
            var allNtFields = allNt.SelectMany(t => t.Fields);
            foreach (var exprt in expTypes)
            {
                if (!exprt?.IsPublic ?? false)
                {
                    exprt.Attributes = exprt.IsNested ? TypeAttributes.NestedPublic : TypeAttributes.Public;
                }
            }
            foreach (var type in allTypes)
            {
                if (!type?.IsPublic ?? false)
                {
                    type.Attributes = type.IsNested ? TypeAttributes.NestedPublic : TypeAttributes.Public;
                }
            }
            foreach (var method in allMethods)
            {
                if (!method?.IsPublic ?? false)
                {
                    method.Access = MethodAttributes.Public;
                }
            }
            foreach (var field in allFields)
            {
                if (!field?.IsPublic ?? false)
                {
                    if (field.Name != "OnMapGenerated")
                        field.Access = FieldAttributes.Public;
                }
            }
            foreach (var method in allNtMethods)
            {
                if (!method?.IsPublic ?? false)
                {
                    method.Access = MethodAttributes.Public;
                }
            }
            foreach (var field in allNtFields)
            {
                if (!field?.IsPublic ?? false)
                {
                    if (field.Name != "OnMapGenerated")
                        field.Access = FieldAttributes.Public;
                }
            }
            FindType(module.Assembly, "MapGeneration.Distributors.Scp079Generator").NestedTypes.Where(x => x.Name == "GeneratorFlags")
                .FirstOrDefault().Attributes = TypeAttributes.NestedPublic;
            module.Write("Assembly-CSharp_publicized.dll");

            Console.WriteLine("Qurre-Public: Created Publicised DLL");
        }

        private static MethodDef FindMethod(TypeDef type, string methodName)
        {
            return type?.Methods.FirstOrDefault(method => method.Name == methodName);
        }

        private static TypeDef FindType(AssemblyDef asm, string classPath)
        {
            return asm.Modules.SelectMany(module => module.Types).FirstOrDefault(type => type.FullName == classPath);
        }
    }
}