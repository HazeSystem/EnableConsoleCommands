using Harmony12;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;

namespace EnableConsoleCommands
{
    public static class ModEntry
    {
        static void Load(UnityModManager.ModEntry modEntry)
        {
            var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
         }

        [HarmonyPatch(typeof(SimpleConsole))]
        [HarmonyPatch("Update")]
        static class Patch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var m_isEditor = AccessTools.Method(typeof(Application), "get_isEditor");
                var m_GetBool = AccessTools.Method(typeof(Tokens), "GetBool");

                var list = instructions.ToList();
                var idxStart = list.FindIndex(code => code.operand == m_isEditor);
                var idxEnd = (list.FindIndex(code => code.operand == m_GetBool) + 1);
                list.RemoveRange(idxStart, idxEnd - idxStart + 1);

                return list.AsEnumerable();
            }
        }
    }
}
