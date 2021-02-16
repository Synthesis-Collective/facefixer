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
    }
}
