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
            string path = @"facefixer.json";
            if (!File.Exists(path))
            {
                throw new System.Exception("'facefixer.json' must exist in the same folder as 'FaceFixer.exe'");

            }

            TextReader textReader = File.OpenText(path);
            dynamic files = JsonConvert.DeserializeObject<HashSet<ModKey>>(textReader.ReadToEnd());

            if (files == null)
            {
                throw new System.Exception("'facefixer.json' must contain a single array of strings of plugin names");
            }

            foreach (var npc in state.LoadOrder.PriorityOrder.WinningOverrides<INpcGetter>())
            {
                foreach (var listing in state.LoadOrder.PriorityOrder.Reverse())
                {
                    if (listing.Mod == null) continue;

                    if (!listing.Mod.Npcs.RecordCache.TryGetValue(npc.FormKey, out var sourceNpc)) continue;

                    if (!files.Contains(listing.Mod.ModKey)) continue;

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
                }
            }
        }
    }
}
