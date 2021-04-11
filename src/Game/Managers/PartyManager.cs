#region license

// Copyright (c) 2021, andreakarasho
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 1. Redistributions of source code must retain the above copyright
//    notice, this list of conditions and the following disclaimer.
// 2. Redistributions in binary form must reproduce the above copyright
//    notice, this list of conditions and the following disclaimer in the
//    documentation and/or other materials provided with the distribution.
// 3. All advertising materials mentioning features or use of this software
//    must display the following acknowledgement:
//    This product includes software developed by andreakarasho - https://github.com/andreakarasho
// 4. Neither the name of the copyright holder nor the
//    names of its contributors may be used to endorse or promote products
//    derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS ''AS IS'' AND ANY
// EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

using System;
using ClassicUO.Configuration;
using ClassicUO.Game.Data;
using ClassicUO.Game.GameObjects;
using ClassicUO.Game.UI.Gumps;
using ClassicUO.Network;
using ClassicUO.Resources;
using System.Linq;

namespace ClassicUO.Game.Managers
{
    internal class PartyManager
    {
        private const int PARTY_SIZE = 10;
        public uint Leader { get; set; }
        public uint Inviter { get; set; }
        public bool CanLoot { get; set; }

        public PartyMember[] Members { get; } = new PartyMember[PARTY_SIZE];

        uint actualIndex = 0;
        public void AddMember(string name, uint serial)
        {
            if(!Contains(serial) && actualIndex < 10)
            {
                Members[actualIndex] = new PartyMember(serial, name);
                actualIndex++;
            }
        }

        public long PartyHealTimer { get; set; }
        public uint PartyHealTarget { get; set; }

        public void ParsePacket(ref PacketBufferReader p)
        {
            byte code = p.ReadByte();

            bool add = false;

            switch (code)
            {
                case 1:
                    add = true;
                    goto case 2;

                case 2:
                    byte count = p.ReadByte();

                    if (count <= 1)
                    {
                        Leader = 0;
                        Inviter = 0;

                        for (int i = 0; i < PARTY_SIZE; i++)
                        {
                            if (Members[i] == null || Members[i].Serial == 0)
                            {
                                break;
                            }

                            BaseHealthBarGump gump = UIManager.GetGump<BaseHealthBarGump>(Members[i].Serial);


                            if (gump != null)
                            {
                                if (code == 2)
                                {
                                    Members[i].Serial = 0;
                                }

                                gump.RequestUpdateContents();
                            }
                        }

                        Clear();

                        UIManager.GetGump<PartyGump>()?.RequestUpdateContents();

                        break;
                    }

                    Clear();

                    uint to_remove = 0xFFFF_FFFF;

                    if (!add)
                    {
                        to_remove = p.ReadUInt();

                        UIManager.GetGump<BaseHealthBarGump>(to_remove)?.RequestUpdateContents();
                    }

                    bool remove_all = !add && to_remove == World.Player;
                    int done = 0;

                    for (int i = 0; i < count; i++)
                    {
                        uint serial = p.ReadUInt();
                        bool remove = !add && serial == to_remove;

                        if (remove && serial == to_remove && i == 0)
                        {
                            remove_all = true;
                        }

                        if (!remove && !remove_all)
                        {
                            if (!Contains(serial))
                            {
                                Members[i] = new PartyMember(serial);
                            }

                            done++;
                        }

                        if (i == 0 && !remove && !remove_all)
                        {
                            Leader = serial;
                        }

                        BaseHealthBarGump gump = UIManager.GetGump<BaseHealthBarGump>(serial);

                        if (gump != null)
                        {
                            gump.RequestUpdateContents();
                        }
                        else
                        {
                            if (serial == World.Player)
                            {
                            }
                        }
                    }

                    if (done <= 1 && !add)
                    {
                        for (int i = 0; i < PARTY_SIZE; i++)
                        {
                            if (Members[i] != null && SerialHelper.IsValid(Members[i].Serial))
                            {
                                uint serial = Members[i].Serial;

                                Members[i] = null;

                                UIManager.GetGump<BaseHealthBarGump>(serial)?.RequestUpdateContents();
                            }
                        }

                        Clear();
                    }


                    UIManager.GetGump<PartyGump>()?.RequestUpdateContents();

                    break;

                case 3:
                case 4: /* questo è il messaggio di chat */
                    uint ser = p.ReadUInt();
                    byte[] buff = p.Buffer;
                    string text_mex = p.ReadUnicode();  /* è il messaggio, non il nome. */

                    for (int i = 0; i < PARTY_SIZE; i++)
                    {
                        if (Members[i] != null && Members[i].Serial == ser)
                        {
                            MessageManager.HandleMessage
                            (
                                null,
                                text_mex,
                                Members[i].Name,
                                ProfileManager.CurrentProfile.PartyMessageHue,
                                MessageType.Party,
                                3,
                                TextType.GUILD_ALLY
                            );

                            break;
                        }
                    }

                    break;

                case 7:
                    Inviter = p.ReadUInt();

                    if (ProfileManager.CurrentProfile.PartyInviteGump)
                    {
                        UIManager.Add(new PartyInviteGump(Inviter));
                    }

                    break;
            }
        }

        public bool Contains(uint serial)
        {
            for (int i = 0; i < PARTY_SIZE; i++)
            {
                PartyMember mem = Members[i];

                if (mem != null && mem.Serial == serial)
                {
                    return true;
                }
            }

            return false;
        }

        char[] charNonVedere = {'\0'};
        public string GetName(uint serial)
        {
            if(Contains(serial))
            {
                foreach (var e in World.Party.Members)
                {
                    if (e.Serial == serial)
                    {
                        return e.Name.Replace("\0", string.Empty); /* questo perchè la stringa termina con Null, l'ho eliminato */
                    }
                }
            }

            return ""; /* giga487, vuol dire che l'elemento che esiste, non esiste */
        }

        public void Clear()
        {
            Leader = 0;
            Inviter = 0;

            for (int i = 0; i < PARTY_SIZE; i++)
            {
                Members[i] = null;
            }
        }
    }

    internal class PartyMember : IEquatable<PartyMember>
    {
        private string _name;

        public PartyMember(uint serial)
        {
            Serial = serial;
            _name = Name;
        }

        public PartyMember(uint serial, string name)
        {
            Serial = serial;
            _name = name;
        }

        /* questa funzione restituisce i nomi mobiles */
        public string Name
        {
            get
            {
                Mobile mobile = World.Mobiles.Get(Serial);

                if (mobile != null)
                {
                    _name = mobile.Name;

                    if (string.IsNullOrEmpty(_name))
                    {
                        _name = ResGeneral.NotSeeing;
                    }
                }

                return _name;
            }
        }

        public bool Equals(PartyMember other)
        {
            if (other == null)
            {
                return false;
            }

            return other.Serial == Serial;
        }

        public uint Serial;
    }
}