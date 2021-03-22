
using System.Collections.Generic;
using ClassicUO.Game.Data;
using ClassicUO.Game.GameObjects;
using ClassicUO.Game.Map;
using ClassicUO.Game.UI.Gumps;
using ClassicUO.Utility.Platforms;
using Microsoft.Xna.Framework;
using ClassicUO.Network;
using System.Linq;

/* giga487, inizializzazione classe */


namespace ClassicUO.Game.Managers
{
    public struct GuildMember
    {
        public string name;
        public uint serial;


    }
    internal class GuildManager
    {
        uint counterMember = 0;
        static public List<GuildMember> guildMem;

        public GuildManager(uint counter = 1)
        {
            guildMem = new List<GuildMember>();
            counterMember = counter;
        }
        public void AddMember(string memberName, uint serialM)
        {
            if(guildMem.Any(r => r.serial == serialM))    
                guildMem.Add(new GuildMember {name = memberName, serial = serialM });
        }

        public void DeleteMember(uint serialM)
        {
            if (guildMem.Any(r => r.serial == serialM))
                guildMem.RemoveAll(r => r.serial == serialM);
        }

        public void ClearMember()
        {
            guildMem.Clear();
        }
        public void ParsePacket(ref PacketBufferReader p)
        {
            byte code = p.ReadByte();

            switch(code)
            {
                case 0: /* first read of guild member */

                    byte count = p.ReadByte();

                    for (int i = 0; i < count; i++)
                    {
                        AddMember(p.ReadUnicode(), p.ReadUInt()); // vengono inviati in modo diverso ora, nome, seriale 
                    }

                    break;

                case 1: // questo messaggio viene inviato quando aggiungo un membro della gilda 
                    DeleteMember(p.ReadUInt());  // leggo il seriale
                    break;
            }
        }

        char[] charNonVedere = { '\0' };
        public string GetName(uint serial)
        {

            foreach (var e in guildMem)
            {
                if (e.serial == serial)
                {
                    return e.name.Replace("\0", string.Empty); /* questo perchè la stringa termina con Null, l'ho eliminato */
                }
            }


            return ""; /* giga487, vuol dire che l'elemento che esiste, non esiste */
        }
    }

}




