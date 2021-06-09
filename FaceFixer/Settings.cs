using Mutagen.Bethesda;
using System;
using System.Collections.Generic;
using System.Text;

namespace FaceFixer
{
    public record Settings
    {
        public List<ModKey> TargetMods = new List<ModKey>();
        public bool PrioritizeBySpecifiedOrder = false;
        public bool PatchAttackRace = true;
        public bool PatchDefaultOutfit = false;
        public bool PatchFaceMorph = true;
        public bool PatchFaceParts = true;
        public bool PatchFarAwayModel = true;
        public bool PatchHairColor = true;
        public bool PatchHeadParts = true;
        public bool PatchHeadTexture = true;
        public bool PatchHeight = true;
        public bool PatchSleepingOutfit = false;
        public bool PatchTextureLighting = true;
        public bool PatchTintLayers = true;
        public bool PatchWeight = true;
        public bool PatchWornArmor = true;
    }
}
