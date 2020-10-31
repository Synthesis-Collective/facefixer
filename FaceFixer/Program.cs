using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;

namespace FaceFixer
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return SynthesisPipeline.Instance.Patch<ISkyrimMod, ISkyrimModGetter>(
                args: args,
                patcher: RunPatch,
                new UserPreferences() { 
                    ActionsForEmptyArgs = new RunDefaultPatcher()
                    {
                        IdentifyingModKey = "FaceFixer.esp",
                        TargetRelease = GameRelease.SkyrimSE
                    }
                }
            );
        }
        public static void RunPatch(SynthesisState<ISkyrimMod, ISkyrimModGetter> state)
        {
            string path = Path.Combine(state.ExtraSettingsDataPath, "facefixer.json");
            if (!File.Exists(path))
            {
                System.Console.WriteLine("'facefixer.json' did not exist in user data folder");
                return;
            }

            TextReader textReader = File.OpenText(path);
            var files = JsonConvert.DeserializeObject<HashSet<ModKey>>(textReader.ReadToEnd());

            if (files == null || files.Count == 0)
            {
                System.Console.WriteLine("'facefixer.json' must contain a single array of strings of plugin names");
                return;
            }

            System.Console.WriteLine("Files to map to:");
            foreach (var file in files)
            {
                System.Console.WriteLine($"  {file.FileName}");
            }

            var npcGroups = state.LoadOrder.PriorityOrder
                .Reverse()
                .Where(listing => listing.Mod != null)
                .Select(x => (ModKey: x.ModKey, Npcs: x.Mod!.Npcs))
                .ToList();

            uint count = 0;

            foreach (var npc in state.LoadOrder.PriorityOrder.WinningOverrides<INpcGetter>())
            {
                foreach (var npcGroup in npcGroups)
                {
                    if (!files.Contains(npcGroup.ModKey)) continue;

                    if (!npcGroup.Npcs.RecordCache.TryGetValue(npc.FormKey, out var sourceNpc)) continue;

                    var modifiedNpc = state.PatchMod.Npcs.GetOrAddAsOverride(npc);
                    modifiedNpc.DeepCopyIn(sourceNpc, new Npc.TranslationMask(false)
                    {
                        AttackRace = true,
                        FaceMorph = true,
                        FaceParts = true,
                        FarAwayModel = true,
                        HairColor = true,
                        HeadParts = true,
                        HeadTexture = true,
                        Height = true,
                        TextureLighting = true,
                        TintLayers = true,
                        Weight = true,
                        WornArmor = true,
                    });
                    count++;
                }
            }

            System.Console.WriteLine($"Patched {count} Npcs");
        }
    }
}
