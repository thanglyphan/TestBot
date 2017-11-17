using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace TestBot.Dialogs.Assets
{

    [Flags]
    public enum Enums
    {
        Tom = 0,
        Utlegg,
        Lønn,
        Pensjon,
        Feriepenger
    }
}
