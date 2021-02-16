using Mutagen.Bethesda;
using System;
using System.Collections.Generic;
using System.Text;

namespace FaceFixer
{
    public record Settings
    {
        public HashSet<ModKey> TargetMods = new HashSet<ModKey>();
    }
}
