using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassicUO.Game.Managers;

namespace ClassicUO.Game.UoMars
{
    class UOMarsPartyWrapper: PartyManager
    {

        public string GetName(uint serial)
        {
            if (Contains(serial))
            {
                foreach (var e in World.Party.Members)
                {
                    if (e.Serial == serial)
                    {
                        return e.Name;
                    }
                }
            }

            return "errore incomprensibile"; /* giga487, vuol dire che l'elemento che esiste, non esiste */
        }
    }
}
